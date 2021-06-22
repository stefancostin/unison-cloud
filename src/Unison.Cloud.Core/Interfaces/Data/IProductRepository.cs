using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;

namespace Unison.Cloud.Core.Interfaces.Data
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        IEnumerable<Product> GetAll();
        void Add(Product product);

    }
}
