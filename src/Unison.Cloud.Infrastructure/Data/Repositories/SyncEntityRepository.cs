﻿using Microsoft.EntityFrameworkCore;
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

        public void Add(SyncEntity entity)
        {
            _context.SyncEntities.Add(entity);
        }

        public SyncEntity Find(int id)
        {
            return _context.SyncEntities.Include(e => e.Node).Include(e => e.Versions).FirstOrDefault(e => e.Id == id);
        }

        public SyncEntity FindByEntityName(string entity)
        {
            return _context.SyncEntities.Where(e => e.Entity == entity).Include(e => e.Versions).FirstOrDefault();
        }

        public IEnumerable<SyncEntity> FindByNodeId(int nodeId)
        {
            return _context.SyncEntities.Where(e => e.NodeId == nodeId).Include(e => e.Node).Include(e => e.Versions).ToList();
        }

        public IEnumerable<SyncEntity> GetAll()
        {
            return _context.SyncEntities.Include(e => e.Node).Include(e => e.Versions).ToList();
        }

        public void Remove(SyncEntity entity)
        {
            _context.SyncEntities.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
