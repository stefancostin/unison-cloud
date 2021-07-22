using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Interfaces.Configuration
{
    public interface IAuthConfiguration
    {
        string Secret { get; set; }
    }
}
