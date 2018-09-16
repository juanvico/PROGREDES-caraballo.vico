using System;
using System.Runtime.Serialization;

namespace Logic
{
    [Serializable]
    class NicknameInUseEx : Exception
    {
        public NicknameInUseEx()
        {
        }

        public NicknameInUseEx(string message) : base(message)
        {
        }

        public NicknameInUseEx(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NicknameInUseEx(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}