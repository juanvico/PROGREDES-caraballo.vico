using System;
using System.ServiceModel;

namespace LogServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var serviceHost = new ServiceHost(typeof(LogService.LogService)))
            {
                Console.WriteLine("Starting service...");
                serviceHost.Open();
                Console.WriteLine("Service is running, press return to stop");
                Console.ReadLine();
            }
        }
    }
}
