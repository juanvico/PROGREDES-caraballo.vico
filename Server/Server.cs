using Logic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Server
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("SERVER RUNNING");
            Game GameLogic = new Game();
            ActionParser.SetGameLogic(GameLogic);
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
                var t1 = new Thread(() => ActionParser.Execute(client));
                Transmitter.Send(client, "Cliente conectado.");
                t1.Start();
            }
        }
    }
}
