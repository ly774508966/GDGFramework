using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ECS
{
    public interface ISystem : IStartable,IEnable,IUpdatable,IDisable
    {
        List<AbsEntity> Entities{ get;}
        Dictionary<string,List<ulong>> m_EventHandle2IndexMapping{ get;}

        void SetEntities(List<AbsEntity> entities);
        void SetActive(bool isActived);
        bool IsActived();
    }
}