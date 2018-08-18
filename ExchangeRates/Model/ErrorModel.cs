using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates.Model
{
    public class ErrorModel
    {
        public string ErrorMessage { get; set; }
        public HttpStatusCode ErrorCode { get; set; }

        public ErrorModel()
        {
        }

        public ErrorModel(string errorMessage, HttpStatusCode errorCode)
        {
            this.ErrorMessage = errorMessage;
            this.ErrorCode = errorCode;

        }

    }
}
