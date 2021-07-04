using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Interfaces.Data;

namespace Unison.Cloud.Infrastructure.Data.Repositories
{
    public class SyncAgentRepository : ISyncAgentRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SyncEntityRepository> _logger;

        public SyncAgentRepository(AppDbContext context, ILogger<SyncEntityRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public SyncAgent Find(int id)
        {
            return _context.SyncAgent.Find(id);
        }

        public SyncAgent FindByInstanceId(string instanceId)
        {
            return _context.SyncAgent.Where(e => e.InstanceId == instanceId).FirstOrDefault();
        }

        public IEnumerable<SyncAgent> GetAll()
        {
            return _context.SyncAgent.ToList();
        }

        public async Task<IEnumerable<SyncAgent>> GetAllAsync()
        {
            return await _context.SyncAgent.ToListAsync();
        }
    }
}
