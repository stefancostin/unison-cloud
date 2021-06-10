using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Services.Amqp;

namespace Unison.Cloud.Infrastructure.Amqp
{
    public class AmqpChannelFactory : IAmqpChannelFactory
    {
        private const string _connectionString = "amqp://guest:guest@localhost:5672";
        private readonly IConnection _connection;

        public AmqpChannelFactory()
        {
            var connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(_connectionString)
            };
            _connection = connectionFactory.CreateConnection();
        }

        public IAmqpManagedChannel CreateManagedChannel()
        {
            return new AmqpManagedChannel(_connection.CreateModel(), this);
        }

        public IModel CreateUnmanagedChannel()
        {
            return _connection.CreateModel();
        }

        ~AmqpChannelFactory()
        {
            _connection?.Dispose();
        }
    }
}
