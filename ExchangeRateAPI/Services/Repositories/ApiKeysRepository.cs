using ExchangeRateAPI.Database;
using ExchangeRateAPI.Services.Manager.Interfaces;
using ExchangeRateAPI.Services.Repositories.Interfaces;
using ExchangeRateAPI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Services.Repositories
{
    public class ApiKeysRepository : BaseRepository<ApiKey>, IApiKeysRepository
    {
        public ApiKeysRepository(ISqlClient sqlClient) : base(sqlClient)
        {

        }

        public bool CheckKey(string passedKey)
        {
            try
            {
                var dbKey = db.ApiKeys.SingleOrDefault(x => x.ApiKey1 == passedKey);

                if (dbKey == null)
                    return false;

                var keyDecode = passedKey.FromBase64();
                var parts = keyDecode.Split('-');

                if (parts.Length == 2)
                {
                    string createPart = parts[0];
                    string validPart = parts[1];

                    var createTime = DateTime.ParseExact(createPart, "HH:mm:ss:fff", System.Globalization.CultureInfo.InvariantCulture);
                    var endDate = DateTime.ParseExact(validPart, "yy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);

                    if ((dbKey.CreateDate.TimeOfDay - createTime.TimeOfDay).Milliseconds < 1000)
                        if (dbKey.ValidTo.Date == endDate.Date)
                            return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public string GenerateNewApiKey(out bool result)
        {
            result = false;
            var dateNow = DateTime.Now;
            ApiKey newKey = new ApiKey
            {
                CreateDate = dateNow,
                ValidTo = dateNow.AddMonths(6),
                Active = true
            };

            string keyPlain = @$"{dateNow.ToString("HH:mm:ss:fff")}-{newKey.ValidTo.Date.ToString("yy.MM.dd")}";
            string keyEncoded = keyPlain.ToBase64();
            newKey.ApiKey1 = keyEncoded;

            result = Insert(newKey);

            return keyEncoded;
        }
    }
}
