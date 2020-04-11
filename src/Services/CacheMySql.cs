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
    public class CacheMySql : ICacheMySql
    {
        private readonly ILogger<CacheMySql> _logger;
        private readonly IMemoryCache _cache;
        private readonly IConsumer<string, string> _consumer;
        private readonly string cardb_cars_topic;

        public CacheMySql(IMemoryCache cache, ILogger<CacheMySql> logger, IConfiguration configuration)
        {
            _cache = cache;
            _logger = logger;

            // Construindo Consumidor
            cardb_cars_topic = configuration.GetSection("TOPIC_MYSQL_CAR").Value;
            _consumer = new ConsumerBuilder<string, string>(GetConsumerConfig(configuration)).Build();
            _consumer.Subscribe(cardb_cars_topic);
        }

        public void Consume(bool returnOnLastOffset, out bool finished, CancellationToken cancellationToken)
        {
            finished = false;
            try
            {

                // Obtendo a quandidade de mensagens na fila
                var watermark = _consumer.QueryWatermarkOffsets(
                    new TopicPartition(cardb_cars_topic, new Partition(0)), TimeSpan.FromMilliseconds(60000));

                if (returnOnLastOffset && watermark.High.Value == 0)
                {
                    finished = true;
                    return;
                }

                // Consumindo o as mensagens e salvando, alterando ou sobrepondo itens do MemoryCache
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
                    _cache.Set(item.Id, item);
                }

                /*
                 * Caso não contenha mais mensagens a serem consumidas na fila
                 * O processo estara finalizado
                 */
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

        public ConsumerConfig GetConsumerConfig(IConfiguration configuration)
        {
            return new ConsumerConfig
            {
                GroupId =
                   $"mysql.cardb.cars.{Guid.NewGuid():N}.group.id", //Choose different group id, because we want to read cache topic from the scratch.
                BootstrapServers = configuration.GetSection("KAFKA_SERVER").Value,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }
    }
}
