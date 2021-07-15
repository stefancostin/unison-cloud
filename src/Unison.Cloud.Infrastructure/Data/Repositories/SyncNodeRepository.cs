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
    public class SyncNodeRepository : ISyncNodeRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SyncNodeRepository> _logger;

        public SyncNodeRepository(AppDbContext context, ILogger<SyncNodeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Add(SyncNode node)
        {
            _context.SyncNodes.Add(node);
        }

        public SyncNode Find(int id)
        {
            return _context.SyncNodes.Find(id);
        }

        public IEnumerable<SyncNode> GetAll()
        {
            return _context.SyncNodes.Include(n => n.Agents).ToList();
        }

        public void Remove(SyncNode node)
        {
            _context.SyncNodes.Remove(node);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
