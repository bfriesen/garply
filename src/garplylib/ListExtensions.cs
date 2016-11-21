namespace garply
{
    public static class ListExtensions
    {
        public static Integer Count(this List list)
        {
            var count = 0;
            while (!list.Head.Type.Equals(Types.Empty))
            {
                count++;
                list = list.Tail;
            }
            return new Integer(count);
        }
    }
}
