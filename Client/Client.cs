using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            var client = new Socket(
               AddressFamily.InterNetwork,
               SocketType.Stream,
               ProtocolType.Tcp
               );
            Console.WriteLine("Ingrese puerto:");
            int n = Int32.Parse(Console.ReadLine());
            client.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), n));
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000));
            Console.WriteLine("Conectado al servidor.");

            while (true)
            {
                string cmd = Console.ReadLine();

            }
            
        }
    }
}
