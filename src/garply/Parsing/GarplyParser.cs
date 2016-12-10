﻿using System.Collections.Generic;
using System.Linq;
using System;
using Garply.Sprache;
using System.Diagnostics;

namespace Garply
{
    internal class GarplyParser
    {
        private delegate IResult<T> TryParse<T>(string input);

        private readonly Parser<Value> _mainParser;
        private readonly ErrorContext _errorContext;
        private readonly TryParse<Value> _tryParse;

        public GarplyParser(ErrorContext errorContext, Scope.Builder scopeBuilder)
        {
            Parser<Value> mainParser = null;
            _mainParser = Parse.Ref(() => mainParser);

            _errorContext = errorContext;

            var assignmentParser = GetAssignmentParser(scopeBuilder);
            var literalParser = GetLiteralExpressionParser(scopeBuilder);

            mainParser =
                assignmentParser
                    .Or(literalParser)
                    .Token();

            _tryParse = mainParser.TryParse;
        }

        private Parser<VariableDefinition> GetVariableDefinitionParser()
        {
            var variableDefinitionParser =
                from mutableMarker in Parse.Char('$').Once().Optional()
                from first in Parse.Letter.Once()
                from rest in Parse.LetterOrDigit.Or(Parse.Chars('_', '-')).Many()
                select new VariableDefinition
                {
                    Name = new string(
                        mutableMarker.GetOrElse(Enumerable.Empty<char>())
                            .Concat(first)
                            .Concat(rest).ToArray()),
                    Mutable = mutableMarker.IsDefined
                };
            return variableDefinitionParser;
        }

        private Parser<Value> GetAssignmentParser(Scope.Builder scopeBuilder)
        {
            var assignmentParser =
                from variableDefinition in GetVariableDefinitionParser()
                from assignmentOperator in Parse.Char('=').Token()
                from value in _mainParser
                select GetAssignmentExpression(scopeBuilder, variableDefinition.Name, value, variableDefinition.Mutable);
            return assignmentParser;
        }

        private Value GetAssignmentExpression(Scope.Builder scopeBuilder, string variableName, Value valueExpression, bool mutable)
        {
            if (valueExpression.Type == Types.Error) return valueExpression;
            var instructions = new List<Instruction>();
            var expression = Heap.GetExpression((int)valueExpression.Raw);
            instructions.AddRange(expression.Instructions);
            instructions.Add(Instruction.AssignVariable(scopeBuilder.GetOrCreateIndex(variableName), mutable));
            valueExpression.RemoveRef();
            return AllocateExpression(expression.Type, instructions.ToArray());
        }

        private Parser<Value> GetVariableParser(Scope.Builder scopeBuilder)
        {
            var variableParser =
                from variableDefinition in GetVariableDefinitionParser()
                let variableIndex = GetVariableIndex(variableDefinition.Name, scopeBuilder)
                select variableIndex.Type == Types.Error ? default(Value) :
                    AllocateExpression(Types.Any, new[]
                    {
                        Instruction.ReadVariable(variableIndex)
                    });
            return variableParser;
        }

        public Value ParseLine(string line)
        {
            var result = _tryParse(line);

            if (result.WasSuccessful)
            {
                if (!result.Remainder.AtEnd)
                {
                    _errorContext.AddError(new Error("Partial line match - the whole line must be parsable as a single expression."));
                    return default(Value);
                }

                return result.Value;
            }
            else
            {
                _errorContext.AddError(new Error($"Parse error: {result.Message}."));
                return default(Value);
            }
        }

        private Parser<Value> GetLiteralExpressionParser(Scope.Builder scopeBuilder)
        {
            var booleanParser = GetBooleanLiteralParser();
            var integerParser = GetIntegerLiteralParser();
            var floatParser = GetFloatLiteralParser();
            var typeParser = GetTypeLiteralParser();
            var stringParser = GetStringLiteralParser();
            var tupleParser = GetTupleLiteralParser();
            var listParser = GetListLiteralParser();
            var variableParser = GetVariableParser(scopeBuilder);
            // TODO: Expression parser?

            var literalParser =
                booleanParser.Or(floatParser).Or(integerParser).Or(typeParser).Or(stringParser).Or(tupleParser).Or(listParser).Or(variableParser);

            return literalParser;
        }

        private Value GetVariableIndex(string variableName, Scope.Builder scopeBuilder)
        {
            int index;
            if (!scopeBuilder.TryGetIndex(variableName, out index))
            {
                // TODO: error context?
                return default(Value);
            }
            return new Value(index);
        }

        private Parser<Value> GetBooleanLiteralParser()
        {
            var trueParser =
                from t in Parse.String("true")
                select AllocateExpression(Types.Boolean, new Instruction[]
                {
                    Instruction.True()
                });

            var falseParser =
                from f in Parse.String("false")
                select AllocateExpression(Types.Boolean, new Instruction[]
                {
                    Instruction.False()
                });

            return trueParser.Or(falseParser);
        }

        private Parser<Value> GetIntegerLiteralParser()
        {
            var integerParser =
                from negate in Parse.Char('-').Optional()
                from digits in Parse.Numeric.AtLeastOnce()
                let value = ParseLong(digits, negate.IsDefined)
                select value.Type == Types.Error ? value :
                    AllocateExpression(Types.Integer, new Instruction[]
                    {
                        Instruction.LoadInteger(value)
                    });

            return integerParser;
        }

        private Value ParseLong(IEnumerable<char> digits, bool negate)
        {
            var stringValue = new string(Once('-', negate).Concat(digits).ToArray());
            try
            {
                return new Value(long.Parse(stringValue));
            }
            catch (Exception ex)
            {
                _errorContext.AddError(new Error(ex.Message));
                _errorContext.AddError(new Error($"Error parsing int value '{stringValue}'"));
                return default(Value);
            }
        }

