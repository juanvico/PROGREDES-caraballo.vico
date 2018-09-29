using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
                else if (cmd.Equals("newplayer"))
                {
                    Transmitter.Send(socket, "newplayer");
                    NewPlayerAction(socket);
                }
                else if (cmd.Equals("connect"))
                {
                    Transmitter.Send(socket, "connect");
                    ConnectPlayerToGame(socket);
                }
                else if (cmd.Equals("enter"))
                {
                    Transmitter.Send(socket, "enter");
                    TryEnterGame(socket);
                }
                else if (cmd.Equals("attack"))
                {
                    Transmitter.Send(socket, "attack");
                }
                else if (cmd.StartsWith("move"))
                {
                    Transmitter.Send(socket, cmd);
                }
                else
                {
                    Console.WriteLine(Utils.GetClientAvailableCmds());
                }
            }
        }

        private static void TryEnterGame(Socket socket)
        {
            bool validRole = false;
            string role = "";
            while (!validRole)
            {
                Console.WriteLine("Choose your role (MONSTER or SURVIVOR):");
                role = Console.ReadLine();
                role = Utils.ToLwr(role);
                if (role.Equals("monster") || role.Equals("survivor"))
                {
                    validRole = true;
                }
            }
            
            Transmitter.Send(socket, role);
        }

        private static void ConnectPlayerToGame(Socket socket)
        {
            Console.WriteLine("Insert nickname:");
            string nickname = Console.ReadLine();
            Transmitter.Send(socket, nickname);
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
                    Console.WriteLine("Insert avatar path:");
                    string avatar = Console.ReadLine();
                    Transmitter.Send(socket, avatar);
                    break;
                }
            }
        }
    }
}