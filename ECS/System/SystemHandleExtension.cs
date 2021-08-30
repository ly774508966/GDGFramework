using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GDG.ECS
{
    public static class SystemHandleExtension
    {
        public static SystemHandle WithAll<T>(this SystemHandle handle)where T: IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsMatchComponent<T>()
            select entity;
            return handle;
        }
        public static SystemHandle WithAll<T1,T2>(this SystemHandle handle)where T1: IComponentData where T2: IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsMatchComponent<T1>()&&entity.IsMatchComponent<T2>()
            select entity;
            return handle;
        }
        public static SystemHandle WithAll<T1,T2,T3>(this SystemHandle handle)where T1: IComponentData where T2: IComponentData where T3:IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsMatchComponent<T1>()&&entity.IsMatchComponent<T2>()&&entity.IsMatchComponent<T3>()
            select entity;
            return handle;
        }
        public static SystemHandle WithAll<T1,T2,T3,T4>(this SystemHandle handle)where T1: IComponentData where T2: IComponentData where T3:IComponentData where T4:IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsMatchComponent<T1>()&&entity.IsMatchComponent<T2>()&&entity.IsMatchComponent<T3>()&&entity.IsMatchComponent<T4>()
            select entity;
            return handle;
        }
        
        public static bool IsMatchComponent<T>(this AbsEntity entity)
        {
            foreach(var item in entity.Components)
            {
                if(item is T)
                    return true;
            }
            return false;
        }
        public static IComponentData ComponentMatch(this AbsEntity entity,IComponentData component)
        {
            foreach(var item in entity.Components)
            {
                if(item.GetType()==component.GetType())
                {
                    component = item;
                    return component;
                }
            }
            return null;
        }
    }
}
