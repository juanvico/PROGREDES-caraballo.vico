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
            var startAcceptingClients = new Thread(() => AcceptClients(server));
            var executeServerCommands = new Thread(() => ExecuteCommands());
            startAcceptingClients.Start();
            executeServerCommands.Start();




        }

        public static void AcceptClients(Socket server)
        {
            while (true)
            {
                var client = server.Accept();
                var clientThread = new Thread(() => ActionParser.Execute(client));
                Transmitter.Send(client, "Cliente conectado.");
                clientThread.Start();

            }

        }
        public static void ExecuteCommands()
        {
            while (true)
            {
                var command = Console.ReadLine();
                ActionParser.ExecuteCommand(command);
            }
                
        }
    }
}
