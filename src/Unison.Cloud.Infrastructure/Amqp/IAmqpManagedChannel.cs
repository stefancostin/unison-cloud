using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Infrastructure.Amqp
{
    /// <summary>
    /// Handles retrieving a self-managed RabbitMQ channel, even if it was closed due to unforseen errors.
    /// It is also responsible for disposing the RabbitMQ channel when it is no longer used.
    /// </summary>
    public interface IAmqpManagedChannel
    {
        IModel GetChannel();
    }
}
