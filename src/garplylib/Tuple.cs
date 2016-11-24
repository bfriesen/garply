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
        private readonly Items _items = new Items();
        
        public Tuple(Integer arity, IExecutionContext context)
        {
            for (int i = 0; i < arity.Value; i++)
            {
                var item = context.Pop();
                Debug.Assert(item.Type != Types.Error);
                _items.Add(item);
            }
#if UNSTABLE
            _items.Lock();
#endif
            Arity = arity;
        }

        public Tuple(Value[] items)
        {
            Debug.Assert(items != null);
            foreach (var item in items)
            {
                Debug.Assert(item.Type != Types.Error);
                _items.Add(item);
            }
#if UNSTABLE
            _items.Lock();
#endif
            Arity = new Integer(items.Length);
        }

        public Tuple(Value[] items, Name[] names)
        {
            Debug.Assert(items != null);
            Debug.Assert(names == null);
            Debug.Assert(items.Length == names.Length);
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var name = names[i];
                Debug.Assert(item.Type != Types.Error);
                Debug.Assert(name != null);
                _items.SetCurrentName(name);
                _items.Add(item);
            }
#if UNSTABLE
            _items.Lock();
#endif
            Arity = new Integer(items.Length);
        }

        public Integer Arity { get; }

        public Value GetItem(Integer index)
        {
            Debug.Assert(index.Value < _items.Count && index.Value <= int.MaxValue);
            return _items[(int)index.Value];
        }

        public Value GetItem(Name name)
        {
#if UNSTABLE
            try
            {
#endif
                return _items[name];
#if UNSTABLE
            }
            catch (KeyNotFoundException)
            {
                Debug.Fail("Key not found");
                throw;
            }
#endif
        }

        public void Write(BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            var id = metadataDatabase.GetTupleId(this);
            id.Write(writer, metadataDatabase);
        }

        private class Items : KeyedCollection<Name, Value>
        {
            private Name _currentName;

            public void SetCurrentName(Name name)
            {
                _currentName = name;
            }

            protected override Name GetKeyForItem(Value item)
            {
                var name = _currentName;
                _currentName = null;
                return name;
            }

#if UNSTABLE
            private bool _isLocked;

            public void Lock()
            {
                _isLocked = true;
            }

            protected override void InsertItem(int index, Value item)
            {
                if (_isLocked) throw new InvalidOperationException("Cannot insert item in locked Items collection.");
                base.InsertItem(index, item);
            }

            protected override void SetItem(int index, Value item)
            {
                if (_isLocked) throw new InvalidOperationException("Cannot set item in locked Items collection.");
                base.SetItem(index, item);
            }

            protected override void ClearItems()
            {
                if (_isLocked) throw new InvalidOperationException("Cannot clear items in locked Items collection.");
                base.ClearItems();
            }

            protected override void RemoveItem(int index)
            {
                if (_isLocked) throw new InvalidOperationException("Cannot remove item in locked Items collection.");
                base.RemoveItem(index);
            }
#endif
        }

        internal string DebuggerDisplay => $"tuple({Arity.Value})";

        IEnumerator<Value> IEnumerable<Value>.GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
