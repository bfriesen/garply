using System.IO;

namespace garply
{
    public interface IOperand
    {
        void Write(Stream stream);
    }
}
