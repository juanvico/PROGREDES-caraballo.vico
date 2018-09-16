using System;
using System.Runtime.Serialization;

namespace Logic
{
    [Serializable]
    class ConnectedNicknameInUseEx : Exception
    {
        override public string Message { get; }
        public ConnectedNicknameInUseEx()
        {
            Message = "Already exists connected player with same nickname. Try again.";
        }
    }
}