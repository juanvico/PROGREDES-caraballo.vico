using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    class Client
    {
        private static IPAddress serverIP;
        private static int serverPort;
        private static IPAddress clientIP;
        private static int clientPort;
        static void Main(string[] args)
        {
            try
            {
                SetSocketData();
                var socket = new Socket(
                   AddressFamily.InterNetwork,
                   SocketType.Stream,
                   ProtocolType.Tcp
                   );
                socket.Bind(new IPEndPoint(clientIP, clientPort));
                socket.Connect(new IPEndPoint(serverIP, serverPort));
                var t2 = new Thread(() => Transmitter.Receive(socket));
                t2.Start();
                var t1 = new Thread(() => ActionParser.Execute(socket));
                t1.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }

        private static void SetSocketData()
        {
            AskIPAndPort("client");
            AskIPAndPort("server");
        }

        private static void AskIPAndPort(string subject)
        {
            bool isIPValid = false;
            bool isPortValid = false;
            while (!isIPValid)
            {
                if (subject.Equals("client"))
                {
                    Console.WriteLine("Write your IP: (localhost: 127.0.0.1)");
                    isIPValid = IPAddress.TryParse(Console.ReadLine(), out clientIP);
                }
                else
                {
                    Console.WriteLine("Write Server IP: (localhost: 127.0.0.1)");
                    isIPValid = IPAddress.TryParse(Console.ReadLine(), out serverIP);
                }
            }
            while (!isPortValid)
            {
                if (subject.Equals("client"))
                {
                    Console.WriteLine("Write your port:");
                    isPortValid = Int32.TryParse(Console.ReadLine(), out clientPort);
                }
                else
                {
                    Console.WriteLine("Write Server port:");
                    isPortValid = Int32.TryParse(Console.ReadLine(), out serverPort);
                }
            }
        }
    }
}
