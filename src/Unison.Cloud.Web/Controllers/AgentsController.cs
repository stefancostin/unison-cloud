using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Exceptions;
using Unison.Cloud.Core.Interfaces.Data;
using Unison.Cloud.Web.Models;
using Unison.Cloud.Web.Utilities;

namespace Unison.Cloud.Web.Controllers
{
    [Authorize]
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

            if (agent == null)
                return NotFound();

            return Ok(agent.ToHttpModel());
        }

        [HttpPost]
        public ActionResult Post(AgentDto agent)
        {
            ValidateRequest(agent);
            _agentRepository.Add(agent.ToDbModel());
            _agentRepository.Save();
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, AgentDto agent)
        {
            ValidateRequest(agent);

            SyncAgent existingAgent = _agentRepository.Find(id);

            if (existingAgent == null)
                return NotFound();

            existingAgent.InstanceId = agent.InstanceId;
            existingAgent.NodeId = agent.Node.Id;
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

        public void ValidateRequest(AgentDto agent)
        {
            if (agent.Node == null || agent.Node.Id == 0)
                throw new InvalidRequestException("Agent's node cannot be null or missing an id");

            if (string.IsNullOrWhiteSpace(agent.InstanceId))
                throw new InvalidRequestException("Agent's instance id cannot be null or empty");

            SyncAgent existingInstance = _agentRepository.FindByInstanceId(agent.InstanceId);

            if (existingInstance != null && existingInstance.Id != agent.Id)
                throw new InvalidRequestException("Agent's instance id is duplicated");
        }
    }
}
