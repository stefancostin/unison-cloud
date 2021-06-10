using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Services.Amqp
{
    /// <summary>
    /// The AMQP Client handles all publishing and subscribing to AMQP exchanges.
    /// Its lifespan is request scoped as to have a single AMQP channel per thread.
    /// </summary>
    public interface IAmqpClient
    {
        void Publish(string message);
    }
}
