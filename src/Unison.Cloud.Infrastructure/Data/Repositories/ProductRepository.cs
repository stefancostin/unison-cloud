using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data;
using Unison.Cloud.Core.Interfaces.Data;

namespace Unison.Cloud.Infrastructure.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(AppDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // TODO: Make them async w/ Task<>
        public void Add(Product product)
        {
            _context.Add(product);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products.ToList();
        }
    }
}
