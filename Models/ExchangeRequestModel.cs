using ExchangeRateAPI.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Models
{
    public class ExchangeRequestModel
    {
        [Required]
        [DataType(DataType.Text)]
        [DefaultValue("{\"USD\":\"EUR\"}")]
        [RegularExpression(@"^{(\""[A-Z]{3}\""\ *\:\""[A-Z]{3}\""\,?\ ?)+}$")]
        public string CurrencyCodes { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Api key must be included")]
        [DataType(DataType.Text)]
        [ApiKeyValidation]
        public string ApiKey { get; set; }

        private Dictionary<string, string> _currencyCodesDict;
        internal Dictionary<string, string> CurrencyCodesDict
        {
            get
            {
                if(_currencyCodesDict == null)
                    if (CurrencyCodes != null && CurrencyCodes != string.Empty)
                        _currencyCodesDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(CurrencyCodes);

                return _currencyCodesDict;
            }
        }
    }
}
