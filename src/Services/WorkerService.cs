using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Kafka.Mysql.Example.Services
{
    public class WorkerService : BackgroundService
    {
        private readonly ICdcService _cdcService;

        public WorkerService(ICdcService cdcService)
        {
            _cdcService = cdcService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() => _cdcService.Consume(false, out _, stoppingToken), stoppingToken);
            }
        }
    }
}
