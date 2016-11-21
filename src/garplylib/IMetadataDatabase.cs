namespace garply
{
    public interface IMetadataDatabase
    {
        void RegisterString(String value);
        void RegisterType(Type value);

        Integer GetStringId(String value);
        Integer GetTypeId(Type value);
        String LoadString(Integer id);
        Type LoadType(Integer id);
    }
}
