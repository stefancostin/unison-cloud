using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Data;

namespace Unison.Cloud.Infrastructure.Data
{
    /// <summary>
    /// Retrieves a database connection from the connection pool that ADO.NET
    /// creates when provided with the same connection string
    /// </summary>
    public class RawDbContext : IDbContext
    {
        private readonly IConfiguration _config;

        public RawDbContext(IConfiguration config)
        {
            _config = config;
        }

        /// <returns>Returns a connection from the connection pool</returns>
        public DbConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("Unison"));
        }
    }
}
