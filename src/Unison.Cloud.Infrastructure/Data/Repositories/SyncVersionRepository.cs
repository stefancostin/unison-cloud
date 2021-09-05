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
    public class SyncVersionRepository : ISyncVersionRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SyncVersionRepository> _logger;

        public SyncVersionRepository(AppDbContext context, ILogger<SyncVersionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(SyncVersion version)
        {
            _context.SyncVersions.Add(version);
        }

        public SyncVersion Find(int id)
        {
            return _context.SyncVersions.Find(id);
        }

        public IEnumerable<SyncVersion> FindByAgentId(int agentId)
        {
            return _context.SyncVersions.Where(v => v.AgentId == agentId).ToList();
        }

        public SyncVersion FindByAgentIdAndEntityId(int agentId, int entityId)
        {
            return _context.SyncVersions.FirstOrDefault(v => v.AgentId == agentId && v.EntityId == entityId);
        }

        public IEnumerable<SyncVersion> FindByEntityId(int entityId)
        {
            return _context.SyncVersions.Where(v => v.EntityId == entityId).ToList();
        }

        public IEnumerable<SyncVersion> GetAll()
        {
            return _context.SyncVersions.ToList();
        }

        public void Remove(SyncVersion version)
        {
            _context.SyncVersions.Remove(version);
        }

        public void Update(SyncVersion version)
        {
            _context.SyncVersions.Update(version);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
