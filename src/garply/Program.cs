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

            string line;
            while (true)
            {
                Console.Write("garply> ");
                switch (line = Console.ReadLine().Trim())
                {
                    case "": continue;
                    case ":q": return;
                    case ":c": Console.Clear(); continue;
                    case ":h":
                        Console.WriteLine();
                        Console.WriteLine(Heap.StringDump);
                        Console.WriteLine(Heap.ListDump);
                        Console.WriteLine(Heap.TupleDump);
                        Console.WriteLine(Heap.ExpressionDump(executionContext));
                        Console.WriteLine();
                        continue;
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
                            error.RemoveRef();
                        }
                        else Console.WriteLine(value.ToString());
                        value.RemoveRef();
                        parseResult.RemoveRef();
                        break;
                    }
                    case Types.Error:
                    {
                        var error = executionContext.GetError();
                        Console.WriteLine(error.ToString());
                        error.RemoveRef();
                        break;
                    }
                    default: break;
                }
            }
        }
    }
}
