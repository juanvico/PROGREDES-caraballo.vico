using System;
using System.Runtime.Serialization;

namespace Logic
{
    [Serializable]
    class NicknameInUseEx : Exception
    {
        override public string Message { get; }
        public NicknameInUseEx()
        {
            Message = "Already exists player with same nickname.";
        }
    }
}