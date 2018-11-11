using PlayerCRUDServiceInterfaces;

namespace GameServer
{
    public class TestData
    {
        public static void LoadTestData()
        {
            using (var players = new CRUDService.PlayerCRUDServiceClient())
            {
                players.Add(new Player() { Nickname = "1", Avatar = "default" });
                players.Add(new Player() { Nickname = "2", Avatar = "default" });
                players.Add(new Player() { Nickname = "3", Avatar = "default" });
                players.Add(new Player() { Nickname = "4", Avatar = "default" });
                players.Add(new Player() { Nickname = "5", Avatar = "default" });
                players.Add(new Player() { Nickname = "6", Avatar = "default" });
                players.Add(new Player() { Nickname = "7", Avatar = "default" });
                players.Add(new Player() { Nickname = "8", Avatar = "default" });
            }
        }
    }
}
