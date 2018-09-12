using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Obligatorio
{
    class Server
    {
        static void Main(string[] args)
        {
            var server = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
                );
            server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000));
            server.Listen(25);
            while (true)
            {
                var client = server.Accept();
                var thread = new Thread(() => Game.Welcome(client));
                thread.Start();

            }
        }
    }
}
