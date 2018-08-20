using ExchangeRates.Common;
using ExchangeRates.Model;
using System;
using System.Web.Http;

namespace ExchangeRates.Controller
{
    [RoutePrefix("Rate")]
    public class ExchangeRatesController : ApiController
    {
        [Route("{currencyPair = currencyPair}")] //Sample Route http://localhost:7000/Rate?CurrencyPair=USDINR
        [Route("{currencyPair}")] // Sample Route http://localhost:7000/Rate/USDINR
        public IHttpActionResult GetExchangeRate(string currencyPair)
        {
            try
            {
                ExchangeRatesModel exchangeRatesModel = new ExchangeRatesModel();
                return Ok(exchangeRatesModel.GetExchangeRate(currencyPair));
            }
            catch (CustomException ex)
            {
                //Handling Custom Exceptions
                ErrorModel errorModel = new ErrorModel(ex.ErrorModel.ErrorMessage, ex.ErrorModel.ErrorCode);
                return Json(errorModel);
            }
            catch (Exception ex)
            {
                //Exceptions other than custom erros are being handled here.
                Logger.Error(ex.Message);
                CustomException customException = new CustomException(System.Net.HttpStatusCode.InternalServerError, "Something went wrong..!!");
                return Json(customException.ErrorModel);
            }
        }

        //[WebApiCache(60)]
        //[Route("GetAll")]
        //public IHttpActionResult GetAll()
        //{
        //    ExchangeRatesModel rateModel = new ExchangeRatesModel();
        //    return Ok(rateModel.GetAllExchangeRates());
        //}
    }
}
