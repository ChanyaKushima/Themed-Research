using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Games.Object
{
    [DebuggerDisplay("Count = {Count}")]
    internal class ItemList<T> : IList<T>, IReadOnlyList<T>
    {
        private T[] _items;
        private int _size;
        private int _version;

        private readonly static T[] _emptyArray = new T[0];

        public ItemList()
        {
            _items = _emptyArray;
        }

        public ItemList(int capacity)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }

            _items = (capacity == 0) ? _emptyArray : new T[capacity];
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
            get
            {
                return _items.Length;
            }
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

        private void EnsureCapacity(int min) => throw new NotImplementedException();
        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size);
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
        public void CopyTo(T[] array, int arrayIndex, int length)
        {
            if (array is null)
            {
                ThrowHelper.ThrowArgumentNullException();
            }
            if (length > array.Length - arrayIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }

            for (int i = 0; i < length; i++)
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
            throw new NotImplementedException();
        }

        public void RemoveAt(int index) => throw new NotImplementedException();
        public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

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
    }
}
