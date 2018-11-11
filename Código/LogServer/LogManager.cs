using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogServer
{
    class LogManager : MarshalByRefObject
    {
        private List<string> Log;

        public void NewLog()
        {
            Log = new List<string>();
        }
        public void SendLogEntry(string message)
        {

        }
    }
}
