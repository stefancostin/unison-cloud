using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Services.Amqp;

namespace Unison.Cloud.Core.Services
{
    public class Sync : ISync
    {
        private readonly ILogger<Sync> _logger;
        private readonly IAmqpClient _amqpClient;

        public Sync(ILogger<Sync> logger, IAmqpClient amqpClient)
        {
            _logger = logger;
            _amqpClient = amqpClient;
        }

        public void Execute(object state)
        {
            _logger.LogInformation("Hello from Job");
            _amqpClient.Publish("Hello from Cloud Server");
        }
    }
}
