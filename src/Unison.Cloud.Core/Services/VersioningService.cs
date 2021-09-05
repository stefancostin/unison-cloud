using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Core.Interfaces.Services;
using Unison.Cloud.Core.Utilities;

namespace Unison.Cloud.Core.Services
{
    public class VersioningService : IVersioningService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<VersioningService> _logger;
        private readonly object _incrementLock;

        public VersioningService(IServiceProvider services, ILogger<VersioningService> logger)
        {
            _services = services;
            _logger = logger;
            _incrementLock = new object();
        }

        public long IncrementVersion(SyncVersion currentVersion)
        {
            long version = currentVersion.Version;

            using (var scope = _services.CreateScope())
            {
                lock (_incrementLock)
                {
                    var versionRepository = scope.ServiceProvider.GetRequiredService<ISyncVersionRepository>();
                    SyncVersion syncVersion = versionRepository.FindByAgentIdAndEntityId(currentVersion.AgentId, currentVersion.EntityId);

                    syncVersion.Version = Interlocked.Increment(ref version);

                    versionRepository.Update(syncVersion);
                    versionRepository.Save();
                }
            }

            return version;
        }

        public SyncVersion GetVersion(string entityName, int agentId)
        {
            SyncEntity entity = GetEntityMetadata(entityName);
            SyncVersion syncVersion = new SyncVersion()
            {
                EntityId = entity.Id,
                AgentId = agentId,
            };

            long? currentVersion = entity.Versions?.FirstOrDefault(v => v.AgentId == agentId)?.Version;

            if (currentVersion == null)
            {
                syncVersion.Version = Versioning.NewVersion;
                InitializeVersion(syncVersion);
            }
            else
            {
                syncVersion.Version = (long)currentVersion;
            }

            return syncVersion;
        }

        private SyncEntity GetEntityMetadata(string entity)
        {
            using (var scope = _services.CreateScope())
            {
                var entityRepository = scope.ServiceProvider.GetRequiredService<ISyncEntityRepository>();
                return entityRepository.FindByEntityName(entity);
            }
        }

        private void InitializeVersion(SyncVersion newVersion)
        {
            using (var scope = _services.CreateScope())
            {
                var versionRepository = scope.ServiceProvider.GetRequiredService<ISyncVersionRepository>();
                versionRepository.Add(newVersion);
                versionRepository.Save();
            }
            _logger.LogInformation($"Created sync version for entity {newVersion.EntityId} and agent {newVersion.AgentId}.");
        }
    }
}
