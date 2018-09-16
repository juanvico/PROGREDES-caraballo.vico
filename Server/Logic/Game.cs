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
        private static List<string> ConnectedPlayers;
        private static bool StartedGame;
        private static Timer GameTimer;
        static readonly object objPlayers = new object();
        static readonly object objConnPlayers = new object();

        const int THREE_MINUTES = 1000 * 60 * 3;

        public Game()
        {
            Players = new List<Player>();
            ConnectedPlayers = new List<string>();

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

        public static void ConnectPlayer(string nick)
        {

            if (ConnectedPlayers.Contains(nick))
            {
                throw new ConnectedNicknameInUseEx();
            }
            else if (!Players.Exists(pl=>pl.Nickname==nick))
            {
                throw new NotExistingPlayer();
            }
            else if (!StartedGame)
            lock (objConnPlayers)
            {
                ConnectedPlayers.Add(nick);
            }
        }

        public static void StartGame()
        {
            StartedGame = true;
            GameTimer = new Timer(THREE_MINUTES);
            GameTimer.Elapsed += EndGameByTimer;
            GameTimer.AutoReset = false;
            GameTimer.Enabled = true;
        }

        private static void EndGameByTimer(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Game finished by timmer. jaja");
        }

        public static bool ISGameInProcess()
        {
            return StartedGame;
        }
    }
}