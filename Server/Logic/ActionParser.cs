using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public static class ActionParser
    {
        public static bool Execute(string cmd, Socket socket)
        {
            if (cmd.Equals("newplayer"))
            {

            }
            if (cmd.Equals("exit"))
            {
                IPEndPoint remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
                Console.WriteLine("Cliente " + remoteIpEndPoint.Address + " cerrado.");
                socket.Close();
                return true;
            }
            return false;
        }
    }
}
