using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;

namespace Unison.Cloud.Core.Interfaces.Data
{
    public interface ISyncAgentRepository
    {
        SyncAgent Find(int id);
        SyncAgent FindByInstanceId(string instanceId);
        Task<IEnumerable<SyncAgent>> GetAllAsync();
        IEnumerable<SyncAgent> GetAll();
    }
}
