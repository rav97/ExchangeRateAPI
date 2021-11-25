using ExchangeRateAPI.Services.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Utility
{
    public class ApiKeyValidationAttribute : ValidationAttribute
    {
        private string _errorMessage = "Invalid API key";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult(_errorMessage);

            string key = value.ToString();

            try
            {
                var keyDecode = key.FromBase64();
                var parts = keyDecode.Split('-');

                if (parts.Length == 2)
                {
                    string createPart = parts[0];
                    string validPart = parts[1];

                    var createTime = DateTime.ParseExact(createPart, "HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture);
                    var endDate = DateTime.ParseExact(validPart, "yy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);
                }

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
