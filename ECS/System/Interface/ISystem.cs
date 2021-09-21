using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ECS
{
    public interface ISystem : IStartable,IEnable,IUpdatable,IDisable
    {
        List<AbsEntity> Entities{ get;}
        Dictionary<string,List<ulong>> m_Event2IndexListMapping{ get;}
        Dictionary<ulong,double> m_Index2TimeHandleMapping{ get;}
        Dictionary<ulong,ulong> m_Index2FrameHandleMapping{ get;}
        Dictionary<ulong, string> m_Index2EventMapping{ get; }
        void SetEntities(List<AbsEntity> entities);
        void SetActive(bool isActived);
        bool IsActived();
        void AddEntity(AbsEntity entity);
        bool RemoveEntity(AbsEntity entity);
    }
}