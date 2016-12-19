using System;

namespace Garply.Repl
{
    internal interface ICompletionEngine
    {
        ConsoleKeyInfo Trigger { get; }
        string[] GetCompletions(string partial);
        char[] GetTokenDelimiters();
    }
}
