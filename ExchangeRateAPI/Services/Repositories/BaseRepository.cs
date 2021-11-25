using ExchangeRateAPI.Database;
using ExchangeRateAPI.Services.Manager.Interfaces;
using ExchangeRateAPI.Services.Repositories.Interfaces;
using ExchangeRateAPI.Utility;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeRateAPI.Services.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ExchangeDBContext db;
        private IDbContextTransaction transaction;

        public BaseRepository(ISqlClient sqlClient)
        {
            db = sqlClient.DbContext;
        }

        public T SelectById(int id)
        {
            return db.Set<T>().Find(id);
        }
        public List<T> SelectAll()
        {
            return db.Set<T>().ToList();
        }
        public bool Insert(T data)
        {
            db.Set<T>().Add(data);
            return db.SaveChanges() > 0;
        }
        public bool Update(int id, T data)
        {
            var item = SelectById(id);
            item.UpdateObject(data);

            db.Set<T>().Update(item);
            return db.SaveChanges() >= 0;
        }
        public bool Delete(int id)
        {
            var item = SelectById(id);
            db.Set<T>().Remove(item);

            return db.SaveChanges() > 0;
        }
        public void StartTransaction()
        {
            transaction = db.Database.BeginTransaction();
        }

        public bool EndTransaction(bool commit)
        {
            if(transaction != null)
            {
                if (commit)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
            return commit;
        }

        public R CallTransact<R, T1>(Func<T1, R> function, T1 arg1) where R : struct, IComparable
        {
            StartTransaction();
            var result = function(arg1);
            CheckResult(result);
            return result;

        }
        public R CallTransact<R, T1, T2>(Func<T1, T2, R> function, T1 arg1, T2 arg2) where R : struct, IComparable
        {
            StartTransaction();
            var result = function(arg1, arg2);
            CheckResult(result);
            return result;
        }
        public R CallTransact<R, T1, T2, T3>(Func<T1, T2, T3, R> function, T1 arg1, T2 arg2, T3 arg3) where R : struct, IComparable
        {
            StartTransaction();
            var result = function(arg1, arg2, arg3);
            CheckResult(result);
            return result;
        }

        private void CheckResult<R>(R result) where R : struct, IComparable
        {
            if(result is bool)
            {
                if (result.Equals(true))
                    EndTransaction(true);
                else
                    EndTransaction(false);
            }
            else
            {
                if (result.CompareTo(0) > 0)
                    EndTransaction(true);
                else
                    EndTransaction(false);
            }
        }
    }
}
