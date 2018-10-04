using System;

namespace Logic
{
    [Serializable]
    class NotActiveMatch : Exception
    {
        override public string Message { get; }
        public NotActiveMatch()
        {
            Message = "There is not any active match in process. Try again later.";
        }
    }
}