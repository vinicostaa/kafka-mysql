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
   public class IntegrationTest
    {
        
        [Fact]
        public async Task Get_car_not_exist()
        {
            using var system = SystemUnderTest.ForStartup<Startup>();
  
            await system.Scenario(_ =>
            {
                _.Get.Url("/car/1");
                _.StatusCodeShouldBe(404);
            });
        }

        [Fact]
        public async Task Get_car_exist()
        {
            using var system = SystemUnderTest.ForStartup<Startup>();
            var cache = system.Services.GetRequiredService<ICdcService>();

            bool fineshed;
            do
            {
                cache.Consume(true, out fineshed, CancellationToken.None);
            } while (fineshed == false);

            await system.Scenario(_ =>
            {
                _.Get.Url("/car/1");
                _.StatusCodeShouldBe(200);
            });
        }
    }
}
