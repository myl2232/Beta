using GameFramework;
using GameFramework.DataTable;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public static class EntityExtension
    {
        // 关于 EntityId 的约定：
        // 0 为无效
        // 正值用于和服务器通信的实体（如玩家角色、NPC、怪等，服务器只产生正值）
        // 负值用于本地生成的临时实体（如特效、FakeObject等）
        private static int s_SerialId = 0;

        public static EntityObject GetGameEntity(this EntityComponent entityComponent, int entityId)
        {
            UnityGameFramework.Runtime.Entity entity = entityComponent.GetEntity(entityId);
            if (entity == null)
            {
                return null;
            }

            return (EntityObject)entity.Logic;
        }

        public static void HideEntity(this EntityComponent entityComponent, EntityObject entity)
        {
            entityComponent.HideEntity(entity.Entity);
        }

        public static void AttachEntity(this EntityComponent entityComponent, EntityObject entity, int ownerId, string parentTransformPath = null, object userData = null)
        {
            entityComponent.AttachEntity(entity.Entity, ownerId, parentTransformPath, userData);
        }

        public static void ShowNPC(this EntityComponent entityComponent, NPCData data)
        {
            entityComponent.ShowEntity(typeof(NPC), "NPC", data);
        }

        public static void ShowEnemy(this EntityComponent entityComponent, NPCData data)
        {
            entityComponent.ShowEntity(typeof(Enemy), "Enemy", data);
        }

        public static void ShowEthan(this EntityComponent entityComponent, EthanData data)
		{
            entityComponent.ShowEntity(typeof(Ethan), "Ethan", data);
		}

        public static void ShowAttachment(this EntityComponent entityComponent, AttachmentData data)
        {
            if (data == null)
            {
                Log.Warning("Data is invalid.");
                return;
            }

            IDataTable<DRAttachment> dtEntity = GameEntry.DataTable.GetDataTable<DRAttachment>();
            DRAttachment drEntity = dtEntity.GetDataRow(data.TypeId);
            if (drEntity == null)
            {
                Log.Warning("Can not load entity id '{0}' from data table.", data.TypeId.ToString());
                return;
            }

        }

        public static void ShowStructure(this EntityComponent entityComponent, StructureData data)
        {
            entityComponent.ShowEntity(typeof(Structure), "Structure", data);
        }
        

        public static void ShowAvatar(this EntityComponent entityComponent, AvatarData data)
        {
            if (data == null)
            {
                Log.Warning("Data is invalid.");
                return;
            }

            IDataTable<DRAvatar> dtEntity = GameEntry.DataTable.GetDataTable<DRAvatar>();
            DRAvatar drEntity = dtEntity.GetDataRow(data.TypeId);
            if (drEntity == null)
            {
                Log.Warning("Can not load entity id '{0}' from data table.", data.TypeId.ToString());
                return;
            }
            data.Skeleton = drEntity.Data.Skeleton;
            data.Parts = drEntity.Data.Parts;
            entityComponent.ShowEntity(data.Id, typeof(AvatarEntity), drEntity.Data.Skeleton, "Avatar", data);
        }

        private static void ShowEntity(this EntityComponent entityComponent, Type logicType, string entityGroup, EntityData data)
        {
            if (data == null)
            {
                Log.Warning("Data is invalid.");
                return;
            }

            IDataTable<DREntity> dtEntity = GameEntry.DataTable.GetDataTable<DREntity>();
            DREntity drEntity = dtEntity.GetDataRow(data.TypeId);
            if (drEntity == null)
            {
                Log.Warning("Can not load entity id '{0}' from data table.", data.TypeId.ToString());
                return;
            }

            entityComponent.ShowEntity(data.Id, logicType, AssetUtility.GetEntityAsset(drEntity.AssetName), entityGroup, data);
        }

        public static int GenerateSerialId(this EntityComponent entityComponent)
        {
            return --s_SerialId;
        }
    }
}
