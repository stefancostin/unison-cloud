using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ISyncLogRepository _logsRepository;
        private readonly ILogger<LogsController> _logger;

        public LogsController(ISyncLogRepository logsRepository, ILogger<LogsController> logger)
        {
            _logsRepository = logsRepository;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<LogDto>> Get()
        {
            IEnumerable<SyncLog> logs = _logsRepository.GetAll();
            return Ok(logs.Select(l => l.ToHttpModel()));
        }

        [HttpGet("{id}")]
        public ActionResult<LogDto> Get(int id)
        {
            SyncLog log = _logsRepository.Find(id);

            if (log == null)
                return NotFound();

            return Ok(log.ToHttpModel());
        }
    }
}
