using System;
using System.Runtime.Serialization;

namespace Logic
{
    [Serializable]
    class NotExistingPlayer : Exception
    {
        override public string Message { get; }
        public NotExistingPlayer()
        {
            Message = "Inexistent player with specified nickname. Try again.";
        }
    }
}