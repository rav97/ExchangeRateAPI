using System;
using System.Collections.Generic;

#nullable disable

namespace ExchangeRateAPI.Database
{
    public partial class ApiKey
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string ApiKey1 { get; set; }
        public DateTime ValidTo { get; set; }
        public bool Active { get; set; }
    }
}
