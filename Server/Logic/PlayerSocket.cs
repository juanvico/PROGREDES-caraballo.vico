using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class PlayerSocket
    {
        public string IP { get; set; }
        public string Port { get; set; }

        public static PlayerSocket Create(string ip, string port)
        {
            return new PlayerSocket()
            {
                IP = ip,
                Port = port
            };
        }
        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            PlayerSocket ps = (PlayerSocket)obj;
            if (this.IP==ps.IP && this.Port==ps.Port)
            {
                return true;
            }
            return false;
        }
    }
}
