using System;
using System.Collections.Generic;
using System.Text;

namespace DeadlyOnline.Logic
{

    [Serializable]
    public class BadResponseException : Exception
    {
        public BadResponseException() { }
        public BadResponseException(string message) : base(message) { }
        public BadResponseException(string message, Exception inner) : base(message, inner) { }
        protected BadResponseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
