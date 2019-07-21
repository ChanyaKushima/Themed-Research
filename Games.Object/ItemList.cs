using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Games.Object
{
    [DebuggerDisplay("Count = {Count}")]
    public class ItemList<T> : IList<T>, IReadOnlyList<T>
    {
        private const int MaxArrayLength = 0x7FEFFFFF;
        private const int DefaultCapacity = 4;

        private readonly static T[] _emptyArray = new T[0];

        private T[] _items;
        private int _size;
        private int _version;
        internal Func<int, int> CapacityAdder = c => c * 2;

        public ItemList()
        {
            _items = _emptyArray;
        }

        public ItemList(Func<int, int> capacityAdder) : this()
        {
            CapacityAdder = capacityAdder;
        }

        public ItemList(int capacity)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }

            _items = (capacity == 0) ? _emptyArray : new T[capacity];
        }

        public ItemList(int capacity, Func<int, int> capacityAdder) : this(capacity)
        {
            CapacityAdder = capacityAdder;
        }

        public ItemList(IEnumerable<T> collection)
        {
            if (collection is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(collection));
            }

            if (collection is ICollection<T> c)
            {
                int count = c.Count;
                if (count == 0)
                {
                    _items = _emptyArray;
                }
                else
                {
                    _items = new T[count];
                    c.CopyTo(_items, 0);
                    _size = count;
                }
            }
            else
            {
                _size = 0;
                _items = _emptyArray;

                using IEnumerator<T> en = collection.GetEnumerator();
                while (en.MoveNext())
                {
                    Add(en.Current);
                }
            }
        }

        public int Capacity
        {
            get { return _items.Length; }
            set
            {
                if (value < _size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException();
                }

                if (value != _items.Length && value != 0)
                {
                    T[] newItems = new T[value];
                    if (_size > 0)
                    {
                        Array.Copy(_items, 0, newItems, 0, _size);
                    }
                    _items = newItems;
                    _version++;
                }
            }
        }

        public int Count => _size;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException();
                }
                return _items[index];
            }
            set
            {
                if (index >= _size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException();
                }
                _items[index] = value;
                _version++;
            }
        }

        public void Add(T item)
        {
            if (_items.Length == _size)
            {
                EnsureCapacity(_size + 1);
            }
            _items[_size++] = item;
            _version++;
        }
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection is null)
            {
                ThrowHelper.ThrowArgumentNullException();
            }

            if (collection is ICollection<T> c)
            {
                int count = c.Count;
                if (count != 0)
                {
                    c.CopyTo(_items, _size);
                    _size += count;
                }
            }
            else
            {
                using IEnumerator<T> en = collection.GetEnumerator();
                while (en.MoveNext())
                {
                    Add(en.Current);
                }
            }
        }

        private void EnsureCapacity(int min)
        {
            int newCapacity = _items.Length == 0 ? DefaultCapacity : CapacityAdder(_items.Length);
            if ((uint)newCapacity > MaxArrayLength)
            {
                newCapacity = MaxArrayLength;
            }
            else if (newCapacity < min)
            {
                newCapacity = min;
            }
            Capacity = newCapacity;
        }

        public void Clear()
        {
            if (_size > 0)
            {
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                {
                    Array.Clear(_items, 0, _size);
                }
                _size = 0;
            }
            _version++;
        }
        public bool Contains(T item)
        {
            if (item is null)
            {
                for (int i = 0; i < _size; i++)
                {
                    if (_items[i] is null) { return true; }
                }
                return false;
            }
            else
            {
                for (int i = 0; i < _size; i++)
                {
                    EqualityComparer<T> c = EqualityComparer<T>.Default;
                    if (c.Equals(_items[i], item)) { return true; }
                }
                return false;
            }
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex, _size);
        }
        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            if (array is null)
            {
                ThrowHelper.ThrowArgumentNullException();
            }
            if (count > array.Length - arrayIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }

            for (int i = 0; i < count; i++)
            {
                array[i + arrayIndex] = _items[i];
            }
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _size);
        }
        public int IndexOf(T item, int index)
        {
            if (index >= _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }
            return Array.IndexOf(_items, item, 0, _size);
        }

        public void Insert(int index, T item)
        {
            int size = _size;

            if ((uint)index > (uint)size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }
            if (size == _items.Length)
            {
                EnsureCapacity(size + 1);
            }
            if (index < size)
            {
                Array.Copy(_items, index, _items, index + 1, size - index);
            }
            _items[index] = item;
            _size++;
            _version++;
        }
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            int size = _size;

            if (collection is null)
            {
                ThrowHelper.ThrowArgumentNullException();
            }
            if ((uint)index > (uint)size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }

            if (collection is ICollection<T> c)
            {
                int count = c.Count;
                if (count > 0)
                {
                    EnsureCapacity(size + count);
                    if (index < size)
                    {
                        Array.Copy(_items, index, _items, index + count, size - index);
                    }

                    if (this == c)
                    {
                        Array.Copy(_items, 0, _items, index, index);
                        Array.Copy(_items, index + count, _items, index * 2, size - index);
                    }
                    else
                    {
                        c.CopyTo(_items, index);
                    }
                    _size += count;
                }
            }
            else
            {
                using IEnumerator<T> en = collection.GetEnumerator();
                while (en.MoveNext())
                {
                    Insert(index++, en.Current);
                }
            }
            _version++;
        }
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)_size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }
            _size--;

            if (index < _size)
            {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }
            if (index == _size)
            {
                _items[_size] = default;
            }
            _version++;
        }

        public void RemoveRange(int index, int count)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowIndexOutOfRangeException();
            }

            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }

            if (_size - index < count)
            {
                ThrowHelper.ThrowArgumentException();
            }

            if (count > 0)
            {
                _size -= count;
                if (index < _size)
                {
                    Array.Copy(_items, index + count, _items, index, _size - index);
                }

                _version++;
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                {
                    Array.Clear(_items, _size, count);
                }
            }
        }

        public IEnumerator<T> GetEnumerator() => new List<T>(_items).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region AsSpan methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => new Span<T>(_items, 0, _size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan(Index startIndex)
        {
            int actualIndex = startIndex.GetOffset(_size);
            if (actualIndex > _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }

            return new Span<T>(_items, actualIndex, _size - actualIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan(int start)
        {
            if (start > _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }

            return new Span<T>(_items, start, _size - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan(Range range)
        {
            (int start, int length) = range.GetOffsetAndLength(_size);
            return new Span<T>(_items, start, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan(int start, int length) => new Span<T>(_items, start, length);

        #endregion
    }
}
