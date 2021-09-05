using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;

namespace Unison.Cloud.Core.Interfaces.Services
{
    public interface IVersioningService
    {
        long IncrementVersion(SyncVersion version);
        SyncVersion GetVersion(string entityName, int agentId);
    }
}
