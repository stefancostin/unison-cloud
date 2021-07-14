using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Web.Models;
using Unison.Cloud.Web.Utils;

namespace Unison.Cloud.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly ISyncAgentRepository _agentRepository;
        private readonly ILogger<AgentsController> _logger;

        public AgentsController(ISyncAgentRepository agentRepository, ILogger<AgentsController> logger)
        {
            _agentRepository = agentRepository;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AgentDto>> Get()
        {
            IEnumerable<SyncAgent> entities = _agentRepository.GetAll();
            return Ok(entities.Select(a => a.ToHttpModel()));
        }

        [HttpGet("{id}")]
        public ActionResult<AgentDto> Get(int id)
        {
            SyncAgent agent = _agentRepository.Find(id);
            return Ok(agent.ToHttpModel());
        }

        [HttpPost]
        public ActionResult Post(AgentDto agent)
        {
            _agentRepository.Add(agent.ToDbModel());
            _agentRepository.Save();
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, AgentDto agent)
        {
            SyncAgent existingAgent = _agentRepository.Find(id);

            if (existingAgent == null)
                return NotFound();

            existingAgent.NodeId = agent.NodeId;
            existingAgent.UpdatedAt = DateTime.Now;

            _agentRepository.Save();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            SyncAgent agent = _agentRepository.Find(id);

            if (agent == null)
                return NotFound();

            _agentRepository.Remove(agent);
            _agentRepository.Save();
            return Ok();
        }
    }
}
