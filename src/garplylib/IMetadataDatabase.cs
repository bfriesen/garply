namespace garply
{
    public interface IMetadataDatabase
    {
        Integer GetStringId(String value);
        Integer GetTypeId(Type value);
        String LoadString(Integer id);
        Type LoadType(Integer id);
    }
}
