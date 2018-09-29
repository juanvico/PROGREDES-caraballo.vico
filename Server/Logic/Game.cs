using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static Logic.GamePlayer;

namespace Logic
{
    public class Game
    {
        private static List<Player> Players;
        private static List<GamePlayer> Party;
        private static int CurrentPlayersNumber;
        private static GamePlayer[,] Matrix;
        private static Dictionary<string, GamePlayer> GamePlayers;

        private static bool ActiveMatch;
        private static Timer GameTimer;
        static readonly object objPlayers = new object();
        static readonly object objParty = new object();
        static readonly object objGamePlayers = new object();
        static readonly object objAttack = new object();

        public static bool IsAlive(Socket socket)
        {
            string nickname = GetNicknameBySocket(socket);
            return GamePlayers[nickname].IsAlive;
        }

        static readonly object objMatrix = new object();

        const int THREE_MINUTES = 1000 * 60 * 3;
       
        public static void InitGame()
        {
            Players = new List<Player>();
            Party = new List<GamePlayer>();
            SetMatchHelpers();
        }

        public static void Move(Socket socket, string cmd)
        {
            GamePlayer gp = GamePlayers[GetNicknameBySocket(socket)];
            PlayerSpot possibleSpot = Utils.Move(gp.Spot, cmd);
            if (!ValidIndex(possibleSpot.Row, possibleSpot.Column))
            {
                throw new IncorrectMoveCmdEx();
            }
            if (!IsEmptySpot(possibleSpot.Row, possibleSpot.Column))
            {
                throw new ExistsPlayerForMoveEx();
            }
            lock (objMatrix)
            {
                Matrix[gp.Spot.Row, gp.Spot.Column] = null;
                Matrix[possibleSpot.Row, possibleSpot.Column] = gp;
                gp.Spot = possibleSpot;
            }
            InspectCloserPlayers(gp.Nickname);
        }

        private static void SetMatchHelpers()
        {
            GamePlayers = new Dictionary<string, GamePlayer>();
            CurrentPlayersNumber = 0;
            Matrix = new GamePlayer[8, 8];
            ActiveMatch = false;
        }

        public static void AddPlayer(Player p)
        {
            if (Players.Exists(pl => pl.Nickname == p.Nickname))
            {
                throw new NicknameInUseEx();
            }
            lock (objPlayers)
            {
                Players.Add(p);
            }
        }

        public static void Attack(Socket socket)
        {
            string nickname = GetNicknameBySocket(socket);
            GamePlayer gp = GamePlayers[nickname];

            List<GamePlayer> closerPlayers = GetCloserPlayers(gp);

            foreach (GamePlayer playerToAttack in closerPlayers)
            {
                if (AreNotSurvivors(gp, playerToAttack))
                {
                    lock (objAttack)
                    {
                        gp.Attack(playerToAttack);
                    }
                    string msgToAttacker, msgToAttacked;
                    if (playerToAttack.IsAlive)
                    {
                        msgToAttacker = Utils.GetAttackerDamageStatus(gp, playerToAttack);
                        msgToAttacked = Utils.GetAttackedDamageStatus(gp, playerToAttack);
                    }
                    else
                    {
                        msgToAttacker = Utils.GetAttackerKillStatus(playerToAttack);
                        msgToAttacked = Utils.GetAttackedKillStatus(gp);
                        RemoveDeadPlayer(playerToAttack);
                    }
                    Transmitter.Send(gp.PlayerSocket, msgToAttacker);
                    Transmitter.Send(playerToAttack.PlayerSocket, msgToAttacked);
                }
            }
            EndMatch();
            InspectCloserPlayers(nickname);
        }

        private static bool AreNotSurvivors(GamePlayer gp, GamePlayer playerToAttack)
        {
            return !(gp.IsSurvivor() && playerToAttack.IsSurvivor());
        }

        private static void EndMatch()
        {
            int aliveMonsters = 0, aliveSurvivors = 0;
            foreach (GamePlayer gp in GamePlayers.Values)
            {
                if (gp.IsMonster() && gp.IsAlive)
                {
                    aliveMonsters++;
                }
                else if (gp.IsSurvivor() && gp.IsAlive)
                {
                    aliveSurvivors++;
                }
            }
            if (aliveMonsters == 1 && aliveSurvivors == 0)
            {
                GamePlayer winner = GamePlayers.Values.First();
                string msg = Utils.GetWinnerMessage(winner);
                foreach (GamePlayer connectedPlayer in Party)
                {
                    Transmitter.Send(connectedPlayer.PlayerSocket, msg);
                }
                Game.ResetMatch();
            }
            else if (aliveMonsters == 0 && aliveSurvivors > 0)
            {
                foreach (GamePlayer connectedPlayer in Party)
                {
                    string msg = Utils.GetSurvivorWinMessage();
                    Transmitter.Send(connectedPlayer.PlayerSocket, msg);
                }
                Game.ResetMatch();
            }
        }

        public static void EndMatchByTimer()
        {
            int aliveMonsters = 0, aliveSurvivors = 0;
            foreach (GamePlayer gp in GamePlayers.Values)
            {
                if (gp.IsMonster())
                {
                    aliveMonsters++;
                }
                else if (gp.IsSurvivor())
                {
                    aliveSurvivors++;
                }
            }
            if (aliveSurvivors > 0)
            {
                foreach (GamePlayer connectedPlayer in Party)
                {
                    string msg = Utils.GetSurvivorWinMessage();
                    Transmitter.Send(connectedPlayer.PlayerSocket, msg);
                }
                Game.ResetMatch();
            }
            else if (aliveMonsters > 1 && aliveSurvivors == 0)
            {
                foreach (GamePlayer connectedPlayer in Party)
                {
                    string msg = Utils.GetNoWinnersMessage();
                    Transmitter.Send(connectedPlayer.PlayerSocket, msg);
                }
                Game.ResetMatch();
            }
        }

