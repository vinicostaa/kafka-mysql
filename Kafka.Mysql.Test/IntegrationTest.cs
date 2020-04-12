using System;
using Xunit;
using Kafka.Mysql.Example.Services;
using System.Threading;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Kafka.Mysql.Example.Controllers;

namespace Kafka.Mysql.Test
{
   public class IntegrationTest
    {
        [Fact]
        public void ConsumingAndSaveOnMemoryCache()
        {
            var logger = Mock.Of<ILogger<CdcService>>();
            var memoryCache = Mock.Of<IMemoryCache>();
            var cachEntry = Mock.Of<ICacheEntry>();
            

            var mockMemoryCache = Mock.Get(memoryCache);
            mockMemoryCache
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(cachEntry);

            CdcService cdcService = new CdcService(memoryCache, logger);

            bool fineshed;
            do
            {
                cdcService.Consume(true, out fineshed, CancellationToken.None);
            } while (fineshed == false);

            Assert.True(fineshed);
        }
    }
}
