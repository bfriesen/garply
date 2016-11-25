using System.IO;

namespace Garply
{
    public interface IOperand
    {
        void Write(Opcode opcode, BinaryWriter writer, IMetadataDatabase metadataDatabase);
    }
}
