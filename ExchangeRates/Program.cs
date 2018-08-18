using ExchangeRates.Common;
using Microsoft.Owin.Hosting;
using System;
using System.Configuration;

namespace ExchangeRates
{
    class Program
    {
        static void Main()
        {      
            string baseAddress = ConfigurationManager.AppSettings["BaseURL"].ToString();
            // Start OWIN host 
            using (WebApp.Start(url: baseAddress))
            {
                Console.WriteLine("Service Hosted " + baseAddress);
                System.Threading.Thread.Sleep(-1);
            }
        }
    }
}
