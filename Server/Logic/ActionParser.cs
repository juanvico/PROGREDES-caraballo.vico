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
                string command = Transmitter.Receive(socket);
                var cmd = Utils.ToLwr(command);

                if (Game.IsCurrentlyPlayingMatch(socket))
                {
                    if (Game.IsAlive(socket))
                    {
                        if (cmd.Equals("attack"))
                        {
                            Game.Attack(socket);
                        }
                        else if (cmd.Equals("move"))
                        {

                        }
                        else
                        {
                            Transmitter.Send(socket, "ATTACK or MOVE [][]");
                        }
                    }
                    else
                    {
                        Transmitter.Send(socket, "You lost this match. Wait until it finishes.");
                    }
                }
                else
                {
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
                    else if (cmd.Equals("connect"))
                    {
                        string nick = Transmitter.Receive(socket);
                        try
                        {
                            Game.ConnectPlayerToParty(GamePlayer.Create(socket, nick));
                            Transmitter.Send(socket, "Player connected to the game.");
                        }
                        catch (ConnectedNicknameInUseEx ex)
                        {
                            Transmitter.Send(socket, ex.Message);
                        }
                        catch (NotExistingPlayer ex)
                        {
                            Transmitter.Send(socket, ex.Message);
                        }
                    }
                    else if (cmd.Equals("enter"))
                    {
                        string role = Transmitter.Receive(socket);
                        try
                        {
                            Game.TryEnter();
                            string nickname = Game.GetNicknameBySocket(socket);
                            Game.AssignRole(role, nickname);
                            Game.AddPlayerToMatch(nickname);
                            Transmitter.Send(socket, "Logged in to match correctely. Start to play.");
                        }
                        catch (NotActiveMatch ex)
                        {
                            Transmitter.Send(socket, ex.Message);
                        }
                        catch (IncorrectRoleEx ex)
                        {
                            Transmitter.Send(socket, ex.Message);
                        }
                        catch (MaxNumberOfPlayers ex)
                        {
                            Transmitter.Send(socket, ex.Message);
                        }
                    }
                    else if (cmd.Equals("exit"))
                    {
                        IPEndPoint remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
                        Console.WriteLine("Cliente " + remoteIpEndPoint.Address + " cerrado.");
                        socket.Close();
                        break;
                    }
                }
            }
        }

        public static void ExecuteCommand(string cmd)
        {
            var command = Utils.ToLwr(cmd);

            if (Game.IsActiveMatch())
            {
                Console.WriteLine("Can't execute other commands while game in process.");
            }
            else if (command.Equals("startgame"))
            {
                Game.StartGame();
                Console.WriteLine("Game started !!!");
            }
        }
    }
}
