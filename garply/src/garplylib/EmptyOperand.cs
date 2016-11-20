using System.IO;

namespace garply
{
    public struct EmptyOperand : IOperand
    {
        public void Write(Stream stream)
        {
        }
    }
}
