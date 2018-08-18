using System;
using System.Collections.Generic;

namespace ExchangeRates.Model
{
    public class ExchangeRatesModel
    {
        ExchangeRates.BusinessLayer.ExchangeRatesLogic exchangeRatesInstance = ExchangeRates.BusinessLayer.ExchangeRatesLogic.Instance;

        #region Property
        public string Currency { get; set; }
        public decimal Rate { get; set; }
        #endregion

        #region Methods
        public List<ExchangeRatesModel> GetAllExchangeRates()
        {
            try
            {
                return exchangeRatesInstance.GetAllExchangeRates();
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        //Get Exchange Rate for the provided Currency Pair
        public decimal GetExchangeRate(string currencyPair)
        {
            try
            {
                return exchangeRatesInstance.GetExchangeRate(currencyPair);
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
