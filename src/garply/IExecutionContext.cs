namespace Garply
{
    internal interface IExecutionContext : IErrorContext
    {
        void Push(Value value);
        Value Pop();
        int Size { get; }
    }
}
