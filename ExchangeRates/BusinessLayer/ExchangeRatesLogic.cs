using ExchangeRates.Common;
using ExchangeRates.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Xml;

namespace ExchangeRates.BusinessLayer
{
    public sealed class ExchangeRatesLogic
    {
        static ExchangeRatesLogic _exchangeRatesInstance;   
        public static ExchangeRatesLogic Instance
        {
            get { return _exchangeRatesInstance ?? (_exchangeRatesInstance = new ExchangeRatesLogic()); }
        }
        private ExchangeRatesLogic() { }

        private const string _cacheKey = "GetAllExchangeRates";

        private List<ExchangeRatesModel> allExchangeRates = new List<ExchangeRatesModel>();

        public List<ExchangeRatesModel> GetAllExchangeRates()
        {
            using (XmlReader xmlr = XmlReader.Create(ConfigurationManager.AppSettings["XMLPath"].ToString()))
            {
                xmlr.ReadToFollowing("Cube");
                while (xmlr.Read())
                {
                    if (xmlr.NodeType != XmlNodeType.Element) continue;

                    if (xmlr.GetAttribute("time") == null)
                    {
                        ExchangeRatesModel model = new ExchangeRatesModel();
                        model.Currency = xmlr.GetAttribute("currency");
                        model.Rate = decimal.Parse(xmlr.GetAttribute("rate"), CultureInfo.InvariantCulture);
                        allExchangeRates.Add(model);
                    }                                       
                }
            }

            //// Add Reference Currency -- EUR in List<ExchangeRatesModel>
            //ExchangeRatesModel refModel = new ExchangeRatesModel();
            //refModel.Currency = "EUR";
            //refModel.Rate = 1m;
            //allExchangeRates.Add(refModel);
              return allExchangeRates;
        }

        public decimal GetExchangeRate(string CurrencyPair)
        {
            try
            {
                #region Caching
                ObjectCache cache = MemoryCache.Default;
                if (cache.Contains(_cacheKey))
                    allExchangeRates = (List<ExchangeRatesModel>)cache.Get(_cacheKey);
                else
                {
                    try
                    {
                        allExchangeRates = GetAllExchangeRates();
                    }
                    catch(Exception ex)
                    {
                        Logger.Error(ex.Message);
                        CustomException exception = new CustomException(System.Net.HttpStatusCode.InternalServerError, "Error occured while reading XML file");
                        throw exception;
                    }

                    // Storing data in the cache    
                    CacheItemPolicy cacheItemPolicy = new CacheItemPolicy
                    {
                        // Caching Set to 1 Hour
                        AbsoluteExpiration = DateTime.Now.AddHours(1.0)
                    };
                    cache.Add(_cacheKey, allExchangeRates, cacheItemPolicy);
                }
                #endregion

                string refCurrency = ConfigurationManager.AppSettings["RefCurrency"].ToString(); //EUR
                decimal rateFrom = 0m;
                decimal rateTo = 0m;
                string currencyFrom = string.Empty;
                string currencyTo = string.Empty;

                if (CurrencyPair.Length > 6)
                {
                    CustomException exception = new CustomException(System.Net.HttpStatusCode.BadRequest, "Invalid Currency Pair");
                    throw exception;
                }

                try
                {
                    currencyFrom = CurrencyPair.Substring(0, 3);
                    currencyTo = CurrencyPair.Substring(3, 3);
                }
                catch(Exception ex)
                {
                    Logger.Error(ex.Message);
                    CustomException exception = new CustomException(System.Net.HttpStatusCode.BadRequest, "Invalid Currency Pair");
                    throw exception;
                }


                if (currencyFrom == refCurrency)
                {
                    rateFrom = 1m;
                }
                else
                {
                    if (allExchangeRates.Any(cur => cur.Currency.ToUpper() == currencyFrom.ToUpper()))
                    {
                        rateFrom = allExchangeRates.Where(x => x.Currency.ToUpper() == currencyFrom.ToUpper()).Select(x => x.Rate).FirstOrDefault();
                    }
                    else
                    {
                        CustomException exception = new CustomException(System.Net.HttpStatusCode.NotFound, "Currency '" + currencyFrom + "' not available for conversion in XML File");
                        throw exception;
                    }
                }




                if (currencyTo == refCurrency)
                {
                    rateTo = 1m;
                }
                else
                {
                    if (allExchangeRates.Any(cur => cur.Currency.ToUpper() == currencyTo.ToUpper()))
                    {
                        rateTo = allExchangeRates.Where(x => x.Currency.ToUpper() == currencyTo.ToUpper()).Select(x => x.Rate).FirstOrDefault();
                    }
                    else
                    {
                        CustomException exception = new CustomException(System.Net.HttpStatusCode.NotFound, "Currency '" + currencyTo + "' not available for conversion in XML File");
                        throw exception;
                    }
                }
               
                // Rounding Rates to 4 Decimal Place
                return Math.Round(rateTo / rateFrom, 4);
            }
            catch (Exception ex )
            {
                throw ex;
            }
           
        }
    }
}
