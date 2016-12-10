using System.Collections.Generic;

namespace Garply
{
    internal class ErrorContext
    {
        private readonly Queue<Error> _errors = new Queue<Error>();
        public void AddError(Error error) => _errors.Enqueue(error);

        public Value TakeErrors()
        {
            Value errorList = Empty.List;
            while (_errors.Count > 0)
            {
                var error = _errors.Dequeue();
                var errorMessage = Heap.AllocateString(error.Message);
                var errorTuple = Heap.AllocateTuple(errorMessage);
                errorList = Heap.AllocateList(errorTuple, errorList);
                errorMessage.AddRef();
                errorTuple.AddRef();
                errorList.AddRef();
            }
            return errorList;
        }
    }
}
