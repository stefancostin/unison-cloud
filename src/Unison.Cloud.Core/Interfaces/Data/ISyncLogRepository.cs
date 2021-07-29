using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;

namespace Unison.Cloud.Core.Interfaces.Data
{
    public interface ISyncLogRepository : IBaseRepository
    {
        void Add(SyncLog syncLog);
        void Add(List<SyncLog> syncLog);
        SyncLog Find(int id);
        SyncLog FindByCorrelationId(string correlationId);
        IEnumerable<SyncLog> GetAll();
    }
}
