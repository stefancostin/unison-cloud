using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Core.Exceptions
{
    public class AgentNotFoundException : Exception
    {
        public AgentNotFoundException() { }

        public AgentNotFoundException(string message) : base(message) { }

        public AgentNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
