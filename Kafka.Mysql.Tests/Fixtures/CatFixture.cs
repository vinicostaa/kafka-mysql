using System;
using Kafka.Mysql.Example;
using Kafka.Mysql.Example.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kafka.Mysql.Tests.Fixtures
{
    public class BaseFixture<TStartup> : IDisposable where TStartup : class
    {
        public IHost Hostx { get; }

        //public HttpClient Client { get; }
        //public TestServer Server { get; }

        public BaseFixture()
        {
            Hostx = Host
                .CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                })
                .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                .Build();

            //CacheMysql = host.Services.GetRequiredService<ICacheMySql>();
            
            //Server = new TestServer(builder);
            //Client = Server.CreateClient();
        }

        public void Dispose()
        {

            //Client.Dispose();
            //Server.Dispose();
        }
    }
}
