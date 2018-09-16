using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class Game
    {
        private static List<Player> Players;
        private static List<string> ConnectedPlayers;
        static readonly object objPlayers = new object();
        static readonly object objConnPlayers = new object();
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
            lock (objConnPlayers)
            {
                ConnectedPlayers.Add(nick);
            }
        }
    }
}