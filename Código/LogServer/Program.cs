﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace LogServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var remotingTcpChannel = new TcpChannel(7000);

            ChannelServices.RegisterChannel(remotingTcpChannel, false);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(LogManager),
                "LogManager",
                WellKnownObjectMode.SingleCall);

            Console.WriteLine("Server has started at: tcp://127.0.0.1/LogManager");
            Console.ReadLine();

            ChannelServices.UnregisterChannel(remotingTcpChannel);
        }
    }
}
