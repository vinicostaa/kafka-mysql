using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Kafka.Mysql.Example.Services
{
    public class Worker : BackgroundService
    {
        private readonly ICacheMySql _cacheMySql;

        public Worker(ICacheMySql cacheMySql)
        {
            _cacheMySql = cacheMySql;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() => _cacheMySql.Consume(false, out _, stoppingToken), stoppingToken);
            }
        }
    }
}
