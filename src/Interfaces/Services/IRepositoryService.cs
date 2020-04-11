using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kafka.Mysql.Example.ViewModels;

namespace Kafka.Mysql.Example.Interfaces.Services
{
    public interface IRepositoryService
    {
        bool GetByIdFromCache(int id, out object value);
        void RemoveFromCache(int id);
        void SetFromCache(int id, object item);
    }
}
