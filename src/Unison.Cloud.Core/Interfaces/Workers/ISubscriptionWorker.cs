using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Cloud.Core.Interfaces.Workers
{
    public interface ISubscriptionWorker : IAmqpSubscriptionWorker { }
}
