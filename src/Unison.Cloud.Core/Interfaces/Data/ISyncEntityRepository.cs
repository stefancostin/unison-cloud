using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;

namespace Unison.Cloud.Core.Interfaces.Data
{
    public interface ISyncEntityRepository : IBaseRepository
    {
        void Add(SyncEntity entity);
        SyncEntity Find(int id);
        SyncEntity FindByEntityName(string entity);
        IEnumerable<SyncEntity> FindByNodeId(int nodeId);
        IEnumerable<SyncEntity> GetAll();
        void Remove(SyncEntity entity);
    }
}
