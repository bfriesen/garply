using System.Collections.Generic;

namespace Garply
{
    internal class ErrorContext : IErrorContext
    {
        private readonly Queue<Error> _errors = new Queue<Error>();

        public void AddError(Error error)
        {
            _errors.Enqueue(error);
        }

        public Value GetError()
        {
            Value errorTuple = default(Value);
            while (_errors.Count > 0)
            {
                var error = _errors.Dequeue();
                var errorMessage = Heap.AllocateString(error.Message, false);
                errorMessage.AddRef();
                errorTuple = Heap.AllocateTuple(errorMessage, errorTuple);
                errorTuple.AddRef();
            }
            return errorTuple;
        }
    }
}
