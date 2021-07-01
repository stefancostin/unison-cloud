using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;

namespace Unison.Cloud.Core.Interfaces.Data
{
    public interface ISyncEntityRepository
    {
        SyncEntity Find(int id);
        Task<IEnumerable<SyncEntity>> GetAllAsync();
        IEnumerable<SyncEntity> GetAll();
    }
}
