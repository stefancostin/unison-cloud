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
    public class SyncLogRepository : ISyncLogRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SyncLogRepository> _logger;

        public SyncLogRepository(AppDbContext context, ILogger<SyncLogRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public void Add(List<SyncLog> syncLog)
        {
            foreach (SyncLog log in syncLog)
            {
                _context.SyncLog.Add(log);
            }
        }

        public SyncLog Find(int id)
        {
            return _context.SyncLog.Find(id);
        }

        public SyncLog FindByCorrelationId(string correlationId)
        {
            return _context.SyncLog.FirstOrDefault(sl => sl.CorrelationId == correlationId);
        }

        public IEnumerable<SyncLog> GetAll()
        {
            return _context.SyncLog.ToList();
        }

        public async Task<IEnumerable<SyncLog>> GetAllAsync()
        {
            return await _context.SyncLog.ToListAsync();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
