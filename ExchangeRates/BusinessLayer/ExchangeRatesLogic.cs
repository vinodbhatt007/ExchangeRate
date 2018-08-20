using ExchangeRates.Common;
using ExchangeRates.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
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

        private const string cacheKey = "GetAllExchangeRates";
        private List<ExchangeRatesModel> allExchangeRates;
        private ExchangeRatesModel exchangeRatesModel;

        public List<ExchangeRatesModel> GetAllExchangeRates()
        {
            allExchangeRates = new List<ExchangeRatesModel>();
            string XMLPath = ConfigurationManager.AppSettings["XMLPath"];
            using (XmlReader xmlr = XmlReader.Create(XMLPath))
            {
                xmlr.ReadToFollowing("Cube");
                while (xmlr.Read())
                {
                    if (xmlr.NodeType != XmlNodeType.Element) continue;

                    if (xmlr.GetAttribute("currency") != null && xmlr.GetAttribute("rate") != null)
                    {
                        exchangeRatesModel = new ExchangeRatesModel
                        {
                            Currency = xmlr.GetAttribute("currency").ToUpper(),
                            Rate = decimal.Parse(xmlr.GetAttribute("rate"), CultureInfo.InvariantCulture)
                        };
                        allExchangeRates.Add(exchangeRatesModel);
                    }
                }
            }
            return allExchangeRates;
        }

        public decimal GetExchangeRate(string currencyPair)
        {
            try
            {
                decimal rateFrom = 0m;
                decimal rateTo = 0m;
                string currencyFrom = string.Empty;
                string currencyTo = string.Empty;
                //Read Reference Currency from Config File
                string refCurrency = ConfigurationManager.AppSettings["RefCurrency"]; //EUR

                // Currency Pair Validations
                if (string.IsNullOrEmpty(currencyPair))
                {
                    Logger.Error("Error Occured in GetExchangeRate(): Invalid Currency Pair: Currency Pair cannot be NULL or Empty");
                    CustomException exception = new CustomException(System.Net.HttpStatusCode.BadRequest, "Invalid Currency Pair: Currency Pair cannot be NULL or Empty");
                    throw exception;
                }
                else if (currencyPair.Length > 6)
                {
                    Logger.Error("Error Occured in GetExchangeRate(): Invalid Currency Pair: Length Cannot be greater than 6");
                    CustomException exception = new CustomException(System.Net.HttpStatusCode.BadRequest, "Invalid Currency Pair: Length Cannot be greater than 6");
                    throw exception;
                }


                // Get CurrencyTo and CurrencyFrom 
                try
                {
                    currencyFrom = currencyPair.Substring(0, 3).ToUpper();
                    currencyTo = currencyPair.Substring(3, 3).ToUpper();
                }
                catch (Exception ex)
                {
                    Logger.Error("Error Occured in GetExchangeRate(): " + ex.Message);
                    CustomException exception = new CustomException(System.Net.HttpStatusCode.BadRequest, "Invalid Currency Pair");
                    throw exception;
                }

                // Set Reference Currency to EUR, if RefCurrency is null or not provided in Config file
                if (string.IsNullOrEmpty(refCurrency))
                {
                    refCurrency = "EUR";
                }

                #region GetAllExchangeRates Caching
                allExchangeRates = new List<ExchangeRatesModel>();
                ObjectCache cache = MemoryCache.Default;
                if (cache.Contains(cacheKey))
                    allExchangeRates = (List<ExchangeRatesModel>)cache.Get(cacheKey);
                else
                {
                    try
                    {
                        allExchangeRates = GetAllExchangeRates();
                    }
                    catch (ArgumentNullException ex)
                    {
                        Logger.Error("Error Occured in GetExchangeRate(): " +ex.Message);
                        CustomException exception = new CustomException(System.Net.HttpStatusCode.InternalServerError, "Error occured while reading Config file");
                        throw exception;
                    }
                    catch (FileNotFoundException ex)
                    {
                        Logger.Error("Error Occured in GetExchangeRate(): " + ex.Message);
                        CustomException exception = new CustomException(System.Net.HttpStatusCode.InternalServerError, ex.Message);
                        throw exception;
                    }
                    catch(Exception ex)
                    {
                        Logger.Error("Error Occured in GetExchangeRate(): " + ex.Message);
                        CustomException exception = new CustomException(System.Net.HttpStatusCode.InternalServerError, "Error occured while reading XML file");
                        throw exception;
                    }

                    //Set Default Cache Time to 0 Minutes
                    double cacheTimingInMinutes = 0;
                    double.TryParse(ConfigurationManager.AppSettings["CacheTimingInMinutes"], out cacheTimingInMinutes);
                    // Storing data in the cache    
                    CacheItemPolicy cacheItemPolicy = new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(cacheTimingInMinutes)
                    };
                    cache.Add(cacheKey, allExchangeRates, cacheItemPolicy);
                }
                #endregion

                // SET rateFrom to 1 if conversion is being done from EUR
                if (currencyFrom == refCurrency)
                {
                    rateFrom = 1m;
                }
                else
                {
                    if (allExchangeRates.Any(cur => cur.Currency == currencyFrom))
                    {
                        rateFrom = allExchangeRates.Where(x => x.Currency == currencyFrom).Select(x => x.Rate).FirstOrDefault();
                    }
                    else
                    {
                        Logger.Error("Error Occured in GetExchangeRate(): Currency '" + currencyFrom + "' not available for conversion in XML File");
                        CustomException exception = new CustomException(System.Net.HttpStatusCode.NotFound, "Currency '" + currencyFrom + "' not available for conversion in XML File");
                        throw exception;
                    }
                }



                // SET rateTo to 1 if conversion is being done to EUR
                if (currencyTo == refCurrency)
                {
                    rateTo = 1m;
                }
                else
                {
                    if (allExchangeRates.Any(cur => cur.Currency == currencyTo))
                    {
                        rateTo = allExchangeRates.Where(x => x.Currency== currencyTo).Select(x => x.Rate).FirstOrDefault();
                    }
                    else
                    {
                        Logger.Error("Error Occured in GetExchangeRate(): Currency '" + currencyTo + "' not available for conversion in XML File");
                        CustomException exception = new CustomException(System.Net.HttpStatusCode.NotFound, "Currency '" + currencyTo + "' not available for conversion in XML File");
                        throw exception;
                    }
                }
               
                // Rounding Rates to 4 Decimal Place
                return Math.Round(rateTo / rateFrom, 4);
            }
            catch(DivideByZeroException ex)
            {
                Logger.Error("Error Occured in GetExchangeRate(): " + ex.Message);
                CustomException exception = new CustomException(System.Net.HttpStatusCode.InternalServerError, ex.Message);
                throw exception;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
    }
}
