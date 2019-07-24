using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DeadlyOnline.Logic
{
    class EmptyEnumerator : IEnumerator
    {
        public static IEnumerator Instance => _instance ??= new EmptyEnumerator();

        private static EmptyEnumerator _instance;

        public object Current => throw new InvalidOperationException();

        public bool MoveNext() => false;
        public void Reset() { }
    }
}
