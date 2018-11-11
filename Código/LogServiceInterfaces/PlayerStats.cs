using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogServiceInterfaces
{
    public class PlayerStats
    {
        public string Nickname { get; set; }
        public string Role { get; set; }
        public int KillScore { get; set; }
        public DateTime Date { get; set; }

        public bool IsWinner { get; set; }
        public bool IsAlive { get; set; }

        public PlayerStats()
        {
            Date = DateTime.Now;
            KillScore = 0;
            IsWinner = false;
            IsAlive = true;
        }

        public bool IsMonster()
        {
            return Role.Equals("monster");
        }

        public bool IsSurvivor()
        {
            return Role.Equals("survivor");
        }
    }
}
