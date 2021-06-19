using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Interfaces.Data
{
    public interface ISQLRepository
    {
        List<Dictionary<string, object>> Read(string sql);
    }
}
