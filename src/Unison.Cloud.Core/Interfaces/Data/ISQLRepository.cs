using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data;
using Unison.Cloud.Core.Models;

namespace Unison.Cloud.Core.Interfaces.Data
{
    public interface ISQLRepository
    {
        DataSet Read(QuerySchema schema);
        int Execute(QuerySchema schema);
    }
}
