using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Utility
{
    public static class DBExtension
    {
        /// <summary>
        /// Get non-generic dataset from given Type. Only for simple operations.
        /// </summary>
        /// <param name="context">DbContext</param>
        /// <param name="t">Type of dataset</param>
        /// <returns></returns>
        public static IQueryable Set(this DbContext context, Type t)
        {
            var methods = typeof(DbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.IsGenericMethod && x.Name.Contains("Set")).ToList();

            if (methods.Count < 1)
                return null;

            var method = methods[0].MakeGenericMethod(t);
            return method.Invoke(context, null) as IQueryable;
        }
    }
}
