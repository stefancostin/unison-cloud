using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Services
{
    public interface ISync
    {
        void Execute(object state);
    }
}
