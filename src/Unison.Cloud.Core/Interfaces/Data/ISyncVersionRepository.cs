using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;

namespace Unison.Cloud.Core.Interfaces.Data
{
    public interface ISyncVersionRepository : IBaseRepository
    {
        void Add(SyncVersion version);
        SyncVersion Find(int id);
        IEnumerable<SyncVersion> FindByAgentId(int agentId);
        IEnumerable<SyncVersion> FindByEntityId(int entityId);
        SyncVersion FindByAgentIdAndEntityId(int agentId, int entityId);
        IEnumerable<SyncVersion> GetAll();
        void Update(SyncVersion version);
        void Remove(SyncVersion version);
    }
}
