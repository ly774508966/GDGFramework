using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ECS
{
    public interface ISystem : IStartable,IEnable,IUpdatable,IDisable
    {
        List<AbsEntity> Entities{ get;}
        void SetEntities(List<AbsEntity> entities);
        void SetActive(bool isActived);
        bool IsActived();
    }
}