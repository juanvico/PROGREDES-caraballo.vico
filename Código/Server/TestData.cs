using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obligatorio
{
    public class TestData
    {
        public static void LoadTestData()
        {
            Game.AddPlayer(new Player() { Nickname = "1", Avatar = "default" });
            Game.AddPlayer(new Player() { Nickname = "2", Avatar = "default" });
            Game.AddPlayer(new Player() { Nickname = "3", Avatar = "default" });
            Game.AddPlayer(new Player() { Nickname = "4", Avatar = "default" });
            Game.AddPlayer(new Player() { Nickname = "5", Avatar = "default" });
            Game.AddPlayer(new Player() { Nickname = "6", Avatar = "default" });
            Game.AddPlayer(new Player() { Nickname = "7", Avatar = "default" });
            Game.AddPlayer(new Player() { Nickname = "8", Avatar = "default" });
        }
    }
}
