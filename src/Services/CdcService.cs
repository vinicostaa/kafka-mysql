using System;
using System.Text.Json;
using System.Threading;
using Confluent.Kafka;
using Kafka.Mysql.Example.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Kafka.Mysql.Example.Services
{
    public class CdcService : ICdcService
    {
        private readonly IMemoryCache _cache;
        private readonly IConsumer<string, string> _consumer;
        private readonly ILogger<CdcService> _logger;
        private static readonly string _kafkaTopic = "mysql.cardb.cars";
        private static readonly string _kafkaServer = "localhost:9093";


        public CdcService(IMemoryCache cache, ILogger<CdcService> logger)
        {
            _logger = logger;
            _cache = cache;

            // Criando sempre um novo grupo para consumirmos a fila do zero.
            var conf = new ConsumerConfig
            {
                GroupId = $"mysql.cardb.{Guid.NewGuid()}.group.id",
                BootstrapServers = _kafkaServer,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, string>(conf).Build();
            _consumer.Subscribe(_kafkaTopic);
        }

        public void Consume(bool returnOnLastOffset, out bool finished, CancellationToken cancellationToken)
        {
            finished = false;
            try
            {
                // pegando número de mensagens a consumir nesse tópico
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

                    _logger.LogDebug($"Removendo do cache: {consumeResult.Message.Key}");
                    _cache.Remove(item.Id);
                }
                else
                {
                    var item = JsonSerializer.Deserialize<CarCacheViewModel>(consumeResult.Message.Value);
                    _logger.LogDebug($"Novo cache: {consumeResult.Message.Value}");
                    _logger.LogDebug($"Novo cache: {consumeResult.Message.Value}");
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
