namespace Garply
{
    public interface IMetadataDatabase
    {
        long GetStringId(string value);
        Value LoadString(long id);
    }
}
