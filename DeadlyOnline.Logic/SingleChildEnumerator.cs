using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DeadlyOnline.Logic
{
    class SingleChildEnumerator : IEnumerator
    {
        private bool _hasNext;
        private object _child;

        public SingleChildEnumerator(object child)
        {
            _child = child;
            _hasNext = _child != null;
        }
        public object Current => _hasNext ? _child : null;
        public bool MoveNext()
        {
            bool hasNext = _hasNext;
            _hasNext = false;
            return hasNext;
        }

        public void Reset() => _hasNext = true;
    }
}
