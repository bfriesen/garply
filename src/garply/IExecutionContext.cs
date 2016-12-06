namespace Garply
{
    public interface IExecutionContext : IErrorContext
    {
        void Push(Value value);
        Value Pop();
        int Size { get; }
    }
}
