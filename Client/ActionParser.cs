using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Client
{
    public static class ActionParser
    {
        public static void Execute(string cmd, Socket socket)
        {
            if (cmd.Equals("exit"))
            {
                Environment.Exit(0);
            }
            if (cmd.Equals("newPlayer"))
            {
                NewPlayerAction(socket);
            }

        }

        private static void NewPlayerAction(Socket socket)
        {
            bool[] requestedInfo = new bool[2];
            while (true)
            {
                if (requestedInfo[0] == false)
                {
                    Console.WriteLine("Insert nickname:");
                    string name = Console.ReadLine();
                    Transmitter.Send(socket, name);
                    requestedInfo[0] = true;
                }
                else
                {
                    Console.WriteLine("Insert avatar:");
                    string avatar = Console.ReadLine();
                    Transmitter.Send(socket, avatar);
                    requestedInfo[0] = false;
                    requestedInfo[1] = false;
                }

            }
        }
    }
}
