using System;
using System.Collections.Generic;

#nullable disable

namespace ExchangeRateAPI.Database
{
    public partial class Currency
    {
        public int Id { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
    }
}
