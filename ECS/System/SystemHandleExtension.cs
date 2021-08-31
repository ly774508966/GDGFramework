using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GDG.ModuleManager;
using UnityEngine.Events;

namespace GDG.ECS
{
    public static class SystemHandleExtension
    {
        public static SystemHandle WithAll<T>(this SystemHandle handle) where T : IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsExistComponent<T>()
            select entity;
            return handle;
        }
        public static SystemHandle WithAll<T1, T2>(this SystemHandle handle) where T1 : IComponentData where T2 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsExistComponent<T1>() && entity.IsExistComponent<T2>()
            select entity;
            return handle;
        }
        public static SystemHandle WithAll<T1, T2, T3>(this SystemHandle handle) where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsExistComponent<T1>() && entity.IsExistComponent<T2>() && entity.IsExistComponent<T3>()
            select entity;
            return handle;
        }
        public static SystemHandle WithAll<T1, T2, T3, T4>(this SystemHandle handle) where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsExistComponent<T1>() && entity.IsExistComponent<T2>() && entity.IsExistComponent<T3>() && entity.IsExistComponent<T4>()
            select entity;
            return handle;
        }
        public static SystemHandle WithNone<T>(this SystemHandle handle) where T : IComponentData
        {
            handle.result =
            from entity in handle.result
            where !entity.IsExistComponent<T>()
            select entity;
            return handle;
        }
        public static SystemHandle WithNone<T1, T2>(this SystemHandle handle) where T1 : IComponentData where T2 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where !entity.IsExistComponent<T1>() && !entity.IsExistComponent<T2>()
            select entity;
            return handle;
        }
        public static SystemHandle WithNone<T1, T2, T3>(this SystemHandle handle) where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where !entity.IsExistComponent<T1>() && !entity.IsExistComponent<T2>() && !entity.IsExistComponent<T3>()
            select entity;
            return handle;
        }
        public static SystemHandle WithNone<T1, T2, T3, T4>(this SystemHandle handle) where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where !entity.IsExistComponent<T1>() && !entity.IsExistComponent<T2>() && !entity.IsExistComponent<T3>() && !entity.IsExistComponent<T4>()
            select entity;
            return handle;
        }
        public static SystemHandle ReturnQueryResult(this SystemHandle handle,out IEnumerable<AbsEntity> result)
        {
            result = handle.result;
            return handle;
        }
        public static void Excute(this SystemHandle handle, float secondTime)
        {
            BaseWorld.Instance.monoWorld.StartTimer(handle.Excute, secondTime);
        }
        public static SystemHandle WithEventHandle(this SystemHandle handle, string eventName)
        {
            handle.eventName = eventName;
            return handle;
        }
        public static bool IsExistComponent<T>(this AbsEntity entity)
        {
            foreach (var item in entity.Components)
            {
                if (item is T)
                    return true;
            }
            return false;
        }
        internal static IComponentData ComponentMatch(this AbsEntity entity, IComponentData component)
        {
            foreach (var item in entity.Components)
            {
                if (item.GetType() == component.GetType())
                {
                    component = item;
                    return component;
                }
            }
            return null;
        }
    }
}
