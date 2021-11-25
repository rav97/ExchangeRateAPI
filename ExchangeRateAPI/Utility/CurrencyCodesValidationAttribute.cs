using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Utility
{
    public class CurrencyCodesValidationAttribute : ValidationAttribute
    {
        private string _errorMessage = "Invalid currency codes dictionary. ";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult(_errorMessage);

            string dictString = value.ToString();

            try
            {
                var matches = Regex.Matches(dictString, @"[{,] ?""[A-Z]{3}""");
                var keys = new List<string>();
                foreach (var m in matches)
                    keys.Add(m.ToString().Replace("{", "").Replace(",", "").Replace(" ",""));

                if(keys.Count == keys.Distinct().Count())
                    return ValidationResult.Success;
            }
            catch (Exception e)
            {
                return new ValidationResult(_errorMessage);
            }

            return new ValidationResult(_errorMessage);
        }
    }
}
