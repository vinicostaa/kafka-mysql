using System;
using Kafka.Mysql.Example;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Kafka.Mysql.Tests
{
    public class BaseFixture<TStartup> : IDisposable where TStartup : class
    {
        public BaseFixture()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
                //.ConfigureServices((hostContext, services) =>
                //{
                //    /*
                //        * Iniciando Service Worker que ficará consumindo o tópico em Background
                //        */
                //    services.AddHostedService<Worker>();
                //});
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
