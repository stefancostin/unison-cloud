﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Web.Models;

namespace Unison.Cloud.Web.Utils
{
    public static class HttpMapper
    {
        public static SyncEntity ToDbModel(this EntityDto entityDto)
        {
            return new SyncEntity()
            {
                Id = entityDto.Id,
                NodeId = entityDto.NodeId,
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
                NodeId = syncEntity.NodeId,
                Entity = syncEntity.Entity,
                PrimaryKey = syncEntity.PrimaryKey,
                Fields = syncEntity.Fields
            };
        }
    }
}