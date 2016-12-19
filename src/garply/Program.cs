using Garply.Repl;

namespace Garply
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                ReplEngine.Start();
            }
            else
            {

            }
        }
    }
}
