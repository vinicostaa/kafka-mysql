using System;
using System.Text.Json;
using Kafka.Mysql.Example.Interfaces.Services;
using Kafka.Mysql.Example.ViewModels;
using Microsoft.Extensions.Caching.Memory;

namespace Kafka.Mysql.Example.Services
{
    public class CacheRepositoryService : IRepositoryService
    {
        private readonly IMemoryCache _cache;

        public CacheRepositoryService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void GetByIdFromCache(int id, out object value)
        {
            var key = id;

            if (_cache.TryGetValue(key, out var car))
            {
                value = car;
            } else
            {
                value = null;
            }

            //value = null;
        }
    }
}
