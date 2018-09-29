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
            Game.AddPlayer(new Player() { Nickname = "1", Avatar = "1" });
            Game.AddPlayer(new Player() { Nickname = "2", Avatar = "2" });
            Game.AddPlayer(new Player() { Nickname = "3", Avatar = "3" });
            Game.AddPlayer(new Player() { Nickname = "4", Avatar = "4" });
        }
    }
}
