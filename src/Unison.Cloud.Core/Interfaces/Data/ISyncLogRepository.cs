using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;

namespace Unison.Cloud.Core.Interfaces.Data
{
    public interface ISyncLogRepository
    {
        void Add(List<SyncLog> syncLog);
        SyncLog Find(int id);
        SyncLog FindByCorrelationId(string correlationId);
        Task<IEnumerable<SyncLog>> GetAllAsync();
        IEnumerable<SyncLog> GetAll();
        void SaveChanges();
    }
}
