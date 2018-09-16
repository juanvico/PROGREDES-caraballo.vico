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
        public static void Execute(Socket socket)
        {
            while (true)
            {
                string cmd = Console.ReadLine();

                if (cmd.Equals("exit"))
                {
                    Transmitter.Send(socket, "exit");
                    Environment.Exit(0);
                }
                if (cmd.Equals("newplayer"))
                {
                    Transmitter.Send(socket, "newplayer");
                    NewPlayerAction(socket);
                }
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
                    break;
                }
            }
        }
    }
}
