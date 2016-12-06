namespace Garply
{
    public interface IErrorContext
    {
        void AddError(Error error);
        Value GetError();
    }
}
