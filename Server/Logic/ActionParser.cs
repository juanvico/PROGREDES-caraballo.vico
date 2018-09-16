using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class ActionParser
    {
        private static Game GameLogic;

        public static void SetGameLogic(Game gameLogic)
        {
            GameLogic = gameLogic;
        }
        public static void Execute(Socket socket)
        {
            while (true)
            {
                string cmd = Transmitter.Receive(socket);

                if (cmd.Equals("newplayer"))
                {
                    string nick = Transmitter.Receive(socket);
                    string avatar = Transmitter.Receive(socket);
                    try
                    {
                        Player player = new Player()
                        {
                            Nickname = nick,
                            Avatar = avatar
                        };
                        Game.AddPlayer(player);
                        Transmitter.Send(socket, "Player registered.");
                    }
                    catch (NicknameInUseEx ex)
                    {
                        Transmitter.Send(socket, ex.Message);
                    }
                }
                if (cmd.Equals("exit"))
                {
                    IPEndPoint remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
                    Console.WriteLine("Cliente " + remoteIpEndPoint.Address + " cerrado.");
                    socket.Close();
                    break;
                }
            }
        }
    }
}
