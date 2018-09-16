namespace Logic
{
    public class GamePlayer
    {
        public PlayerSocket PlayerSocket { get; set; }
        public string Nickname { get; set; }
        public string Role { get; set; }
        public int Life { get; set; }
        public int Damage { get; set; }

        public static GamePlayer Create (string ip, string port, string nickname)
        {
            PlayerSocket ps = PlayerSocket.Create(ip, port);

            GamePlayer gp = new GamePlayer()
            {
                PlayerSocket = ps,
                Nickname = nickname
            };

            return gp;
        }

        public void AssignRole(string role)
        {
            this.Role = role;
            if (this.Role.Equals("monster"))
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
    }
}