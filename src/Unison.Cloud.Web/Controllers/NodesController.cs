using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Web.Models;
using Unison.Cloud.Web.Utilities;

namespace Unison.Cloud.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodesController : ControllerBase
    {
        private readonly ISyncNodeRepository _nodeRepository;
        private readonly ILogger<NodesController> _logger;

        public NodesController(ISyncNodeRepository nodeRepository, ILogger<NodesController> logger)
        {
            _nodeRepository = nodeRepository;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<NodeDto>> Get()
        {
            IEnumerable<SyncNode> nodes = _nodeRepository.GetAll();
            return Ok(nodes.Select(n => n.ToHttpModel()));
        }

        [HttpGet("{id}")]
        public ActionResult<SyncNode> Get(int id)
        {
            SyncNode node = _nodeRepository.Find(id);
            return Ok(node.ToHttpModel());
        }

        [HttpPost]
        public ActionResult Post(NodeDto node)
        {
            _nodeRepository.Add(node.ToDbModel());
            _nodeRepository.Save();
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, NodeDto node)
        {
            SyncNode existingNode = _nodeRepository.Find(id);

            if (existingNode == null)
                return NotFound();

            existingNode.Name = node.Name;
            existingNode.Description = node.Description;
            existingNode.UpdatedAt = DateTime.Now;

            _nodeRepository.Save();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            SyncNode node = _nodeRepository.Find(id);

            if (node == null)
                return NotFound();

            _nodeRepository.Remove(node);
            _nodeRepository.Save();
            return Ok();
        }
    }
}
