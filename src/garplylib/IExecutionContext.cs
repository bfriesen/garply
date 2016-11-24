namespace Garply
{
    public interface IExecutionContext
    {
        void Push(Value value);
        Value Pop();
    }
}
