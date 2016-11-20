using System.IO;

namespace garply
{
    public interface IOperand
    {
        void Write(BinaryWriter writer, IMetadataDatabase metadataDatabase);
    }
}
