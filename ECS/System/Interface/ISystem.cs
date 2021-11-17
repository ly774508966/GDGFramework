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
        /// SelectId到执行信息的映射
        /// </summary>
        Dictionary<int, ExcuteInfo> m_SelectId2ExcuteInfo { get; }
        /// <summary>
        /// 实体到执行信息的映射
        /// </summary>
        Dictionary<ulong, List<ExcuteInfo>> m_Index2ExcuteInfoListMapping{ get; }
        /// <summary>
        /// 执行信息到实体的映射
        /// </summary>
        Dictionary<ExcuteInfo, List<ulong>> m_ExcuteInfo2EntityListMapping{ get; }
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
    }
}