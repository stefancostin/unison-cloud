using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Infrastructure.Amqp.Interfaces;

namespace Unison.Cloud.Infrastructure.Amqp.Models
{
    public class AmqpManagedChannel : IAmqpManagedChannel
    {
        private readonly IAmqpChannelFactory _channelFactory;
        private IModel _channel;

        public AmqpManagedChannel(IModel channel, IAmqpChannelFactory channelFactory)
        {
            _channelFactory = channelFactory;
            _channel = channel;
        }

        /// <summary>
        /// Retrieves a RabbitMQ channel. If the initial channel was closed
        /// by an unforseen error, then it will create another channel.
        /// </summary>
        /// <returns>
        /// A RabbitMQ channel.
        /// </returns>
        public IModel GetChannel()
        {
            if (_channel.IsOpen)
            {
                return _channel;
            }
            else
            {
                _channel?.Dispose();
                _channel = _channelFactory.CreateUnmanagedChannel();
                return _channel;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }

        ~AmqpManagedChannel()
        {
            _channel?.Dispose();
        }
    }
}
