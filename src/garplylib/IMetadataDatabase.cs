namespace garply
{
    public interface IMetadataDatabase
    {
        Integer GetStringId(String value);
        String LoadString(Integer id);
    }
}
