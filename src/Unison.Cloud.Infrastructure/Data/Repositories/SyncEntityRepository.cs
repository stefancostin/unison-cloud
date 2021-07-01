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
    public class SyncEntityRepository : ISyncEntityRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SyncEntityRepository> _logger;

        public SyncEntityRepository(AppDbContext context, ILogger<SyncEntityRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public SyncEntity Find(int id)
        {
            return _context.SyncEntities.Find(id);
        }

        public IEnumerable<SyncEntity> GetAll()
        {
            return _context.SyncEntities.ToList();
        }

        public async Task<IEnumerable<SyncEntity>> GetAllAsync()
        {
            return await _context.SyncEntities.ToListAsync();
        }
    }
}
