using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GDG.ModuleManager;
using UnityEngine.Events;
using GDG.Utils;

namespace GDG.ECS
{
    public static class SystemHandleExtension
    {
        public static AbsSystemHandle<E> WithAll<E,T>(this AbsSystemHandle<E> handle)where E:Entity where T : IComponent
        {
            handle.result =
            from entity in handle.result.AsParallel()
            where entity.IsExistComponent<T>()
            select entity;
            return handle;
        }
        public static AbsSystemHandle<E> WithAll<E,T1, T2>(this AbsSystemHandle<E> handle)where E:Entity where T1 : IComponent where T2 : IComponent
        {
            handle.result =
            from entity in handle.result.AsParallel()
            where entity.IsExistComponent<T1>() && entity.IsExistComponent<T2>()
            select entity;
            return handle;
        }
        public static AbsSystemHandle<E> WithAll<E,T1, T2, T3>(this AbsSystemHandle<E> handle)where E:Entity where T1 : IComponent where T2 : IComponent where T3 : IComponent
        {
            handle.result =
            from entity in handle.result.AsParallel()
            where entity.IsExistComponent<T1>() && entity.IsExistComponent<T2>() && entity.IsExistComponent<T3>()
            select entity;
            return handle;
        }
        public static AbsSystemHandle<E> WithAll<E,T1, T2, T3, T4>(this AbsSystemHandle<E> handle)where E:Entity where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
        {
            handle.result =
            from entity in handle.result.AsParallel()
            where entity.IsExistComponent<T1>() && entity.IsExistComponent<T2>() && entity.IsExistComponent<T3>() && entity.IsExistComponent<T4>()
            select entity;
            return handle;
        }
        public static AbsSystemHandle<E> WithNone<E,T>(this AbsSystemHandle<E> handle)where E:Entity where T : IComponent
        {
            handle.result =
            from entity in handle.result.AsParallel()
            where !entity.IsExistComponent<T>()
            select entity;
            return handle;
        }
        public static AbsSystemHandle<E> WithNone<E,T1, T2>(this AbsSystemHandle<E> handle)where E:Entity where T1 : IComponent where T2 : IComponent
        {
            handle.result =
            from entity in handle.result.AsParallel()
            where !entity.IsExistComponent<T1>() && !entity.IsExistComponent<T2>()
            select entity;
            return handle;
        }
        public static AbsSystemHandle<E> WithNone<E,T1, T2, T3>(this AbsSystemHandle<E> handle)where E:Entity where T1 : IComponent where T2 : IComponent where T3 : IComponent
        {
            handle.result =
            from entity in handle.result.AsParallel()
            where !entity.IsExistComponent<T1>() && !entity.IsExistComponent<T2>() && !entity.IsExistComponent<T3>()
            select entity;
            return handle;
        }
        public static AbsSystemHandle<E> WithNone<E,T1, T2, T3, T4>(this AbsSystemHandle<E> handle)where E:Entity where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
        {
            handle.result =
            from entity in handle.result.AsParallel()
            where !entity.IsExistComponent<T1>() && !entity.IsExistComponent<T2>() && !entity.IsExistComponent<T3>() && !entity.IsExistComponent<T4>()
            select entity;
            return handle;
        }
        public static AbsSystemHandle<E> WithAny<E,T1, T2>(this AbsSystemHandle<E> handle)where E:Entity where T1 : IComponent where T2 : IComponent
        {
            handle.result =
            from entity in handle.result.AsParallel()
            where entity.IsExistComponent<T1>() | entity.IsExistComponent<T2>()
            select entity;
            return handle;
        }
        public static AbsSystemHandle<E> WithAny<E,T1, T2, T3>(this AbsSystemHandle<E> handle)where E:Entity where T1 : IComponent where T2 : IComponent where T3 : IComponent
        {
            handle.result =
            from entity in handle.result.AsParallel()
            where entity.IsExistComponent<T1>() | entity.IsExistComponent<T2>() | entity.IsExistComponent<T3>()
            select entity;
            return handle;
        }
        public static AbsSystemHandle<E> WithAny<E,T1, T2, T3, T4>(this AbsSystemHandle<E> handle)where E:Entity where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
        {
            handle.result =
            from entity in handle.result.AsParallel()
            where entity.IsExistComponent<T1>() | entity.IsExistComponent<T2>() | entity.IsExistComponent<T3>() | entity.IsExistComponent<T4>()
            select entity;
            return handle;
        }
        public static AbsSystemHandle<E> ReturnQueryResult<E>(this AbsSystemHandle<E> handle,out IEnumerable<Entity> result)where E:Entity
        {
            result = handle.result;
            return handle;
        }
        /// <summary>
        /// 每隔secondTime秒执行
        /// </summary>
        public static void ExcuteDelayTime<E>(this AbsSystemHandle<E> handle, float secondTime)where E:Entity
        {
            var excuteTime = GDGTools.Timer.GetCurrentTime() + GDGTools.Timer.TimeUnitToMillisecond(secondTime, TimeUnit.Second);

            foreach(var item in handle.result)
            {
                if (!handle.system.m_Index2TimeHandleMapping.TryGetValue(item.Index, out double extime))
                {
                    handle.system.m_Index2TimeHandleMapping.Add(item.Index, excuteTime);
                    GDGTools.Timer.DelayTimeExcute(secondTime, () =>
                     {
                         handle.Excute();
                         handle.system.m_Index2TimeHandleMapping.Remove(item.Index);
                     });
                }
            }
        }
        /// <summary>
        /// 每隔frame帧执行
        /// </summary>
        public static void ExcuteDelayFrame<E>(this AbsSystemHandle<E> handle, ushort frame)where E:Entity
        {
            var excuteFrame = GDGTools.Timer.CurrentFrame + frame;

            foreach(var item in handle.result)
            {
                if (!handle.system.m_Index2FrameHandleMapping.TryGetValue(item.Index, out ulong exframe))
                {
                    handle.system.m_Index2FrameHandleMapping.Add(item.Index, excuteFrame);
                    GDGTools.Timer.DelayFrameExcute(frame, () =>
                     {
                         handle.Excute();
                         handle.system.m_Index2TimeHandleMapping.Remove(item.Index);
                     });
                }
            }
        }
        public static AbsSystemHandle<E> WithEventHandle<E>(this AbsSystemHandle<E> handle, string eventName)where E:Entity
        {
            handle.eventName = eventName;
            return handle;
        }
        public static bool IsExistComponent<T>(this Entity entity)where T:IComponent
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
