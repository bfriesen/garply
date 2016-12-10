using System.Collections.Generic;

namespace Garply
{
    internal class ExecutionContext : ErrorContext
    {
        public ExecutionContext(Scope scope)
        {
            Scope = scope;
        }

        private readonly Stack<Value> _evaluationStack = new Stack<Value>();
        public Value Pop() => _evaluationStack.Pop();
        public void Push(Value value) => _evaluationStack.Push(value);
        public int Size => _evaluationStack.Count;
        public Scope Scope { get; set; }
    }
}
