using Xunit;
using Kafka.Mysql.Example.Services;
using System.Threading;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;


namespace Kafka.Mysql.Test
{
   public class IntegrationTest
    {
        [Fact]
        public void ConsumingAndSaveOnMemoryCache()
        {
            //Arranje
            var logger = Mock.Of<ILogger<CdcService>>();
            var memoryCache = Mock.Of<IMemoryCache>();
            var cachEntry = Mock.Of<ICacheEntry>();
            
            var mockMemoryCache = Mock.Get(memoryCache);
            mockMemoryCache
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(cachEntry);
            CdcService cdcService = new CdcService(memoryCache, logger);

            // Act
            bool fineshed;
            do
            {
                cdcService.Consume(true, out fineshed, CancellationToken.None);
            } while (fineshed == false);

            // Assert
            Assert.True(fineshed);
        }
    }
}
