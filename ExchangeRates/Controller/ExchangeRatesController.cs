using ExchangeRates.Common;
using ExchangeRates.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ExchangeRates.Controller
{
    [RoutePrefix("Rate")]
    public class ExchangeRatesController : ApiController
    {
        ExchangeRatesModel exchangeRatesModel = new ExchangeRatesModel(); 

        [Route("{currencyPair = currencyPair}")] //Sample Route http://localhost:7000/Rate?CurrencyPair=USDINR
        [Route("{currencyPair}")] // Sample Route http://localhost:7000/Rate/USDINR
        public IHttpActionResult GetExchangeRate(string currencyPair)
        {
            try
            {
                if (string.IsNullOrEmpty(currencyPair))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Currency Pair: Currency Pair cannot be NULL or Empty"));
                }
                else if (currencyPair.Length != 6)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Currency Pair: Currency Pair must be of 6 characters"));
                }

                return Ok(exchangeRatesModel.GetExchangeRate(currencyPair));
            }
            catch (CustomException ex)
            {
                //Handling Custom Exceptions
                //ErrorModel errorModel = new ErrorModel(ex.ErrorModel.ErrorMessage, ex.ErrorModel.ErrorCode);
                 return ResponseMessage(Request.CreateErrorResponse(ex.ErrorModel.ErrorCode, ex.ErrorModel.ErrorMessage));

            }
            catch (Exception ex)
            {
                //Exceptions other than custom erros are being handled here.
                Logger.Error(ex.Message);
                //CustomException customException = new CustomException(System.Net.HttpStatusCode.InternalServerError, "Something went wrong..!!");
                //return Json(customException.ErrorModel);
                return ResponseMessage(Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, "Something went wrong..!!"));
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
