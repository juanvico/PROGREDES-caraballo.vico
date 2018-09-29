using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Utils
    {
        public static string ToLwr(string cmd)
        {
            return cmd.ToLower();
        }

        public static string GetClientAvailableCmds()
        {
            return "Available commands: <newplayer> - <connect> - <enter> - <exit>";
        }
    }
}
