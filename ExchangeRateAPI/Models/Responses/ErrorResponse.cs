using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Models.Responses
{
    public class ErrorResponse : Response<ApiError[]>
    {
        [DefaultValue(false)]
        public override bool Succeded { get => base.Succeded; set => base.Succeded = value; }

        public ErrorResponse(string message = "An error occured during processing of request", params ApiError[] errors) : base(errors, message, false) { }
    }
}
