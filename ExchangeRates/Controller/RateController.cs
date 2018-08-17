using ExchangeRates.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;

namespace ExchangeRates.Controller
{
    public class RateController : ApiController
    {
        public decimal Get(string currencypair)
        {
            try
            {
                return ExchangeRates.BusinessLayer.ExchangeRates.GetExchangeRate(currencypair);
            }
            catch (Exception)
            {
                return 1m;
            }
        }
        [CacheFilter(TimeDuration = 6000)]
        public IEnumerable<ExchangeRates.Model.ExchangeRatesModel> Get()
        {
            return ExchangeRates.BusinessLayer.ExchangeRates.GetAllExchangeRates();
        }
    }
}
