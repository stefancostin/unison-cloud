using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Models;
using Unison.Cloud.Web.Models;

namespace Unison.Cloud.Web.Utilities
{
    public static class HttpMapper
    {
        #region Agent Mapping
        public static SyncAgent ToDbModel(this AgentDto agentDto)
        {
            return new SyncAgent()
            {
                Id = agentDto.Id,
                InstanceId = agentDto.InstanceId,
                NodeId = agentDto.Node.Id,
            };
        }

        public static AgentDto ToHttpModel(this SyncAgent syncAgent)
        {
            return new AgentDto()
            {
                Id = syncAgent.Id,
                InstanceId = syncAgent.InstanceId,
                Node = syncAgent.Node?.ToHttpModel(),
            };
        }
        #endregion

        #region Entity Mapping
        public static SyncEntity ToDbModel(this EntityDto entityDto)
        {
            return new SyncEntity()
            {
                Id = entityDto.Id,
                NodeId = entityDto.Node.Id,
                Entity = entityDto.Entity,
                PrimaryKey = entityDto.PrimaryKey ?? "Id",
                Fields = entityDto.Fields
            };
        }

        public static EntityDto ToHttpModel(this SyncEntity syncEntity)
        {
            return new EntityDto()
            {
                Id = syncEntity.Id,
                Node = syncEntity.Node?.ToHttpModel(),
                Entity = syncEntity.Entity,
                PrimaryKey = syncEntity.PrimaryKey,
                Fields = syncEntity.Fields
            };
        }
        #endregion

        #region Node Mapping
        public static SyncNode ToDbModel(this NodeDto nodeDto)
        {
            return new SyncNode()
            {
                Id = nodeDto.Id,
                Name = nodeDto.Name,
                Description = nodeDto.Description
            };
        }

        public static NodeDto ToHttpModel(this SyncNode syncNode)
        {
            return new NodeDto()
            {
                Id = syncNode.Id,
                Name = syncNode.Name,
                Description = syncNode.Description,
                Agents = syncNode.Agents == null ? new List<string>() : syncNode.Agents.Select(a => a.InstanceId).ToList()
            };
        }
        #endregion

        #region Log Mapping
        public static LogDto ToHttpModel(this SyncLog syncLog)
        {
            return new LogDto()
            {
                Id = syncLog.Id,
                CorrelationId = syncLog.CorrelationId,
                Agent = syncLog.Agent?.ToHttpModel(),
                Node = syncLog.Agent?.Node?.ToHttpModel(),
                Entity = syncLog.Entity,
                AddedRecords = syncLog.AddedRecords,
                UpdatedRecords = syncLog.UpdatedRecords,
                DeletedRecords = syncLog.DeletedRecords,
                Completed = syncLog.Completed,
                Date = syncLog.CreatedAt,
            };
        }
        #endregion
    }
}
