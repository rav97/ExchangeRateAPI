using System;
using System.Collections.Generic;

#nullable disable

namespace ExchangeRateAPI.Database
{
    public partial class ExchangeRate
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string CurrencyFrom { get; set; }
        public string CurrencyTo { get; set; }
        public decimal ExchangeRate1 { get; set; }
    }
}
