using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Garply
{
    internal class ExecutionContext : ErrorContext
    {
        private readonly Stack<Value> _evaluationStack = new Stack<Value>();
        private Scope _scope;

        public ExecutionContext(Scope scope)
        {
            Debug.Assert(scope != null);
            _scope = scope;
        }

        public Value Pop() => _evaluationStack.Pop();
        public void Push(Value value) => _evaluationStack.Push(value);
        public int Size => _evaluationStack.Count;

        public Scope Scope
        {
            get { return _scope; }
            set { Debug.Assert(value != null); _scope = value; }
        }
    }
}
