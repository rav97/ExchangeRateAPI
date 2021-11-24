using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Models.Responses
{
    public class Response<T>
    {
        public Response() { }
        public Response(T data, string message = "OK", bool succeded = true)
        {
            this.Succeded = succeded;
            this.Message = message;
            this.Data = data;
        }

        [DefaultValue(true)]
        public virtual bool Succeded { get; set; }
        [DefaultValue("General information about result of operation")]
        public virtual string Message { get; set; }
        public T Data { get; set; }
    }
}
