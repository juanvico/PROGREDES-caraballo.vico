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
                else
                {
                    Transmitter.Send(socket, "Incorrect command");
                }
            }
        }

        private static void TryEnterGame(Socket socket)
        {
            Console.WriteLine("Choose your role (MONSTER or SURVIVOR):");
            string role = Console.ReadLine();
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
                    Console.WriteLine("Insert avatar:");
                    string avatar = Console.ReadLine();
                    Transmitter.Send(socket, avatar);
                    break;
                }
            }
        }
    }
}