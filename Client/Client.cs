﻿using System;
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
            var socket = new Socket(
               AddressFamily.InterNetwork,
               SocketType.Stream,
               ProtocolType.Tcp
               );
            Console.WriteLine("Ingrese puerto:");
            int n = Int32.Parse(Console.ReadLine());
            socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), n));
            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000));

            var t2 = new Thread(() => Transmitter.Receive(socket));
            t2.Start();
            var t1 = new Thread(() => ActionParser.Execute(socket));
            t1.Start();
        }
    }
}
