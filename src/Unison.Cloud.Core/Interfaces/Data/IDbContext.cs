using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Interfaces.Data
{
    /// <summary>
    /// Retrieves a db connection from the connection pool that ADO.NET creates when provided with the same connection string
    /// </summary>
    public interface IDbContext
    {
        /// <returns>Returns a connection from the connection pool</returns>
        public DbConnection GetConnection();
    }
}
