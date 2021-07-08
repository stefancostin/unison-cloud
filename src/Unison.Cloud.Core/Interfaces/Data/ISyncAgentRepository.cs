using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;

namespace Unison.Cloud.Core.Interfaces.Data
{
    public interface ISyncAgentRepository : IBaseRepository
    {
        void Add(SyncAgent agent);
        SyncAgent Find(int id);
        SyncAgent FindByInstanceId(string instanceId);
        IEnumerable<SyncAgent> FindByNodeId(int nodeId);
        IEnumerable<SyncAgent> GetAll();
        void Remove(SyncAgent agent);
    }
}
