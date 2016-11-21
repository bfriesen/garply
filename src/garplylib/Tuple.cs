using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Tuple : IFirstClassType, IEnumerable
    {
        private readonly Items _items = new Items();
        
        private Tuple(IType type)
        {
            Arity = new Integer(0);
            Type = type;
        }

        public Tuple(Integer arity, IExecutionContext context)
        {
            for (int i = 0; i < arity.Value; i++)
            {
                _items.Add(context.Pop());
            }
#if UNSTABLE
            _items.Lock();
#endif
            Arity = arity;
            Type = arity.Value == 0 ? Types.Empty : Types.Tuple;
        }

        public Tuple(IFirstClassType[] items)
        {
#if UNSTABLE
            if (items == null) throw new ArgumentNullException("items");
#endif
            foreach (var item in items)
            {
#if UNSTABLE
                if (item == null) throw new ArgumentException("The 'items' parameter must not have any null values.", "items");
#endif
                _items.Add(item);
            }
#if UNSTABLE
            _items.Lock();
#endif
            Arity = new Integer(items.Length);
            Type = items.Length == 0 ? Types.Empty: Types.Tuple;
        }

        public Tuple(IFirstClassType[] items, Name[] names)
            : this(items, names, null)
        {
        }

        private Tuple(IFirstClassType[] items, Name[] names, IType type)
        {
#if UNSTABLE
            if (items == null) throw new ArgumentNullException("items");
            if (names == null) throw new ArgumentNullException("names");
            if (items.Length != names.Length) throw new ArgumentException("The 'names' parameter must have the same number of values as the 'items' parameter.", "names");
#endif
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var name = names[i];
#if UNSTABLE
                if (item == null) throw new ArgumentException("The 'items' parameter must not have any null values.", "items");
                if (name == null) throw new ArgumentException("The 'names' parameter must not have any null values.", "names");
#endif
                _items.SetCurrentName(name);
                _items.Add(item);
            }
#if UNSTABLE
            _items.Lock();
#endif
            Arity = new Integer(items.Length);
            Type = type ?? (items.Length == 0 ? Types.Empty: Types.Tuple);
        }

        public static Tuple Empty { get; } = new Tuple(Types.Empty);
        public static Tuple Error { get; } = new Tuple(Types.Error);

        public IType Type { get; }
        public Integer Arity { get; }

        public IFirstClassType GetItem(Integer index)
        {
#if UNSTABLE
            if (index.Value >= _items.Count || index.Value > int.MaxValue) throw new ArgumentOutOfRangeException("index");
#endif
            return _items[(int)index.Value];
        }

        public IFirstClassType GetItem(Name name)
        {
#if UNSTABLE
            try
            {
#endif
                return _items[name];
#if UNSTABLE
            }
            catch (KeyNotFoundException ex)
            {
                throw new ArgumentOutOfRangeException("name", ex);
            }
#endif
        }

        private class Items : KeyedCollection<Name, IFirstClassType>
        {
            private Name _currentName;

            public void SetCurrentName(Name name)
            {
                _currentName = name;
            }

            protected override Name GetKeyForItem(IFirstClassType item)
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

            protected override void InsertItem(int index, IFirstClassType item)
            {
                if (_isLocked) throw new InvalidOperationException("Cannot insert item in locked Items collection.");
                base.InsertItem(index, item);
            }

            protected override void SetItem(int index, IFirstClassType item)
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

        internal string DebuggerDisplay
        {
            get
            {
                if (Type.Equals(Types.Empty))
                {
                    return "empty<tuple>";
                }
                else if (Type.Equals(Types.Error))
                {
                    return "error<tuple>";
                }
                else
                {
                    return $"tuple({Arity.Value})";
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
