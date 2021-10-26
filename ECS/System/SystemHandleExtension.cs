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
        /// <summary>
        /// 返回查询到的实体
        /// </summary>
        public static AbsSystemHandle<E> ReturnQueryResult<E>(this AbsSystemHandle<E> handle,out IEnumerable<Entity> result)where E:Entity
        {
            result = handle.result;
            return handle;
        }
        /// <summary>
        /// 查询结果中的 entity 将每隔 secondTime 秒执行
        /// </summary>
        /// <param name="secondTime">延迟执行的时间，单位为秒</param>
        /// <param name="selectId">指定的 Select 的唯一id，不能与其他 SelectId 重复，且不能为 int.MinValue</param>
        public static void ExcuteDelayTime<E>(this AbsSystemHandle<E> handle, float secondTime,int selectId)where E:Entity
        {
            if (handle.system.m_SelectId2ExcuteInfo.TryGetValue(selectId, out ExcuteInfo _excuteInfo))
            {
                if (!_excuteInfo.isTimeRegister)
                {
                    _excuteInfo.excuteTime = GDGTools.Timer.CurrentTime + secondTime;
                    _excuteInfo.delayTime = secondTime;
                }
                handle.excuteInfo = _excuteInfo;
            }
            else
                handle.excuteInfo.selectId = selectId;
            handle.Excute();
        }
        /// <summary>
        /// 查询结果中的 entity 将每隔 frame 帧执行
        /// </summary>
        /// <param name="frame">延迟执行帧数</param>
        /// <param name="selectId">指定的 Select 的唯一id，不能与其他 SelectId 重复，且不能为 int.MinValue</param>
        public static void ExcuteDelayFrame<E>(this AbsSystemHandle<E> handle, ushort frame,int selectId)where E:Entity
        {

            if (handle.system.m_SelectId2ExcuteInfo.TryGetValue(selectId, out ExcuteInfo _excuteInfo))
            {
                if(!_excuteInfo.isFrameRegister)
                {
                    _excuteInfo.excuteFrame = GDGTools.Timer.CurrentFrame + frame;
                    _excuteInfo.delayFrame = frame;
                }
                handle.excuteInfo = _excuteInfo;
            }
            else
                handle.excuteInfo.selectId = selectId;
            handle.Excute();
        }
        /// <summary>
        /// 直到通过 EventCenter 注册的事件被触发，才会调用Excute方法
        /// </summary>
        /// <param name="eventName">事件名称</param>///
        /// <param name="selectId">指定的 Select 的唯一id，不能与其他 SelectId 重复，且不能为 int.MinValue</param>/// 
        public static void ExcuteWithEvent<E>(this AbsSystemHandle<E> handle, string eventName,int selectId)where E:Entity
        {
            if (handle.system.m_SelectId2ExcuteInfo.TryGetValue(selectId, out ExcuteInfo _excuteInfo))
            {
                _excuteInfo.eventName = eventName;
                handle.excuteInfo = _excuteInfo;
            }
            else
                handle.excuteInfo.selectId = selectId;
            handle.Excute();
        }
        /// <summary>
        /// 是否允许执行
        /// </summary>
        public static AbsSystemHandle<E> CanBeEexcuted<E>(this AbsSystemHandle<E> handle, bool canBeExcuted)where E:Entity
        {
            handle.excuteInfo.canBeExcuted = canBeExcuted;
            return handle;
        }
    }
}
