using ExchangeRateAPI.Database;
using ExchangeRateAPI.Services.Manager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Services.Manager
{
    public class SqlClient : ISqlClient
    {
        public ExchangeDBContext DbContext { get; private set; }

        public SqlClient(ExchangeDBContext context)
        {
            this.DbContext = context;
        }
    }
}