        private static void ResetMatch()
        {
            foreach (GamePlayer gp in GamePlayers.Values)
            {
                gp.Reset();
            }
            SetMatchHelpers();
        }

        private static void RemoveDeadPlayer(GamePlayer playerToAttack)
        {
            lock (objMatrix)
            {
                Matrix[playerToAttack.Spot.Row, playerToAttack.Spot.Column] = null;
            }
        }

        private static List<GamePlayer> GetCloserPlayers(GamePlayer gp)
        {
            List<GamePlayer> closerPlayers = new List<GamePlayer>();

            for (int auxRow = -1; auxRow <= 1; auxRow++)
            {
                for (int auxColumn = -1; auxColumn <= 1; auxColumn++)
                {
                    int row = gp.Spot.Row + auxRow;
                    int column = gp.Spot.Column + auxColumn;
                    if (ValidIndex(row, column))
                    {
                        if (!IsSamePlayer(row, column, gp.Spot.Row, gp.Spot.Column))
                        {
                            if (!IsEmptySpot(row, column))
                            {
                                GamePlayer gpToInspect = Matrix[row, column];
                                closerPlayers.Add(gpToInspect);
                            }
                        }
                    }
                }
            }
            return closerPlayers;
        }

        public static void ConnectPlayerToParty(GamePlayer gp)
        {

            if (Party.Exists(p => p.Nickname == gp.Nickname))
            {
                throw new ConnectedNicknameInUseEx();
            }
            else if (!Players.Exists(pl => pl.Nickname == gp.Nickname))
            {
                throw new NotExistingPlayer();
            }
            lock (objParty)
            {
                Party.Add(gp);
            }
        }

        public static void StartGame()
        {
            ActiveMatch = true;
            GameTimer = new Timer(THREE_MINUTES);
            GameTimer.Elapsed += EndGameByTimer;
            GameTimer.AutoReset = false;
            GameTimer.Enabled = true;
        }

        private static void EndGameByTimer(Object source, ElapsedEventArgs e)
        {
            EndMatchByTimer();
        }

        public static bool IsActiveMatch()
        {
            return ActiveMatch;
        }

        public static void AssignRole(string role, string nickname)
        {
            GamePlayer gp = Party.Find(p => p.Nickname == nickname);
            gp.AssignRole(role);
        }

        public static void TryEnter()
        {
            if (!IsActiveMatch())
            {
                throw new NotActiveMatch();
            }
            else if (IsActiveMatch() && CurrentPlayersNumber > 64)
            {
                throw new MaxNumberOfPlayers();
            }
        }

        public static string GetNicknameBySocket(Socket socket1)
        {
            foreach (GamePlayer gp in Party)
            {
                if (EqualsSocket(gp.PlayerSocket, socket1))
                {
                    return gp.Nickname;
                }
            }
            return "";
        }

        public static bool EqualsSocket(Socket socket1, Socket Socket2)
        {
            string ip1 = ((IPEndPoint)socket1.RemoteEndPoint).Address.ToString();
            string port1 = ((IPEndPoint)socket1.RemoteEndPoint).Port.ToString();

            string ip2 = ((IPEndPoint)Socket2.RemoteEndPoint).Address.ToString();
            string port2 = ((IPEndPoint)Socket2.RemoteEndPoint).Port.ToString();

            return ip1 == ip2 && port1 == port2;
        }

        public static void AddPlayerToMatch(string nickname)
        {
            GamePlayer gp = Party.Find(p => p.Nickname == nickname);
            lock (objGamePlayers)
            {
                Tuple<int, int> playerSpot = AssignPlayerSpot();
                gp.AssignSpot(playerSpot.Item1, playerSpot.Item2);
                Matrix[playerSpot.Item1, playerSpot.Item2] = gp;
                GamePlayers.Add(gp.Nickname, gp);
                CurrentPlayersNumber++;
            }
            InspectCloserPlayers(nickname);
        }

        public static bool IsCurrentlyPlayingMatch(Socket socket)
        {
            List<GamePlayer> players = GamePlayers.Values.ToList();
            foreach (GamePlayer gp in players)
            {
                if (EqualsSocket(socket, gp.PlayerSocket))
                {
                    return true;
                }
            }
            return false;
        }

        public static Tuple<int, int> AssignPlayerSpot()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    if (Matrix[row, column] == null)
                    {
                        return Tuple.Create(row, column);
                    }
                }
            }
            return Tuple.Create(-1, -1);
        }

        public static void InspectCloserPlayers(string nickname)
        {
            if (IsActiveMatch())
            {
                GamePlayer gp = GamePlayers[nickname];
                Utils.ShowPlayerStatus(gp);

                List<GamePlayer> closerPlayers = GetCloserPlayers(gp);
                foreach (GamePlayer gpToInspect in closerPlayers)
                {
                    Utils.ShowCloserPlayerStatus(gp, gpToInspect);
                }
            }
        }

        private static bool IsEmptySpot(int row, int column)
        {
            return Matrix[row, column] == null;
        }

        private static bool IsSamePlayer(int row1, int column1, int row2, int column2)
        {
            return row1 == row2 && column1 == column2;
        }

        private static bool ValidIndex(int row, int column)
        {
            bool isRowValid = row >= 0 && row <= 7;
            bool isColumnValid = column >= 0 && column <= 7;
            return isRowValid && isColumnValid;
        }
    }
}
