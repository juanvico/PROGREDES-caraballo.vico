using System;
using System.Runtime.Serialization;

namespace Logic
{
    [Serializable]
    class MaxNumberOfPlayers : Exception
    {
        override public string Message { get; }
        public MaxNumberOfPlayers()
        {
            Message = "The active game is full of participants. Try again later.";
        }
    }
}