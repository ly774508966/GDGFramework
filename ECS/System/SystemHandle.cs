using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace GDG.ECS
{
    public class SystemHandle:IExcutable
    {
        public IEnumerable<AbsEntity> result;
        public Action<AbsEntity> callback;
        public IComponentData[] components;
        public void Excute()
        {
            if(callback == null || result==null)
                return;
            if(components==null && result!=null)
            {
                foreach (var entity in result)
                {
                    callback(entity);
                }
                return;
            }
            foreach (var entity in result)
            {
                foreach(var component in components)
                {
                    entity.ComponentMatch(component);
                }
                callback(entity);
            }
        }
    }
}
