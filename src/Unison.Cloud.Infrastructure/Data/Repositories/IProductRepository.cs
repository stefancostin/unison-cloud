using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Infrastructure.Data.Entities;

namespace Unison.Cloud.Infrastructure.Data.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        void Add(Product product);

    }
}
