using System.Collections.Generic;

namespace Garply
{
    internal class ExecutionContext : IExecutionContext
    {
        private readonly Stack<Value> _evaluationStack = new Stack<Value>();

        public ExecutionContext() : this(new ErrorContext())
        {
        }

        public ExecutionContext(IErrorContext errorContext)
        {
            ErrorContext = errorContext;
        }

        public IErrorContext ErrorContext { get; }

        public Value Pop()
        {
            return _evaluationStack.Pop();
        }

        public void Push(Value value)
        {
            _evaluationStack.Push(value);
        }

        public int Size => _evaluationStack.Count;

        public void AddError(Error error)
        {
            ErrorContext.AddError(error);
        }

        public Value GetError()
        {
            return ErrorContext.GetError();
        }
    }
}
