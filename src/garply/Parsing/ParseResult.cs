//using Garply;

//namespace Garply
//{
//    internal class ParseResult
//    {
//        private ParseResult(ParseResultType type, Expression expression = default(Expression))
//        {
//            Type = type;
//            Expression = expression;
//        }

//        public static ParseResult Whitespace { get; } = new ParseResult(ParseResultType.Whitespace);

//        public static ParseResult Literal(Value value)
//        {
//            return new ParseResult(ParseResultType.Expression, new Expression(new[] { new Instruction(Opcode.Load, value) }));
//        }

//        public ParseResultType Type { get; }
//        public Expression Expression { get; }
//    }
//}