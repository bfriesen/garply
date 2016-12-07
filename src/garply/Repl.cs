using System;

namespace Garply
{
    internal static class Repl
    {
        public static void Start()
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