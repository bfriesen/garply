using System;

namespace Garply
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args == null || args.Length == 0) // REPL
            {
                Repl();
            }
            else
            {

            }
        }

        private static void Repl()
        {
            var executionContext = new ExecutionContext();
            var parser = new GarplyParser(executionContext);

            Console.Clear();
            Console.WriteLine(@"Welcome to the garply REPL. Type "":q"" to exit.");

            string line;
            while (true)
            {
                var strings = Heap.StringDump;
                var lists = Heap.ListDump;
                var tuples = Heap.TupleDump;

                var expressions = Heap.ExpressionDump(executionContext);

                var refCounts = Heap.RefCountDump;
                Console.Write("garply> ");
                switch (line = Console.ReadLine().Trim())
                {
                    case ":q": return;
                }
                var parseResult = parser.ParseLine(line);
                switch (parseResult.Type)
                {
                    case Types.Expression:
                    {
                        var value = Heap.GetExpression((int)parseResult.Raw).Evaluate(executionContext);
                        if (value.Type == Types.Error)
                        {
                            var error = executionContext.GetError();
                            Console.WriteLine(error.ToString());
                            Heap.DecrementTupleRefCount(error);
                        }
                        else Console.WriteLine(value.ToString());
                        Heap.DecrementRefCount(value);
                        Heap.DecrementExpressionRefCount(parseResult);
                        break;
                    }
                    case Types.Error:
                    {
                        var error = executionContext.GetError();
                        Console.WriteLine(error.ToString());
                        Heap.DecrementTupleRefCount(error);
                        break;
                    }
                    default: break;
                }
            }
        }
    }
}
