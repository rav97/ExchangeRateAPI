using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Models.Responses
{
    public class ApiError
    {
        [DefaultValue("Source of error")]
        public string Key { get; set; }

        [DefaultValue("Error info")]
        public string Message { get; set; }
    }
}
