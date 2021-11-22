using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Services.Repositories.Interfaces
{
    public interface IBaseRepository<T>
    {
        T SelectById(int id);
        List<T> SelectAll();
        bool Insert(T data);
        bool Update(int id, T data);
        bool Delete(int id);
        void StartTransaction();
        bool EndTransaction(bool commit);
        R CallTransact<R, T1>(Func<T1, R> function, T1 arg1) where R : struct, IComparable;
        R CallTransact<R, T1, T2>(Func<T1, T2, R> function, T1 arg1, T2 arg2) where R : struct, IComparable;
        R CallTransact<R, T1, T2, T3>(Func<T1, T2, T3, R> function, T1 arg1, T2 arg2, T3 arg3) where R : struct, IComparable;

    }
}
