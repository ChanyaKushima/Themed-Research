using System;
using System.Runtime.Serialization;


namespace DeadlyOnline.Server
{
    [Serializable]
    public class LoginFailedException : Exception
    {
        public LoginFailedException() { }
        public LoginFailedException(string message) : base(message) { }
        public LoginFailedException(string message, Exception inner) : base(message, inner) { }
        protected LoginFailedException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}
