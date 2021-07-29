using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Exceptions;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Core.Interfaces.Workers;
using Unison.Cloud.Core.Models;
using Unison.Common.Amqp.DTO;

namespace Unison.Cloud.Core.Workers
{
    public class SyncErrorWorker : ISubscriptionWorker<AmqpSyncError>
    {
        private readonly ServicesContext _servicesContext;
        private readonly ILogger<SyncErrorWorker> _logger;

        public SyncErrorWorker(ServicesContext servicesContext, ILogger<SyncErrorWorker> logger)
        {
            _servicesContext = servicesContext;
            _logger = logger;
        }

        public void ProcessMessage(AmqpSyncError message)
        {
            ValidateMessage(message);
            UpdateSyncLog(message);
        }

        private void UpdateSyncLog(AmqpSyncError message)
        {
            using (var scope = _servicesContext.Services.CreateScope())
            {
                var syncLogRepository = scope.ServiceProvider.GetRequiredService<ISyncLogRepository>();
                SyncLog syncLog = syncLogRepository.FindByCorrelationId(message.CorrelationId);
                syncLog.ErrorMessage = message.ErrorMessage;
                syncLog.UpdatedAt = DateTime.Now;
                syncLogRepository.Save();
            }
        }

        private void ValidateMessage(AmqpSyncError message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.ErrorMessage))
                throw new InvalidRequestException("An error message cannot be empty");

            if (string.IsNullOrWhiteSpace(message.CorrelationId))
                throw new InvalidRequestException("An error message must have a correlation id");
        }
    }
}
