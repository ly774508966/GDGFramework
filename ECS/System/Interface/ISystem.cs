using System.Collections;
using System.Collections.Generic;
using GDG.Base;
using UnityEngine;

namespace GDG.ECS
{
    public interface ISystem : IStartable,IEnable,IUpdatable,IDisable
    {   
        /// <summary>
        /// 所有被激活的实体集合
        /// </summary>
        List<Entity> Entities{ get;}
        /// <summary>
        /// 设置可遍历的实体集合
        /// </summary>
        void SetEntities(List<Entity> entities);
        /// <summary>
        /// 设置System的激活状态
        /// </summary>
        void SetActive(bool isActived);
        /// <summary>
        /// 是否处于被激活状态
        /// </summary>
        bool IsActived();
        /// <summary>
        /// 向Entities添加实体
        /// </summary>
        void AddEntity(Entity entity);
        /// <summary>
        /// 向Entities移除实体
        /// </summary>
        bool RemoveEntity(Entity entity);
        bool TryGetExcuteInfos(ulong index, out List<int> excuteInfos);
        bool TryGetExcuteInfo(int selectedId, out ExcuteInfo excuteInfo);
        void AddSelectId2ExcuteInfoMapping(int selectedId, ExcuteInfo excuteInfo);
        bool RemoveSelectId2ExcuteInfoMapping(int selectedId);
        void AddEntity2SelectIdMapping(ulong index,List<int> excuteInfos);
        bool RemoveEntity2ExcuteInfosMapping(ulong index);
    }
}