using ExchangeRates.Filters;
using ExchangeRates.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ExchangeRates.BusinessLayer
{
    public class ExchangeRates
    {
        public static List<ExchangeRatesModel> GetAllExchangeRates()
        {
            
            List<ExchangeRatesModel> listModel = new List<ExchangeRatesModel>();
            using (XmlReader xmlr = XmlReader.Create(@"http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml"))
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
                        listModel.Add(model);
                    }
                                         
                }
            }
            return listModel;
        }

        public static decimal GetExchangeRate(string CurrencyPair)
        {
            var all = GetAllExchangeRates();

            string currencyFrom = CurrencyPair.Substring(0, 3);
            string currencyto = CurrencyPair.Substring(CurrencyPair.Length - 3);

            decimal rateFrom = all.Where(x => x.Currency == currencyFrom).Select(x => x.Rate).FirstOrDefault();
            decimal rateTo = all.Where(x => x.Currency == currencyto).Select(x => x.Rate).FirstOrDefault();

            return rateTo / rateFrom;
        }
    }
}
