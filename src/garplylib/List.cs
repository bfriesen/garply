using System.Text;

namespace Garply
{
    public struct List
    {
        public readonly Value Head;
        public readonly int TailIndex;

        internal List(Value head, int tailIndex)
        {
            Head = head;
            TailIndex = tailIndex;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('[');
            var first = true;
            List list = this;
            while (true)
            {
                if (first) first = false;
                else sb.Append(',');
                sb.Append(list.Head.ToString());
                if (list.TailIndex == 0) break;
                list = Heap.GetList(list.TailIndex);
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
