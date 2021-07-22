using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Configuration;

namespace Unison.Cloud.Core.Models
{
    public class AuthConfiguration : IAuthConfiguration
    {
        public string Secret { get; set; }
    }
}
