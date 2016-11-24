using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class List : IEnumerable<Value>
    {
        private List()
        {
            Head = Value.Error;
            Tail = this;
            Count = 0;
        }

        private List(Value head, List tail)
        {
            Head = head;
            Tail = tail;
            Count = tail.Count + 1;
        }

        public static List Empty { get; } = new List();

        public List Add(Value item)
        {
            Debug.Assert(item.Type != Types.Error);
            return new List(item, this);
        }

        public Value Head { get; }
        public List Tail { get; }
        public int Count { get; }

        public void Write(BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            var id = metadataDatabase.GetListId(this);
            id.Write(writer, metadataDatabase);
        }

        IEnumerator<Value> IEnumerable<Value>.GetEnumerator() => new ListEnumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new ListEnumerator(this);

        private class ListEnumerator : IEnumerator<Value>
        {
            private List _list;

            public ListEnumerator(List list)
            {
                // The enumerator needs to start out in a before-the-first-item
                // state, so add an Error value to the front of the list.
                _list = new List(Value.Error, list);
            }

            Value IEnumerator<Value>.Current => _list.Head;
            object IEnumerator.Current => _list.Head;
            void IEnumerator.Reset() { }
            void IDisposable.Dispose() { }

            bool IEnumerator.MoveNext()
            {
                _list = _list.Tail;
                return _list.Count > 0;
            }
        }

        internal string DebuggerDisplay => $"list({Count})";
    }
}
