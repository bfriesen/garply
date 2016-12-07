namespace Garply
{
    internal interface IErrorContext
    {
        void AddError(Error error);
        Value GetError();
    }
}
