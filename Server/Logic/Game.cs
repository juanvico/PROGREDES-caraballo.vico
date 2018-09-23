using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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

        public static bool IsAlive(Socket socket)
        {
            string nickname = GetNicknameBySocket(socket);
            return GamePlayers[nickname].IsAlive;
        }

        static readonly object objMatrix = new object();

        const int THREE_MINUTES = 1000 * 60 * 3;

        public Game()
        {
            Players = new List<Player>();
            Party = new List<GamePlayer>();
            SetMatchHelpers();
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
                gp.Attack(playerToAttack);
                string msgToAttacker, msgToAttacked;
                if (playerToAttack.IsAlive)
                {
                    msgToAttacker = "You damaged " + gp.Damage + " life points to " +
                    playerToAttack.Nickname + " ( " + playerToAttack.Life + " life remaining )";
                    msgToAttacked = gp.Nickname + " damaged you with " + gp.Damage + " life points. You got "
                        + playerToAttack.Life + " remaining life points.";
                }
                else
                {
                    msgToAttacker = "You killed " + playerToAttack.Nickname + ".";
                    msgToAttacked = gp.Nickname + " killed you.";
                    RemoveDeadPlayer(playerToAttack);
                }
                Transmitter.Send(gp.PlayerSocket, msgToAttacker);
                Transmitter.Send(playerToAttack.PlayerSocket, msgToAttacked);
            }
            EndMatch();
            InspectCloserPlayers(nickname);
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
                string msg = winner.Nickname + " (" + winner.Role + ") won the game.";
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
                    string msg = "Survivors won.";
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
                    string msg = "Survivors won.";
                    Transmitter.Send(connectedPlayer.PlayerSocket, msg);
                }
                Game.ResetMatch();
            }
            else if (aliveMonsters > 1 && aliveSurvivors == 0)
            {
                foreach (GamePlayer connectedPlayer in Party)
                {
                    string msg = "Nobody won.";
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
            Matrix[playerToAttack.Spot.Item1, playerToAttack.Spot.Item2] = null;
        }

        private static List<GamePlayer> GetCloserPlayers(GamePlayer gp)
        {
            List<GamePlayer> closerPlayers = new List<GamePlayer>();

            for (int auxI = -1; auxI <= 1; auxI++)
            {
                for (int auxJ = -1; auxJ <= 1; auxJ++)
                {
                    int x = gp.Spot.Item1 + auxI;
                    int y = gp.Spot.Item2 + auxJ;
                    if (ValidIndex(x, y))
                    {
                        if (!IsSamePlayer(x, y, gp.Spot.Item1, gp.Spot.Item2))
                        {
                            if (!IsEmptySpot(x, y))
                            {
                                GamePlayer gpToInspect = Matrix[x, y];
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
            if (!role.ToLower().Equals("monster") && !role.ToLower().Equals("survivor"))
            {
                throw new IncorrectRoleEx();
            }
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
                gp.AssignSpot(playerSpot);
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
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Matrix[i, j] == null)
                    {
                        return new Tuple<int, int>(i, j);
                    }
                }
            }
            return new Tuple<int, int>(-1, -1);
        }

        public static void InspectCloserPlayers(string nickname)
        {
            if (IsActiveMatch())
            {
                GamePlayer gp = GamePlayers[nickname];
                string msg = "Your position is [ " + (gp.Spot.Item1 + 1) + " ][ " + (gp.Spot.Item2 + 1) + " ]";
                Transmitter.Send(gp.PlayerSocket, msg);
                for (int auxI = -1; auxI <= 1; auxI++)
                {
                    for (int auxJ = -1; auxJ <= 1; auxJ++)
                    {
                        int x = gp.Spot.Item1 + auxI;
                        int y = gp.Spot.Item2 + auxJ;
                        if (ValidIndex(x, y))
                        {
                            if (!IsSamePlayer(x, y, gp.Spot.Item1, gp.Spot.Item2))
                            {
                                if (!IsEmptySpot(x, y))
                                {
                                    GamePlayer gpToInspect = Matrix[x, y];
                                    msg = "At [ " + (x + 1) + " ][ " + (y + 1) + " ] there is a " + gpToInspect.Role
                                        + " ( " + gpToInspect.Nickname + " ) with " + gpToInspect.Life + " remaining life points.";
                                    Transmitter.Send(gp.PlayerSocket, msg);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsEmptySpot(int gpi, int gpj)
        {
            return Matrix[gpi, gpj] == null;
        }

        private static bool IsSamePlayer(int x, int y, int i, int j)
        {
            return x == i && y == j;
        }

        private static bool ValidIndex(int x, int y)
        {
            bool isXValid = x >= 0 && x <= 7;
            bool isYValid = y >= 0 && y <= 7;
            return isXValid && isYValid;
        }
    }
}
