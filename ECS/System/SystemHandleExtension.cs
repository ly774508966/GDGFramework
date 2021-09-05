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
        public static ISystemHandle<E> WithAll<E,T>(this ISystemHandle<E> handle)where E:AbsEntity where T : IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsExistComponent<T>()
            select entity;
            return handle;
        }
        public static ISystemHandle<E> WithAll<E,T1, T2>(this ISystemHandle<E> handle)where E:AbsEntity where T1 : IComponentData where T2 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsExistComponent<T1>() && entity.IsExistComponent<T2>()
            select entity;
            return handle;
        }
        public static ISystemHandle<E> WithAll<E,T1, T2, T3>(this ISystemHandle<E> handle)where E:AbsEntity where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsExistComponent<T1>() && entity.IsExistComponent<T2>() && entity.IsExistComponent<T3>()
            select entity;
            return handle;
        }
        public static ISystemHandle<E> WithAll<E,T1, T2, T3, T4>(this ISystemHandle<E> handle)where E:AbsEntity where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsExistComponent<T1>() && entity.IsExistComponent<T2>() && entity.IsExistComponent<T3>() && entity.IsExistComponent<T4>()
            select entity;
            return handle;
        }
        public static ISystemHandle<E> WithNone<E,T>(this ISystemHandle<E> handle)where E:AbsEntity where T : IComponentData
        {
            handle.result =
            from entity in handle.result
            where !entity.IsExistComponent<T>()
            select entity;
            return handle;
        }
        public static ISystemHandle<E> WithNone<E,T1, T2>(this ISystemHandle<E> handle)where E:AbsEntity where T1 : IComponentData where T2 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where !entity.IsExistComponent<T1>() && !entity.IsExistComponent<T2>()
            select entity;
            return handle;
        }
        public static ISystemHandle<E> WithNone<E,T1, T2, T3>(this ISystemHandle<E> handle)where E:AbsEntity where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where !entity.IsExistComponent<T1>() && !entity.IsExistComponent<T2>() && !entity.IsExistComponent<T3>()
            select entity;
            return handle;
        }
        public static ISystemHandle<E> WithNone<E,T1, T2, T3, T4>(this ISystemHandle<E> handle)where E:AbsEntity where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where !entity.IsExistComponent<T1>() && !entity.IsExistComponent<T2>() && !entity.IsExistComponent<T3>() && !entity.IsExistComponent<T4>()
            select entity;
            return handle;
        }
        public static ISystemHandle<E> WithAny<E,T1, T2>(this ISystemHandle<E> handle)where E:AbsEntity where T1 : IComponentData where T2 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsExistComponent<T1>() | entity.IsExistComponent<T2>()
            select entity;
            return handle;
        }
        public static ISystemHandle<E> WithAny<E,T1, T2, T3>(this ISystemHandle<E> handle)where E:AbsEntity where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsExistComponent<T1>() | entity.IsExistComponent<T2>() | entity.IsExistComponent<T3>()
            select entity;
            return handle;
        }
        public static ISystemHandle<E> WithAny<E,T1, T2, T3, T4>(this ISystemHandle<E> handle)where E:AbsEntity where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData
        {
            handle.result =
            from entity in handle.result
            where entity.IsExistComponent<T1>() | entity.IsExistComponent<T2>() | entity.IsExistComponent<T3>() | entity.IsExistComponent<T4>()
            select entity;
            return handle;
        }
        public static ISystemHandle<E> ReturnQueryResult<E>(this ISystemHandle<E> handle,out IEnumerable<AbsEntity> result)where E:AbsEntity
        {
            result = handle.result;
            return handle;
        }
        public static void Excute<E>(this ISystemHandle<E> handle, float secondTime)where E:AbsEntity
        {
            BaseWorld.Instance.monoWorld.StartTimer(handle.Excute, secondTime);
        }
        public static ISystemHandle<E> WithEventHandle<E>(this ISystemHandle<E> handle, string eventName)where E:AbsEntity
        {
            handle.eventName = eventName;
            return handle;
        }
        public static bool IsExistComponent<T>(this AbsEntity entity)where T:IComponentData
        {
            foreach (var item in entity.Components)
            {
                if (item is T)
                    return true;
            }
            return false;
        }
    }
}
