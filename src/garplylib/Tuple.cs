using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace Garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Tuple : IEnumerable<Value>
    {
        private readonly Value[] _items;
        
        public Tuple(Integer arity, IExecutionContext context)
        {
            Debug.Assert(arity != null);
            Debug.Assert(context != null);
            _items = new Value[arity.Value];
            for (int i = 0; i < arity.Value; i++)
            {
                var item = context.Pop();
                Debug.Assert(item.Type != Types.Error);
                _items[i] = item;
            }
            Arity = arity;
        }

        public Tuple(Value[] items)
        {
            Debug.Assert(items != null);
            _items = items;
            Arity = new Integer(items.Length);
        }

        public Integer Arity { get; }

        public Value GetItem(Integer index)
        {
            Debug.Assert(index.Value < _items.Length && index.Value <= int.MaxValue);
            return _items[(int)index.Value];
        }

        public void Write(Opcode opcode, BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            var id = metadataDatabase.GetTupleId(this);
            id.Write(opcode, writer, metadataDatabase);
        }

        internal string DebuggerDisplay => $"tuple({Arity.Value})";

        IEnumerator<Value> IEnumerable<Value>.GetEnumerator() => ((IList<Value>)_items).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
