using System;
using System.Net.Sockets;

namespace Logic
{
    public class GamePlayer
    {
        public Socket PlayerSocket { get; set; }
        public string Nickname { get; set; }
        public string Role { get; set; }
        public int Life { get; set; }
        public int Damage { get; set; }

        public bool IsAlive { get { return Life > 0; } }

        public Tuple<int,int> Spot { get; set; }

        public static GamePlayer Create (Socket playerSocket, string nickname)
        {
            GamePlayer gp = new GamePlayer()
            {
                PlayerSocket = playerSocket,
                Nickname = nickname,
                Spot = new Tuple<int, int>(-1, -1)
            };
            return gp;
        }

        public void AssignRole(string role)
        {
            this.Role = role;
            if (IsMonster())
            {
                Life = 100;
                Damage = 10;
            }
            else
            {
                Life = 20;
                Damage = 5;
            }
        }

        public void AssignSpot(Tuple<int, int> tuple)
        {
            Spot = tuple;
        }

        public void Attack(GamePlayer playerToAttack)
        {
            if (!(this.Role.Equals("survivor") && playerToAttack.Role.Equals("survivor")))
            {
                playerToAttack.ReceiveDamage(this.Damage);
            }
        }

        public void ReceiveDamage(int damage)
        {
            this.Life -= damage;
        }

        public bool IsMonster()
        {
            return Role.Equals("monster");
        }

        public bool IsSurvivor()
        {
            return Role.Equals("survivor");
        }

        public void Reset()
        {
            if (IsMonster())
            {
                Life = 100;
            }
            else
            {
                Life = 20;
            }
            Spot = new Tuple<int, int>(-1, -1);
        }
    }
}