using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class Game
    {
        public static void Welcome(Socket client)
        {
            Console.WriteLine("Nuevo jugador conectado.");
        }
    }
}