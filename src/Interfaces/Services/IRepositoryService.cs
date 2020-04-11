using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kafka.Mysql.Example.ViewModels;

namespace Kafka.Mysql.Example.Interfaces.Services
{
    public interface IRepositoryService
    {
        void GetByIdFromCache(int id, out object value);
        //void Remove(string value);
        //bool TryGetValue(string value);
        //void Set(CarCacheViewModel item);
    }
}
