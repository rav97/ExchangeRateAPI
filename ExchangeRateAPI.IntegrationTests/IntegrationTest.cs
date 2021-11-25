using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ExchangeRateAPI.IntegrationTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient _testClient;
        public IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            _testClient = appFactory.CreateClient();
        }

        private void TestFormat()
        {
            #region [ ARRANGE ]
            #endregion

            #region [ ACT ]
            #endregion

            #region [ ASSERT ]
            #endregion
        }
    }
}
