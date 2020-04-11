using System;
using Xunit;
using Kafka.Mysql.Example;
using Alba;
using System.Threading.Tasks;
using Kafka.Mysql.Example.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace Kafka.Mysql.Test
{
   public class UnitTest1
    {
        
        [Fact]
        public async Task Should_get_car()
        {
            using var system = SystemUnderTest.ForStartup<Startup>();
            var cache = system.Services.GetRequiredService<ICacheMySql>();

            bool fineshed;
            do
            {
                cache.Consume(true, out fineshed, CancellationToken.None);
            } while (fineshed == false);

            // This runs an HTTP request and makes an assertion
            // about the expected content of the response
            await system.Scenario(_ =>
            {
                _.Get.Url("/car/1");
                _.StatusCodeShouldBeOk();
            });

        }
    }
}
