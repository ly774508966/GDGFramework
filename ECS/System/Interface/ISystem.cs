using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ECS
{
    public interface ISystem : IStartable,IEnable,IUpdatable,IDisable
    {
        int CurrentSelectId{ get; set; }
        int MaxSelectId{ get; set; }
        List<Entity> Entities{ get;}
        Dictionary<int, bool> m_SelectId2CanBeExcutedMapping { get; }
        Dictionary<ulong,double> m_Index2TimeHandleMapping{ get;}
        Dictionary<ulong,ulong> m_Index2FrameHandleMapping{ get;}        void SetEntities(List<Entity> entities);
        void SetActive(bool isActived);
        bool IsActived();
        void AddEntity(Entity entity);
        bool RemoveEntity(Entity entity);
    }
}