        private Parser<Value> GetFloatLiteralParser()
        {
            var integerParser =
                from negate in Parse.Char('-').Optional()
                from wholePart in Parse.Numeric.Many()
                from dot in Parse.Char('.')
                from fractionalPart in Parse.Numeric.AtLeastOnce()
                let value = ParseFloat(wholePart, fractionalPart, negate.IsDefined)
                select value.Type == Types.Error ? value :
                    AllocateExpression(Types.Float, new Instruction[]
                    {
                        Instruction.LoadFloat(value)
                    });

            return integerParser;
        }

        private Value ParseFloat(IEnumerable<char> wholePart, IEnumerable<char> fractionalPart, bool negate)
        {
            var stringValue = new string(
                Once('-', negate)
                    .Concat(wholePart)
                    .Concat(Once('.'))
                    .Concat(fractionalPart).ToArray());
            try
            {
                return new Value(double.Parse(stringValue));
            }
            catch (Exception ex)
            {
                _errorContext.AddError(new Error(ex.Message));
                _errorContext.AddError(new Error($"Error parsing float value '{stringValue}'"));
                return default(Value);
            }
        }

        private Parser<Value> GetTypeLiteralParser()
        {
            Parser<Value> typeParser = null;

            foreach (Types type in Enum.GetValues(typeof(Types)))
            {
                var parser = from v in Parse.String(type.ToString())
                             select new Value(type);
                typeParser =
                    typeParser == null
                    ? parser
                    : typeParser.Or(parser);
            }

            return
                from openBrace in Parse.Char('<')
                from type in typeParser
                from closeBrace in Parse.Char('>')
                select AllocateExpression(Types.Type, new Instruction[]
                {
                    Instruction.LoadType(type)
                });
        }

        private Parser<Value> GetStringLiteralParser()
        {
            var stringParser =
                from openQuote in Parse.Char('"')
                from value in
                    (from q1 in Parse.Char('"') from q2 in Parse.Char('"') select '"')
                    .Or(Parse.CharExcept('"'))
                    .Many().Text()
                from closeQuote in Parse.Char('"')
                select AllocateExpression(Types.String, GetStringLiteralRetreivalInstructions(value));

            return stringParser;
        }

        private Instruction[] GetStringLiteralRetreivalInstructions(string value)
        {
            var stringValue = StringDatabase.Register(value);
            return new[]
            {
                Instruction.LoadString(stringValue)
            };
        }

        private Parser<Value> GetTupleLiteralParser()
        {
            var tupleParser =
                from openParen in Parse.Char('{')
                from items in _mainParser.DelimitedBy(Parse.Char(',').Token())
                from closeParen in Parse.Char('}')
                select GetCreateTupleExpression(items);
            return tupleParser;
        }

        private Value GetCreateTupleExpression(IEnumerable<Value> itemExpressionValues)
        {
            var arity = 0;
            var instructions = new List<Instruction>();
            using (var enumerator = itemExpressionValues.GetEnumerator()) while (enumerator.MoveNext())
            {
                var itemExpressionValue = enumerator.Current;
                if (itemExpressionValue.Type == Types.Error)
                {
                    while (enumerator.MoveNext()) enumerator.Current.RemoveRef();
                    return itemExpressionValue;
                }
                arity++;
                Debug.Assert(itemExpressionValue.Type == Types.Expression);
                instructions.AddRange(Heap.GetExpression((int)itemExpressionValue.Raw).Instructions);
                itemExpressionValue.RemoveRef();
            }
            instructions.Add(Instruction.NewTuple(arity));
            var expressionValue = AllocateExpression(Types.Tuple, instructions.ToArray());
            return expressionValue;
        }

        private Parser<Value> GetListLiteralParser()
        {
            var tupleParser =
                from openParen in Parse.Char('[')
                from items in _mainParser.DelimitedBy(
                    from w1 in Parse.WhiteSpace.Many()
                    from c in Parse.Char(',')
                    from w2 in Parse.WhiteSpace.Many()
                    select c)
                from closeParen in Parse.Char(']')
                select GetCreateListExpression(items);

            return tupleParser;
        }

        private Value GetCreateListExpression(IEnumerable<Value> itemExpressionValues)
        {
            var instructions = new List<Instruction>();
            instructions.Add(Instruction.ListEmpty());
            using (var enumerator = itemExpressionValues.GetEnumerator()) while (enumerator.MoveNext())
            {
                var itemExpressionValue = enumerator.Current;
                if (itemExpressionValue.Type == Types.Error)
                {
                    while (enumerator.MoveNext()) enumerator.Current.RemoveRef();
                    return itemExpressionValue;
                }
                Debug.Assert(itemExpressionValue.Type == Types.Expression);
                instructions.AddRange(Heap.GetExpression((int)itemExpressionValue.Raw).Instructions);
                instructions.Add(Instruction.ListAdd());
                itemExpressionValue.RemoveRef();
            }
            var expressionValue = AllocateExpression(Types.List, instructions.ToArray());
            return expressionValue;
        }

        private static Value AllocateExpression(Types type, Instruction[] instructions)
        {
            var expressionValue = Heap.AllocateExpression(type, instructions);
            expressionValue.AddRef();
            return expressionValue;
        }

        private IEnumerable<T> Once<T>(T t, bool condition = true)
        {
            if (condition) yield return t;
        }

        private class VariableDefinition
        {
            public string Name { get; set; }
            public bool Mutable { get; set; }
        }
    }
}