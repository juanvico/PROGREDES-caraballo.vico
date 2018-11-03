using System;
using System.Runtime.Serialization;

namespace Business
{
    [Serializable]
    public class NotConnectedPlayerEx : Exception
    {
        override public string Message { get; }
        public NotConnectedPlayerEx()
        {
            Message = "First run command <connect> in order to enter to the active match.";
        }
    }
}