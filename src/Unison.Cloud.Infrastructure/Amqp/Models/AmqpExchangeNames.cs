using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Cloud.Infrastructure.Amqp.Models
{
    public static class AmqpExchangeNames
    {
        public static string Commands { get { return "Commands"; } }
        public static string Connections { get { return "Connections";  } }

        public static string Heartbeats { get { return "Heartbeats"; } }
        public static string Responses { get { return "Responses"; } }

    }
}
