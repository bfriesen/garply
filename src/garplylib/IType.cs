namespace garply
{
    public interface IType : IFirstClassType
    {
        IName Name { get; }
        IType BaseType { get; }
        Boolean Is(IType other);
    }
}
