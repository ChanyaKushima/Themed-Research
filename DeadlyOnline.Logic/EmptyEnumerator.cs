using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DeadlyOnline.Logic
{
    public class EmptyEnumerator : IEnumerator
    {
        public static IEnumerator Instance => _instance ??= new EmptyEnumerator();

        private static EmptyEnumerator _instance;

        public object Current => throw new InvalidOperationException();

        public bool MoveNext() => false;
        public void Reset() { }
    }

    public class EmptyEnumerator<T> : IEnumerator<T>
    {
        public static IEnumerator<T> Instance => _instance ??= new EmptyEnumerator<T>();

        private static EmptyEnumerator<T> _instance;

        public T Current => throw new InvalidOperationException();

        object IEnumerator.Current => Current;

        public bool MoveNext() => false;
        public void Reset() { }

        public void Dispose() { }
    }
}
