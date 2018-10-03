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
        public static void Execute(Socket socket)
        {
            bool loop = true;
            while (loop)
            {
                string command = Transmitter.Receive(socket);
                var cmd = Utils.ToLwr(command);

                if (Game.IsDeadPlayer(socket))
                {
                    Transmitter.Send(socket, Utils.GetWaitMessageForLoser());
                }
                else if (!Game.IsCurrentlyPlayingMatch(socket))
                {
                    loop = LetExecuteClientAction(socket, cmd);
                }
                else if (Game.IsCurrentlyPlayingMatch(socket))
                {
                    LetExecuteGameAction(socket, cmd);
                }
            }
        }

        private static bool LetExecuteClientAction(Socket socket, string cmd)
        {
            if (cmd.Equals("newplayer"))
            {
                string nick = Transmitter.Receive(socket);
                string avatar = Transmitter.Receive(socket);
                try
                {
                    Player player = new Player
                    {
                        Nickname = nick
                    };
                    if (!avatar.Equals("default"))
                    {
                        player.Avatar = GetTimestamp(DateTime.Now) + avatar;
                        SaveImage(socket, player.Avatar);
                    }
                    else { player.Avatar = avatar; }
                    Game.AddPlayer(player);
                    NotifyClientRegisteredPlayer(socket);
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
                    Game.CheckIfPlayerIsConnected(nickname);
                    Game.AssignRole(role, nickname);
                    Game.AddPlayerToMatch(nickname);
                    Transmitter.Send(socket, "Logged in to match correctely. Start to play.");
                }
                catch (NotActiveMatch ex)
                {
                    Transmitter.Send(socket, ex.Message);
                }
                catch (MaxNumberOfPlayers ex)
                {
                    Transmitter.Send(socket, ex.Message);
                }
                catch (NotConnectedPlayerEx ex)
                {
                    Transmitter.Send(socket, ex.Message);
                }
            }
            else if (cmd.Equals("exit"))
            {
                Game.ExitClient(socket);
                IPEndPoint remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
                Console.WriteLine("Cliente " + remoteIpEndPoint.Address + ":" + remoteIpEndPoint.Port + " cerrado.");
                socket.Close();
                return false;
            }
            return true;
        }

        private static void LetExecuteGameAction(Socket socket, string cmd)
        {
            if (cmd.Equals("attack"))
            {
                Game.Attack(socket);
            }
            else if (cmd.StartsWith("move "))
            {
                try
                {
                    Utils.CheckMovementCmd(cmd);
                    Game.Move(socket, cmd);
                }
                catch (IncorrectMoveCmdEx ex)
                {
                    Transmitter.Send(socket, ex.Message);
                }
                catch (ExistsPlayerForMoveEx ex)
                {
                    Transmitter.Send(socket, ex.Message);
                }
            }
            else
            {
                Transmitter.Send(socket, Utils.GetMatchAvailableCmds());
            }
        }

        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        private static void NotifyClientRegisteredPlayer(Socket socket)
        {
            Transmitter.Separator(socket);
            Transmitter.Send(socket, "Player registered.");
            Transmitter.Separator(socket);
        }

        private static void SaveImage(Socket socket, string nickname)
        {
            ImageWriter.SetImageLocation(nickname);
            while (true)
            {
                byte[] fragment = Transmitter.ReceiveImage(socket);
                ImageWriter.WriteFragment(fragment);
                if (fragment.Length < ImageWriter.fragmentSize) break;
            }
            ImageWriter.CloseFile();
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
                Console.WriteLine("GAME STARTED!");
            }
            else if (cmd.Equals("registeredplayers"))
            {
                Game.ListAllRegisteredPlayers();
            }
            else if (cmd.Equals("connectedplayers"))
            {
                Game.ListAllConnectedPlayers();
            }
            else
            {
                Console.WriteLine(Utils.GetServerAvailableCmds());
            }
        }
    }
}
