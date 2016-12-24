using System;

namespace Garply.Repl
{
    internal static class ReplEngine
    {
        public static void Start()
        {
            var consoleReader = new ConsoleReader();

            var scopeBuilder = new Scope.Builder();
            var executionContext = new ExecutionContext(scopeBuilder.Build());
            var parser = new GarplyParser(executionContext, scopeBuilder);

            Console.Clear();

            while (true)
            {
                Console.Write("garply>");
                string line;
                switch (line = consoleReader.ReadLine())
                {
                    case "": continue;
                    case ":q": return;
                    case ":c": Console.Clear(); continue;
                    case ":d":
                        Console.WriteLine();
                        Console.WriteLine(Heap.Dump);
                        Console.WriteLine();
                        Console.WriteLine(executionContext.Scope);
                        Console.WriteLine();
                        continue;
                }
                var parseResult = parser.ParseLine(line);
                if (scopeBuilder.Size > executionContext.Scope.Size)
                {
                    var newScope = executionContext.Scope.Copy(scopeBuilder);
                    executionContext.Scope.Delete();
                    executionContext.Scope = newScope;
                }
                switch (parseResult.Type)
                {
                    case Types.expression:
                        {
                            var expression = Heap.GetExpression((int)parseResult.Raw);
                            var value = expression.Evaluate(executionContext);
                            if (value.Type == Types.error)
                            {
                                var error = executionContext.TakeErrors();
                                Console.WriteLine(error.ToString());
                                error.RemoveRef();
                            }
                            else Console.WriteLine(value.ToString());
                            value.RemoveRef();
                            parseResult.RemoveRef();
                            break;
                        }
                    case Types.error:
                        {
                            var error = executionContext.TakeErrors();
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