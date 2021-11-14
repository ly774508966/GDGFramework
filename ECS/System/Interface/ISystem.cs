using System.Collections;
using System.Collections.Generic;
using GDG.Base;
using UnityEngine;

namespace GDG.ECS
{
    public interface ISystem : IStartable,IEnable,IUpdatable,IDisable
    {   
        List<Entity> Entities{ get;}
        //Dictionary<ulong, string> m_Index2EventMapping{ get; }
        // Dictionary<string,List<ulong>> m_Event2IndexListMapping{ get;}
        // Dictionary<int, bool> m_SelectId2CanBeExcutedMapping { get; }
        Dictionary<int, ExcuteInfo> m_SelectId2ExcuteInfo { get; }
        Dictionary<ulong, List<ExcuteInfo>> m_Index2ExcuteInfoListMapping{ get; }
        Dictionary<ExcuteInfo, List<ulong>> m_ExcuteInfo2EntityListMapping{ get; }
        void SetEntities(List<Entity> entities);
        void SetActive(bool isActived);
        bool IsActived();
        void AddEntity(Entity entity);
        bool RemoveEntity(Entity entity);
    }
}