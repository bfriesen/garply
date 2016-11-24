namespace Garply
{
    public interface IMetadataDatabase
    {
        void RegisterString(String value);
        Integer GetStringId(String value);
        String LoadString(Integer id);
        void RegisterTuple(Tuple value);
        Integer GetTupleId(Tuple value);
        Tuple LoadTuple(Integer id);
        void RegisterList(List value);
        Integer GetListId(List value);
        List LoadList(Integer id);
    }
}
