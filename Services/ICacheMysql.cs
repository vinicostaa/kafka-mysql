using System;
using System.Threading;

namespace Kafka.Mysql.Example.Services
{
    public interface ICacheMySql
    {
        void Consume(bool returnOnLastOffset, out bool finished, CancellationToken cancellationToken);
    }
}
