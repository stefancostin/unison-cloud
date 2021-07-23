using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Core.Services;
using Unison.Cloud.Web.Models;
using Unison.Cloud.Web.Utilities;

namespace Unison.Cloud.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionsController : ControllerBase
    {
        private readonly ISyncNodeRepository _nodeRepository;
        private readonly ConnectionsManager _connectionsManager;
        private readonly ILogger<ConnectionsController> _logger;

        public ConnectionsController(ConnectionsManager connectionsManager, ISyncNodeRepository nodeRepository, ILogger<ConnectionsController> logger)
        {
            _connectionsManager = connectionsManager;
            _nodeRepository = nodeRepository;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ConnectionDto>> Get()
        {
            List<SyncNode> nodes = _nodeRepository.GetAll().ToList();

            return _connectionsManager.ConnectedInstances.Values
                .Join(nodes, 
                    connectedInstance => connectedInstance.NodeId, 
                    node => node.Id, 
                    (connectedInstance, node) => new ConnectionDto 
                    { 
                        InstanceId = connectedInstance.InstanceId,
                        LastSeen = connectedInstance.LastSeen,
                        Node = node.ToHttpModel()
                    })
                .ToList();
        }
    }
}
