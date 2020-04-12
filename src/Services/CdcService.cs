using System;
using System.Text.Json;
using System.Threading;
using Confluent.Kafka;
using Kafka.Mysql.Example.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kafka.Mysql.Example.Services
{
    public class CdcService : ICdcService
    {
        //private readonly ILogger<CacheMySql> _logger;
        private readonly IMemoryCache _cache;
        private readonly IConsumer<string, string> _consumer;
        private readonly ILogger<CdcService> _logger;
        private static readonly string _kafkaTopic = "mysql.cardb.cars";

        public CdcService(IMemoryCache cache, ILogger<CdcService> logger)
        {
            _cache = cache;
            
            var conf = new ConsumerConfig
            {
                GroupId =
                $"mysql.mystore.products.{Guid.NewGuid():N}.group.id", //Choose different group id, because we want to read cache topic from the scratch.
                BootstrapServers = "localhost:9093",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, string>(conf).Build();
            _logger = logger;

            //var conf = new ConsumerConfig
            //{
            //    GroupId =
            //        $"mysql.mystore.products.{Guid.NewGuid():N}.group.id", //Choose different group id, because we want to read cache topic from the scratch.
            //    BootstrapServers = "localhost:9093",
            //    AutoOffsetReset = AutoOffsetReset.Earliest
            //};

           
           // _consumer = new ConsumerBuilder<string, string>(conf).Build();
            _consumer.Subscribe(_kafkaTopic);
        }

        public void Consume(bool returnOnLastOffset, out bool finished, CancellationToken cancellationToken)
        {
            finished = false;
            try
            {
                var watermark = _consumer.QueryWatermarkOffsets(
                    new TopicPartition(_kafkaTopic, new Partition(0)), TimeSpan.FromMilliseconds(60000));

                if (returnOnLastOffset && watermark.High.Value == 0)
                {
                    finished = true;
                    return;
                }

                var consumeResult = _consumer.Consume(cancellationToken);
                if (consumeResult.Message.Value == null)
                {
                    var item = JsonSerializer.Deserialize<CarCacheViewModel>(consumeResult.Message.Key);
                    if (!_cache.TryGetValue(item.Id, out _))
                        return;

                    _logger.LogDebug($"remove cache: {consumeResult.Message.Key}");
                    _cache.Remove(item.Id);
                }
                else
                {
                    var item = JsonSerializer.Deserialize<CarCacheViewModel>(consumeResult.Message.Value);
                    _logger.LogDebug($"new cache: {consumeResult.Message.Value}");
                    _logger.LogDebug($"new cache: {consumeResult.Message.Value}");
                    _cache.Set(item.Id, item);
                }

                if (returnOnLastOffset && watermark.High.Value - 1 == consumeResult.Offset.Value)
                    finished = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _consumer.Close();
                _consumer.Dispose();
                throw;
            }
        }
    }
}
