using Microsoft.Owin.Hosting;
using System;
using System.Configuration;

namespace ExchangeRates
{
    class Program
    {
        static void Main()
        {      
            string baseAddress = ConfigurationManager.AppSettings["BaseURL"];
            // Start OWIN host 
            using (WebApp.Start(url: baseAddress))
            {
                Console.WriteLine("Service Hosted On Server: " + baseAddress);
                System.Threading.Thread.Sleep(-1);
            }
        }
    }
}
