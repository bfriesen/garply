using System.Collections.Generic;
using System.Linq;
using System;
using Garply.Sprache;
using System.Diagnostics;

namespace Garply
{
    internal class GarplyParser
    {
        private readonly MainParser _mainParser;
        private readonly ErrorContext _errorContext;

        public GarplyParser(ErrorContext errorContext)
        {
            _mainParser = new MainParser();
            _errorContext = errorContext;

            var literalParser = GetLiteralExpressionParser();

            _mainParser.SetParser(
                from whitespace in Parse.WhiteSpace.Many()
                from result in literalParser
                select result);
        }

        public Value ParseLine(string line)
        {
            var result = _mainParser.Parser.TryParse(line);

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

        private Parser<Value> GetLiteralExpressionParser()
        {
            var booleanParser = GetBooleanLiteralParser();
            var integerParser = GetIntegerLiteralParser();
            var floatParser = GetFloatLiteralParser();
            var typeParser = GetTypeLiteralParser();
            var stringParser = GetStringLiteralParser();
            var tupleParser = GetTupleLiteralParser();
            var listParser = GetListLiteralParser();
            // TODO: Expression parser?

            var literalParser =
                booleanParser.Or(floatParser).Or(integerParser).Or(typeParser).Or(stringParser).Or(tupleParser).Or(listParser);

            return literalParser;
        }

        private Parser<Value> GetBooleanLiteralParser()
        {
            var trueParser =
                from t in Parse.String("true")
                select AllocateExpression(Types.Boolean, new Instruction[]
                {
                    new Instruction(Opcode.LoadBoolean, new Value(true))
                });

            var falseParser =
                from f in Parse.String("false")
                select AllocateExpression(Types.Boolean, new Instruction[]
                {
                    new Instruction(Opcode.LoadBoolean, new Value(false))
                });

            return trueParser.Or(falseParser);
        }

        private Parser<Value> GetIntegerLiteralParser()
        {
            var integerParser =
                from negate in Parse.Char('-').Optional()
                from digits in Parse.Numeric.AtLeastOnce()
                let value = ParseLong(digits, negate.IsDefined)
                where value.Type != Types.Error
                select AllocateExpression(Types.Integer, new Instruction[]
                {
                    new Instruction(Opcode.LoadInteger, value)
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
            catch (Exception)
            {
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
                where value.Type != Types.Error
                select AllocateExpression(Types.Float, new Instruction[]
                {
                    new Instruction(Opcode.LoadFloat, value)
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
            catch (Exception)
            {
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
                    new Instruction(Opcode.LoadType, type)
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
                new Instruction(Opcode.LoadString, stringValue)
            };
        }

        private Parser<Value> GetTupleLiteralParser()
        {
            var tupleParser =
                from openParen in Parse.Char('(')
                from items in Parse.Ref(() => _mainParser.Parser).DelimitedBy(Parse.Char(',').Token())
                from closeParen in Parse.Char(')')
                select AllocateExpression(Types.Tuple, GetTupleLiteralCreationInstructions(items));
            return tupleParser;
        }

        private Instruction[] GetTupleLiteralCreationInstructions(IEnumerable<Value> items)
        {
            var arity = 0;
            var instructions = new List<Instruction>();
            foreach (var item in items)
            {
                arity++;
                Debug.Assert(item.Type == Types.Expression);
                instructions.AddRange(Heap.GetExpression((int)item.Raw).Instructions);
                item.RemoveRef();
            }
            instructions.Add(new Instruction(Opcode.NewTuple, new Value(arity)));
            return instructions.ToArray();
        }

        private Parser<Value> GetListLiteralParser()
        {
            var tupleParser =
                from openParen in Parse.Char('[')
                from items in Parse.Ref(() => _mainParser.Parser).DelimitedBy(
                    from w1 in Parse.WhiteSpace.Many()
                    from c in Parse.Char(',')
                    from w2 in Parse.WhiteSpace.Many()
                    select c)
                from closeParen in Parse.Char(']')
                select AllocateExpression(Types.List, GetListLiteralCreationInstructions(items));

            return tupleParser;
        }

        private Instruction[] GetListLiteralCreationInstructions(IEnumerable<Value> items)
        {
            var instructions = new List<Instruction>();
            instructions.Add(new Instruction(Opcode.ListEmpty));
            foreach (var expressionValue in items.Reverse())
            {
                Debug.Assert(expressionValue.Type == Types.Expression);
                instructions.AddRange(Heap.GetExpression((int)expressionValue.Raw).Instructions);
                instructions.Add(new Instruction(Opcode.ListAdd));
                expressionValue.RemoveRef();
            }
            return instructions.ToArray();
        }

        private static Value AllocateExpression(Types type, Instruction[] instructions)
        {
            var expressionValue = Heap.AllocateExpression(Types.Tuple, instructions);
            expressionValue.AddRef();
            return expressionValue;
        }

        private IEnumerable<char> Once(char c, bool condition = true)
        {
            if (condition) yield return c;
        }

        private class MainParser
        {
            public Parser<Value> Parser { get; private set; }
            public void SetParser(Parser<Value> parser) => Parser = parser;
        }
    }
}