using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogServiceInterfaces
{
    public class Match
    {
        public List<PlayerStats> PlayerStats { get; set; }
        public DateTime Date { get; set; }

        public Match()
        {
            Date = DateTime.Now;
            PlayerStats = new List<PlayerStats>();
        }
    }
}
