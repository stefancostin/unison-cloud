using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Models;

namespace Unison.Cloud.Core.Interfaces.Amqp
{
    /// <summary>
    /// Subscribes to an exchange using its own RabbitMQ channel.
    /// </summary>
    public interface IAmqpSubscriber
    {
        void Subscribe();
        void Unsubscribe();
    }
}
