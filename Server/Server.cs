using Logic;
using Obligatorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("SERVER RUNNING");
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
                var t1 = new Thread(() => Transmitter.Receive(client));
                Transmitter.Send(client, "Cliente conectado.");
                t1.Start();
            }
        }


        
    }
}
