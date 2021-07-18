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
using Unison.Cloud.Web.Utils;

namespace Unison.Cloud.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntitiesController : ControllerBase
    {
        private readonly ISyncEntityRepository _entityRepository;
        private readonly ILogger<EntitiesController> _logger;

        public EntitiesController(ISyncEntityRepository entityRepository, ILogger<EntitiesController> logger)
        {
            _entityRepository = entityRepository;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<EntityDto>> Get()
        {
            IEnumerable<SyncEntity> entities = _entityRepository.GetAll();
            return Ok(entities.Select(e => e.ToHttpModel()));
        }

        [HttpGet("{id}")]
        public ActionResult<EntityDto> Get(int id)
        {
            SyncEntity entity = _entityRepository.Find(id);
            return Ok(entity.ToHttpModel());
        }

        [HttpPost]
        public ActionResult Post(EntityDto entity)
        {
            ValidateRequest(entity);
            _entityRepository.Add(entity.ToDbModel());
            _entityRepository.Save();
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, EntityDto entity)
        {
            ValidateRequest(entity);

            SyncEntity existingEntity = _entityRepository.Find(id);

            if (existingEntity == null)
                return NotFound();

            MapFieldsToDbModel(entity, existingEntity);

            _entityRepository.Save();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            SyncEntity entity = _entityRepository.Find(id);

            if (entity == null)
                return NotFound();

            _entityRepository.Remove(entity);
            _entityRepository.Save();
            return Ok();
        }

        private void MapFieldsToDbModel(in EntityDto requestEntity, in SyncEntity existingEntity)
        {
            existingEntity.NodeId = requestEntity.Node.Id;
            existingEntity.Entity = requestEntity.Entity;
            existingEntity.PrimaryKey = requestEntity.PrimaryKey;
            existingEntity.Fields = requestEntity.Fields;
            existingEntity.UpdatedAt = DateTime.Now;
        }

        public void ValidateRequest(EntityDto entity)
        {
            if (entity.Node == null || entity.Node.Id == 0)
                throw new InvalidRequestException("Entity's node cannot be null or missing an id");
        }
    }
}
