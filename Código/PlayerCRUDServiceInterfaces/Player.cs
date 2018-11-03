using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerCRUDServiceInterfaces
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Avatar { get; set; }

        public Player()
        {
            Id = Guid.NewGuid();
        }
    }
}
