using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Windows;
using System.Windows.Media;

namespace Games.Object.Documents
{
    public abstract class Block
    {
        public abstract void Draw(DrawingContext drawingContext);
    }

    public class BlockCollection : IEnumerable<Block>, IList<Block>
    {
        private readonly List<Block> _blocks;

        public Block this[int index]
        {
            get => _blocks[index];
            set => _blocks[index] = value;
        }

        public int Count => _blocks.Count;
        public int Capacity => _blocks.Capacity;

        public bool IsReadOnly => false;

        public void Add(Block item) => _blocks.Add(item);
        public void Clear() => _blocks.Clear();
        public bool Contains(Block item) => _blocks.Contains(item);
        public void CopyTo(Block[] array, int arrayIndex) => _blocks.CopyTo(array, arrayIndex);
        public int IndexOf(Block item) => _blocks.IndexOf(item);
        public void Insert(int index, Block item) => _blocks.Insert(index, item);
        public void InsertRange(int index, IEnumerable<Block> collection) => _blocks.InsertRange(index, collection);
        public bool Remove(Block item) => _blocks.Remove(item);
        public void RemoveAt(int index) => _blocks.RemoveAt(index);
        public void RemoveAll(Predicate<Block> match) => _blocks.RemoveAll(match);
        public IEnumerator<Block> GetEnumerator() => _blocks.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public BlockCollection()
        {
            _blocks = new List<Block>();
        }
    }
}
