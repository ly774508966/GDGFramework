using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ECS
{
    public interface IEntityCreateable
    {
        AbsEntity CreateEntity(uint typeID , Action<AbsEntity> initCallback = null , Action<AbsEntity> enableCallback = null);
    }
}