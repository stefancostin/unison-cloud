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

        public void Add(SyncAgent agent)
        {
            _context.Add(agent);
        }

        public SyncAgent Find(int id)
        {
            return _context.SyncAgent.Find(id);
        }

        public SyncAgent FindByInstanceId(string instanceId)
        {
            return _context.SyncAgent.Where(a => a.InstanceId == instanceId).FirstOrDefault();
        }

        public IEnumerable<SyncAgent> FindByNodeId(int nodeId)
        {
            return _context.SyncAgent.Where(a => a.NodeId == nodeId).ToList();
        }

        public IEnumerable<SyncAgent> GetAll()
        {
            return _context.SyncAgent.ToList();
        }

        public void Remove(SyncAgent agent)
        {
            _context.SyncAgent.Remove(agent);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
