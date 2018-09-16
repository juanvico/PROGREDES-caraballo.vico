using System;
using System.Runtime.Serialization;

namespace Logic
{
    [Serializable]
    class IncorrectRoleEx : Exception
    {
        override public string Message { get; }
        public IncorrectRoleEx()
        {
            Message = "Select MONSTER or SURVIVOR role.";
        }
    }
}