namespace garply
{
    public interface IName : IFirstClassType
    {
        string Value { get; }
        IName ParentName { get; }
    }
}
