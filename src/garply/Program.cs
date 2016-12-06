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

            while (true)
            {
                Console.Write("garply> ");
                string line;
                switch (line = Console.ReadLine().Trim())
                {
                    case "": continue;
                    case ":q": return;
                    case ":c": Console.Clear(); continue;
                    case ":d":
                        Console.WriteLine();
                        Console.WriteLine(Heap.Dump);
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
