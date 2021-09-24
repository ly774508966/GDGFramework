using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ECS
{
    public interface IEntityCreateable
    {
        Entity CreateEntity(uint typeID);
    }
}