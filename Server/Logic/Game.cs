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
        static readonly object obj = new object();
        public Game()
        {
            Players = new List<Player>();
        }

        public static void AddPlayer(Player p)
        {
            if (Players.Exists(pl=>pl.Nickname==p.Nickname))
            {
                throw new NicknameInUseEx();
            }
            lock (obj)
            {
                Players.Add(p);
            }
        }

    }
}