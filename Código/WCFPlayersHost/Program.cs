using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFPlayersHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var serviceHost = new ServiceHost(typeof(PlayerCRUDService.PlayerCRUDService)))
            {
                Console.WriteLine("Starting service...");
                serviceHost.Open();
                Console.WriteLine("Service is running, press return to sstop");
                Console.ReadLine();
            }
        }
    }
}
