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

        public bool GetByIdFromCache(int id, out object value)
        {
            var key = id;
            _cache.TryGetValue(key, out var car);
            value = car;
            return car != null;
        }

        public void RemoveFromCache(int id)
        {
            _cache.Remove(id);
        }

        public void SetFromCache(int id, object item)
        {
            _cache.Set(id, item);
        }
    }
}
