using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Models.Responses
{
    public class EmptyResponse : Response<object>
    {
        public EmptyResponse(string message = "OK", bool succeded = true) : base(null, message, succeded) { }
    }
}
