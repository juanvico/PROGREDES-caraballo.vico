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
        private List<Player> Players;
        public Game()
        {
            Players = new List<Player>();
        }

        public void AddPlayer(Player p)
        {
            if (Players.Exists(pl=>pl.Nickname==p.Nickname))
            {
                throw new NicknameInUseEx();
            }
            Players.Add(p);
        }

    }
}