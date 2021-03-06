using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Mysql.Example.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kafka.Mysql.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var cdcService = host.Services.GetRequiredService<ICdcService>();

            /*
             * Salvando as mensagens já existentes na fila no Memory Cache
             */
            bool fineshed;
            do
            {
                cdcService.Consume(true, out fineshed, CancellationToken.None);
            } while (fineshed == false);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    /*
                    * Iniciando Service Worker que ficará consumindo o tópico em Background
                    */
                    services.AddHostedService<WorkerService>();
                });
    }
}
