using System.IO;

namespace Garply
{
    public interface IOperand
    {
        void Write(BinaryWriter writer, IMetadataDatabase metadataDatabase);
    }
}
