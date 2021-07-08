using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;

namespace Unison.Cloud.Core.Interfaces.Data
{
    public interface ISyncNodeRepository : IBaseRepository
    {
        void Add(SyncNode node);
        SyncNode Find(int id);
        IEnumerable<SyncNode> GetAll();
        void Remove(SyncNode node);
    }
}
