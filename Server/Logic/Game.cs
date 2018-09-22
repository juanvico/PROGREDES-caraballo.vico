using System;
using System.Collections.Generic;
using System.Linq;
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
        private static List<GamePlayer> GamePlayers;

        private static bool ActiveMatch;
        private static Timer GameTimer;
        static readonly object objPlayers = new object();
        static readonly object objParty = new object();
        static readonly object objGamePlayers = new object();

        const int THREE_MINUTES = 1000 * 60 * 3;

        public Game()
        {
            Players = new List<Player>();
            Party = new List<GamePlayer>();
            GamePlayers = new List<GamePlayer>();
        }

        public static void AddPlayer(Player p)
        {
            if (Players.Exists(pl=>pl.Nickname==p.Nickname))
            {
                throw new NicknameInUseEx();
            }
            lock (objPlayers)
            {
                Players.Add(p);
            }
        }

        public static void ConnectPlayerToParty(GamePlayer gp)
        {

            if (Party.Exists(p => p.Nickname==gp.Nickname))
            {
                throw new ConnectedNicknameInUseEx();
            }
            else if (!Players.Exists(pl=>pl.Nickname==gp.Nickname))
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
            Console.WriteLine("Game finished by timmer. jaja");
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
        }

        public static string GetNicknameBySocket(string ip, string port)
        {
            return Party.Find(gp => gp.PlayerSocket.Equals(PlayerSocket.Create(ip, port))).Nickname;
        }

        public static void AddPlayerToMatch(string nickname)
        {
            GamePlayer gp = Party.Find(p => p.Nickname == nickname);
            lock (objGamePlayers)
            {
                GamePlayers.Add(gp);
            }
        }
    }
}