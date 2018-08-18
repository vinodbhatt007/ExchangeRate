using ExchangeRates.Model;
using System;
using System.Net;

namespace ExchangeRates.Common
{
    [Serializable]
    public class CustomException : Exception
    {
        public ErrorModel ErrorModel;

        public CustomException()
        {
        }

        public CustomException(HttpStatusCode errorCode, string errorMessage)
        {
            ErrorModel = new ErrorModel(errorMessage, errorCode);
        }
    }
}
