using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;

namespace ExchangeRates
{
    class Program
    {
        static void Main()
        {
            string baseAddress = "http://localhost:9000/";

            // Start OWIN host 
            using (WebApp.Start(url: baseAddress))
            {
                Console.WriteLine("Service Hosted " + baseAddress);
                System.Threading.Thread.Sleep(-1);
            }
        }
    }
}
