using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using GDG.ModuleManager;
using GDG.Utils;
using GDG.Base;

namespace GDG.ECS
{
    #region SystemCallback
    public delegate void SystemCallback(Entity entity);
    public delegate void SystemCallback<T>(Entity entity, T c) where T : class, IComponent;
    public delegate void SystemCallback<T1, T2>(Entity entity, T1 c1, T2 c2) where T1 : class, IComponent where T2 : class, IComponent;
    public delegate void SystemCallback<T1, T2, T3>(Entity entity, T1 c1, T2 c2, T3 c3) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent;
    public delegate void SystemCallback<T1, T2, T3, T4>(Entity entity, T1 c1, T2 c2, T3 c3, T4 c4) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent;
    public delegate void SystemCallback<T1, T2, T3, T4, T5>(Entity entity, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent;
    public delegate void SystemCallback<T1, T2, T3, T4, T5, T6>(Entity entity, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent;
    public delegate void SystemCallback<T1, T2, T3, T4, T5, T6, T7>(Entity entity, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent;
    public delegate void SystemCallback<T1, T2, T3, T4, T5, T6, T7, T8>(Entity entity, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7, T8 c8) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent where T8 : class, IComponent;
    #endregion
    #region SystemHandle
    public abstract class SystemHandleBase : IExcutable
    {
        internal List<Entity> result;
        internal ExcuteInfo excuteInfo = new ExcuteInfo();
        internal ISystem system;
        public abstract void Excute();
    }
    public class SystemHandle : SystemHandleBase
    {
        public static SystemHandle None = new SystemHandle() { };
        public SystemCallback callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count() == 0)
            {
                ObjectPool<SystemHandle>.Instance.Push(this);
                return;
            }

            for (int index = 0; index < result.Count(); index++)
            {
                if (excuteInfo.selectId != int.MinValue)
                    CallbackExcute(result[index], excuteInfo, system, callback);
                else
                    callback(result[index]);
            }
            ObjectPool<SystemHandle>.Instance.Push(this);
        }
        void CallbackExcute(Entity entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback callback)
        {

            if (!system.TryGetExcuteInfo(excuteInfo.selectId, out ExcuteInfo _excuteInfo))
            {
                _excuteInfo = excuteInfo;
                system.AddSelectId2ExcuteInfoMapping(excuteInfo.selectId, _excuteInfo);
            }
            //对于不同SelectId：
            //如果该实体已经注册过该ExcuteInfo则返回
            if (system.TryGetExcuteInfos(entity.Index, out List<int> checkList))
            {
                if (checkList.Contains(_excuteInfo.selectId))
                    return;
                else
                    checkList.Add(_excuteInfo.selectId);
            }
            //否则进行注册
            else
            {
                checkList = new List<int>() { _excuteInfo.selectId };
                system.AddEntity2SelectIdMapping(entity.Index, checkList);
            }
            #region ExcuteInfo 处理

            //是否存在事件
            if (!string.IsNullOrEmpty(_excuteInfo.eventName))
            {
                UnityAction action = null;
                action = () =>
                {
                    if (!entity.IsActived)
                    {
                        EventManager.Instance.RemoveActionListener(_excuteInfo.eventName, action);
                        system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                    }
                    else
                        callback(entity);
                };
                EventManager.Instance.AddActionListener(_excuteInfo.eventName, action);
            }
            //是否注册了延迟时间
            else if (_excuteInfo.excuteTime != double.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayTimeExcute(_excuteInfo.delayTime, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity);
                 });
            }
            //是否注册了延迟帧
            else if (_excuteInfo.excuteFrame != ulong.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayFrameExcute(_excuteInfo.delayFrame, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity);
                 });
            }
            #endregion

        }

    }
    public class SystemHandle<T> : SystemHandleBase where T : class, IComponent
    {
        public static SystemHandle<T> None = new SystemHandle<T>() { };
        public SystemCallback<T> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count() == 0)
            {
                ObjectPool<SystemHandle<T>>.Instance.Push(this);
                return;
            }

            for (int index = 0; index < result.Count(); index++)
            {
                foreach (var component in World.EntityManager.GetComponents(result.ElementAt(index)))
                {
                    if (component is T c)
                    {
                        if (excuteInfo.selectId != int.MinValue)
                            CallbackExcute(result[index], excuteInfo, system, callback, c);
                        else
                            callback(result[index], c);
                        break;
                    }
                }
            }
            ObjectPool<SystemHandle<T>>.Instance.Push(this);
        }
        void CallbackExcute(Entity entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<T> callback, T c)
        {

            if (!system.TryGetExcuteInfo(excuteInfo.selectId, out ExcuteInfo _excuteInfo))
            {
                _excuteInfo = excuteInfo;
                system.AddSelectId2ExcuteInfoMapping(excuteInfo.selectId, _excuteInfo);
            }
            //对于不同SelectId：
            //如果该实体已经注册过该ExcuteInfo则返回
            if (system.TryGetExcuteInfos(entity.Index, out List<int> checkList))
            {
                if (checkList.Contains(_excuteInfo.selectId))
                    return;
                else
                    checkList.Add(_excuteInfo.selectId);
            }
            //否则进行注册
            else
            {
                checkList = new List<int>() { _excuteInfo.selectId };
                system.AddEntity2SelectIdMapping(entity.Index, checkList);
            }
            #region ExcuteInfo 处理

            //是否存在事件
            if (!string.IsNullOrEmpty(_excuteInfo.eventName))
            {
                UnityAction action = null;
                action = () =>
                {
                    if (!entity.IsActived)
                    {
                        EventManager.Instance.RemoveActionListener(_excuteInfo.eventName, action);
                        system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                    }
                    else
                        callback(entity, c);
                };
                EventManager.Instance.AddActionListener(_excuteInfo.eventName, action);
            }
            //是否注册了延迟时间
            else if (_excuteInfo.excuteTime != double.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayTimeExcute(_excuteInfo.delayTime, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c);
                 });
            }
            //是否注册了延迟帧
            else if (_excuteInfo.excuteFrame != ulong.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayFrameExcute(_excuteInfo.delayFrame, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c);
                 });
            }
            #endregion

        }

    }
    public class SystemHandle<T1, T2> : SystemHandleBase where T1 : class, IComponent where T2 : class, IComponent
    {
        public static SystemHandle<T1, T2> None = new SystemHandle<T1, T2>() { };
        public SystemCallback<T1, T2> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count() == 0)
                return;
            T1 t1 = null;
            T2 t2 = null;


            for (int index = 0; index < result.Count(); index++)
            {
                foreach (var component in World.EntityManager.GetComponents(result.ElementAt(index)))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                }
                if (excuteInfo.selectId != int.MinValue)
                    CallbackExcute(result[index], excuteInfo, system, callback, t1, t2);
                else
                    callback(result[index], t1, t2);
            }
            ObjectPool<SystemHandle<T1, T2>>.Instance.Push(this);
        }
        void CallbackExcute(Entity entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<T1, T2> callback, T1 c1, T2 c2)
        {

            if (!system.TryGetExcuteInfo(excuteInfo.selectId, out ExcuteInfo _excuteInfo))
            {
                _excuteInfo = excuteInfo;
                system.AddSelectId2ExcuteInfoMapping(excuteInfo.selectId, _excuteInfo);
            }
            //对于不同SelectId：
            //如果该实体已经注册过该ExcuteInfo则返回
            if (system.TryGetExcuteInfos(entity.Index, out List<int> checkList))
            {
                if (checkList.Contains(_excuteInfo.selectId))
                    return;
                else
                    checkList.Add(_excuteInfo.selectId);
            }
            //否则进行注册
            else
            {
                checkList = new List<int>() { _excuteInfo.selectId };
                system.AddEntity2SelectIdMapping(entity.Index, checkList);
            }
            #region ExcuteInfo 处理

            //是否存在事件
            if (!string.IsNullOrEmpty(_excuteInfo.eventName))
            {
                UnityAction action = null;
                action = () =>
                {
                    if (!entity.IsActived)
                    {
                        EventManager.Instance.RemoveActionListener(_excuteInfo.eventName, action);
                        system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                    }
                    else
                        callback(entity, c1, c2);
                };
                EventManager.Instance.AddActionListener(_excuteInfo.eventName, action);
            }
            //是否注册了延迟时间
            else if (_excuteInfo.excuteTime != double.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayTimeExcute(_excuteInfo.delayTime, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2);
                 });
            }
            //是否注册了延迟帧
            else if (_excuteInfo.excuteFrame != ulong.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayFrameExcute(_excuteInfo.delayFrame, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2);
                 });
            }
            #endregion
        }

    }
    public class SystemHandle<T1, T2, T3> : SystemHandleBase where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
    {
        public static SystemHandle<T1, T2, T3> None = new SystemHandle<T1, T2, T3>() { };
        public SystemCallback<T1, T2, T3> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count() == 0)
            {
                ObjectPool<SystemHandle<T1, T2, T3>>.Instance.Push(this);
                return;
            }
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;

            for (int index = 0; index < result.Count(); index++)
            {
                foreach (var component in World.EntityManager.GetComponents(result.ElementAt(index)))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                }
                if (excuteInfo.selectId != int.MinValue)
                    CallbackExcute(result[index], excuteInfo, system, callback, t1, t2, t3);
                else
                    callback(result[index], t1, t2, t3);
            }
            ObjectPool<SystemHandle<T1, T2, T3>>.Instance.Push(this);
        }
        void CallbackExcute(Entity entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<T1, T2, T3> callback, T1 c1, T2 c2, T3 c3)
        {

            if (!system.TryGetExcuteInfo(excuteInfo.selectId, out ExcuteInfo _excuteInfo))
            {
                _excuteInfo = excuteInfo;
                system.AddSelectId2ExcuteInfoMapping(excuteInfo.selectId, _excuteInfo);
            }
            //对于不同SelectId：
            //如果该实体已经注册过该ExcuteInfo则返回
            if (system.TryGetExcuteInfos(entity.Index, out List<int> checkList))
            {
                if (checkList.Contains(_excuteInfo.selectId))
                    return;
                else
                    checkList.Add(_excuteInfo.selectId);
            }
            //否则进行注册
            else
            {
                checkList = new List<int>() { _excuteInfo.selectId };
                system.AddEntity2SelectIdMapping(entity.Index, checkList);
            }
            #region ExcuteInfo 处理

            //是否存在事件
            if (!string.IsNullOrEmpty(_excuteInfo.eventName))
            {
                UnityAction action = null;
                action = () =>
                {
                    if (!entity.IsActived)
                    {
                        EventManager.Instance.RemoveActionListener(_excuteInfo.eventName, action);
                        system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                    }
                    else
                        callback(entity, c1, c2, c3);
                };
                EventManager.Instance.AddActionListener(_excuteInfo.eventName, action);
            }
            //是否注册了延迟时间
            else if (_excuteInfo.excuteTime != double.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayTimeExcute(_excuteInfo.delayTime, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2, c3);
                 });
            }
            //是否注册了延迟帧
            else if (_excuteInfo.excuteFrame != ulong.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayFrameExcute(_excuteInfo.delayFrame, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2, c3);
                 });
            }
            #endregion
        }
    }
    public class SystemHandle<T1, T2, T3, T4> : SystemHandleBase where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
    {
        public static SystemHandle<T1, T2, T3, T4> None = new SystemHandle<T1, T2, T3, T4>() { };
        public SystemCallback<T1, T2, T3, T4> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count() == 0)
            {
                ObjectPool<SystemHandle<T1, T2, T3, T4>>.Instance.Push(this);
                return;
            }

            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;


            for (int index = 0; index < result.Count(); index++)
            {
                foreach (var component in World.EntityManager.GetComponents(result.ElementAt(index)))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                }
                if (excuteInfo.selectId != int.MinValue)
                    CallbackExcute(result[index], excuteInfo, system, callback, t1, t2, t3, t4);
                else
                    callback(result[index], t1, t2, t3, t4);
            }
            ObjectPool<SystemHandle<T1, T2, T3, T4>>.Instance.Push(this);
        }
        void CallbackExcute(Entity entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<T1, T2, T3, T4> callback, T1 c1, T2 c2, T3 c3, T4 c4)
        {

            if (!system.TryGetExcuteInfo(excuteInfo.selectId, out ExcuteInfo _excuteInfo))
            {
                _excuteInfo = excuteInfo;
                system.AddSelectId2ExcuteInfoMapping(excuteInfo.selectId, _excuteInfo);
            }
            //对于不同SelectId：
            //如果该实体已经注册过该ExcuteInfo则返回
            if (system.TryGetExcuteInfos(entity.Index, out List<int> checkList))
            {
                if (checkList.Contains(_excuteInfo.selectId))
                    return;
                else
                    checkList.Add(_excuteInfo.selectId);
            }
            //否则进行注册
            else
            {
                checkList = new List<int>() { _excuteInfo.selectId };
                system.AddEntity2SelectIdMapping(entity.Index, checkList);
            }
            #region ExcuteInfo 处理

            //是否存在事件
            if (!string.IsNullOrEmpty(_excuteInfo.eventName))
            {
                UnityAction action = null;
                action = () =>
                {
                    if (!entity.IsActived)
                    {
                        EventManager.Instance.RemoveActionListener(_excuteInfo.eventName, action);
                        system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                    }
                    else
                        callback(entity, c1, c2, c3, c4);
                };
                EventManager.Instance.AddActionListener(_excuteInfo.eventName, action);
            }
            //是否注册了延迟时间
            else if (_excuteInfo.excuteTime != double.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayTimeExcute(_excuteInfo.delayTime, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2, c3, c4);
                 });
            }
            //是否注册了延迟帧
            else if (_excuteInfo.excuteFrame != ulong.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayFrameExcute(_excuteInfo.delayFrame, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2, c3, c4);
                 });
            }
            #endregion
        }
    }
    public class SystemHandle<T1, T2, T3, T4, T5> : SystemHandleBase where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent
    {
        public static SystemHandle<T1, T2, T3, T4, T5> None = new SystemHandle<T1, T2, T3, T4, T5>() { };
        public SystemCallback<T1, T2, T3, T4, T5> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count() == 0)
            {
                ObjectPool<SystemHandle<T1, T2, T3, T4, T5>>.Instance.Push(this);
                return;
            }

            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;


            for (int index = 0; index < result.Count(); index++)
            {
                foreach (var component in World.EntityManager.GetComponents(result.ElementAt(index)))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                    if (component is T5 c5) t5 = c5;
                }
                if (excuteInfo.selectId != int.MinValue)
                    CallbackExcute(result[index], excuteInfo, system, callback, t1, t2, t3, t4, t5);
                else
                    callback(result[index], t1, t2, t3, t4, t5);
            }
            ObjectPool<SystemHandle<T1, T2, T3, T4, T5>>.Instance.Push(this);
        }
        void CallbackExcute(Entity entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<T1, T2, T3, T4, T5> callback, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5)
        {

            if (!system.TryGetExcuteInfo(excuteInfo.selectId, out ExcuteInfo _excuteInfo))
            {
                _excuteInfo = excuteInfo;
                system.AddSelectId2ExcuteInfoMapping(excuteInfo.selectId, _excuteInfo);
            }
            //对于不同SelectId：
            //如果该实体已经注册过该ExcuteInfo则返回
            if (system.TryGetExcuteInfos(entity.Index, out List<int> checkList))
            {
                if (checkList.Contains(_excuteInfo.selectId))
                    return;
                else
                    checkList.Add(_excuteInfo.selectId);
            }
            //否则进行注册
            else
            {
                checkList = new List<int>() { _excuteInfo.selectId };
                system.AddEntity2SelectIdMapping(entity.Index, checkList);
            }
            #region ExcuteInfo 处理

            //是否存在事件
            if (!string.IsNullOrEmpty(_excuteInfo.eventName))
            {
                UnityAction action = null;
                action = () =>
                {
                    if (!entity.IsActived)
                    {
                        EventManager.Instance.RemoveActionListener(_excuteInfo.eventName, action);
                        system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                    }
                    else
                        callback(entity, c1, c2, c3, c4, c5);
                };
                EventManager.Instance.AddActionListener(_excuteInfo.eventName, action);
            }
            //是否注册了延迟时间
            else if (_excuteInfo.excuteTime != double.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayTimeExcute(_excuteInfo.delayTime, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2, c3, c4, c5);
                 });
            }
            //是否注册了延迟帧
            else if (_excuteInfo.excuteFrame != ulong.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayFrameExcute(_excuteInfo.delayFrame, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2, c3, c4, c5);
                 });
            }
            #endregion
        }

    }
    public class SystemHandle<T1, T2, T3, T4, T5, T6> : SystemHandleBase where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent
    {
        public static SystemHandle<T1, T2, T3, T4, T5, T6> None = new SystemHandle<T1, T2, T3, T4, T5, T6>() { };
        public SystemCallback<T1, T2, T3, T4, T5, T6> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count() == 0)
            {
                ObjectPool<SystemHandle<T1, T2, T3, T4, T5, T6>>.Instance.Push(this);
                return;
            }

            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            T6 t6 = null;


            for (int index = 0; index < result.Count(); index++)
            {
                foreach (var component in World.EntityManager.GetComponents(result.ElementAt(index)))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                    if (component is T5 c5) t5 = c5;
                    if (component is T6 c6) t6 = c6;
                }
                if (excuteInfo.selectId != int.MinValue)
                    CallbackExcute(result[index], excuteInfo, system, callback, t1, t2, t3, t4, t5, t6);
                else
                    callback(result[index], t1, t2, t3, t4, t5, t6);
            }
            ObjectPool<SystemHandle<T1, T2, T3, T4, T5, T6>>.Instance.Push(this);
        }
        void CallbackExcute(Entity entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<T1, T2, T3, T4, T5, T6> callback, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6)
        {

            if (!system.TryGetExcuteInfo(excuteInfo.selectId, out ExcuteInfo _excuteInfo))
            {
                _excuteInfo = excuteInfo;
                system.AddSelectId2ExcuteInfoMapping(excuteInfo.selectId, _excuteInfo);
            }
            //对于不同SelectId：
            //如果该实体已经注册过该ExcuteInfo则返回
            if (system.TryGetExcuteInfos(entity.Index, out List<int> checkList))
            {
                if (checkList.Contains(_excuteInfo.selectId))
                    return;
                else
                    checkList.Add(_excuteInfo.selectId);
            }
            //否则进行注册
            else
            {
                checkList = new List<int>() { _excuteInfo.selectId };
                system.AddEntity2SelectIdMapping(entity.Index, checkList);
            }
            #region ExcuteInfo 处理

            //是否存在事件
            if (!string.IsNullOrEmpty(_excuteInfo.eventName))
            {
                UnityAction action = null;
                action = () =>
                {
                    if (!entity.IsActived)
                    {
                        EventManager.Instance.RemoveActionListener(_excuteInfo.eventName, action);
                        system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                    }
                    else
                        callback(entity, c1, c2, c3, c4, c5, c6);
                };
                EventManager.Instance.AddActionListener(_excuteInfo.eventName, action);
            }
            //是否注册了延迟时间
            else if (_excuteInfo.excuteTime != double.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayTimeExcute(_excuteInfo.delayTime, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2, c3, c4, c5, c6);
                 });
            }
            //是否注册了延迟帧
            else if (_excuteInfo.excuteFrame != ulong.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayFrameExcute(_excuteInfo.delayFrame, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2, c3, c4, c5, c6);
                 });
            }
            #endregion
        }
    }
    public class SystemHandle<T1, T2, T3, T4, T5, T6, T7> : SystemHandleBase where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent
    {
        public static SystemHandle<T1, T2, T3, T4, T5, T6, T7> None = new SystemHandle<T1, T2, T3, T4, T5, T6, T7>() { };
        public SystemCallback<T1, T2, T3, T4, T5, T6, T7> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count() == 0)
            {
                ObjectPool<SystemHandle<T1, T2, T3, T4, T5, T6, T7>>.Instance.Push(this);
                return;
            }

            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            T6 t6 = null;
            T7 t7 = null;


            for (int index = 0; index < result.Count(); index++)
            {
                foreach (var component in World.EntityManager.GetComponents(result.ElementAt(index)))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                    if (component is T5 c5) t5 = c5;
                    if (component is T6 c6) t6 = c6;
                    if (component is T7 c7) t7 = c7;
                }
                if (excuteInfo.selectId != int.MinValue)
                    CallbackExcute(result[index], excuteInfo, system, callback, t1, t2, t3, t4, t5, t6, t7);
                else
                    callback(result[index], t1, t2, t3, t4, t5, t6, t7);
            }
            ObjectPool<SystemHandle<T1, T2, T3, T4, T5, T6, T7>>.Instance.Push(this);
        }
        void CallbackExcute(Entity entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<T1, T2, T3, T4, T5, T6, T7> callback, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7)
        {

            if (!system.TryGetExcuteInfo(excuteInfo.selectId, out ExcuteInfo _excuteInfo))
            {
                _excuteInfo = excuteInfo;
                system.AddSelectId2ExcuteInfoMapping(excuteInfo.selectId, _excuteInfo);
            }
            //对于不同SelectId：
            //如果该实体已经注册过该ExcuteInfo则返回
            if (system.TryGetExcuteInfos(entity.Index, out List<int> checkList))
            {
                if (checkList.Contains(_excuteInfo.selectId))
                    return;
                else
                    checkList.Add(_excuteInfo.selectId);
            }
            //否则进行注册
            else
            {
                checkList = new List<int>() { _excuteInfo.selectId };
                system.AddEntity2SelectIdMapping(entity.Index, checkList);
            }
            #region ExcuteInfo 处理

            //是否存在事件
            if (!string.IsNullOrEmpty(_excuteInfo.eventName))
            {
                UnityAction action = null;
                action = () =>
                {
                    if (!entity.IsActived)
                    {
                        EventManager.Instance.RemoveActionListener(_excuteInfo.eventName, action);
                        system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                    }
                    else
                        callback(entity, c1, c2, c3, c4, c5, c6, c7);
                };
                EventManager.Instance.AddActionListener(_excuteInfo.eventName, action);
            }
            //是否注册了延迟时间
            else if (_excuteInfo.excuteTime != double.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayTimeExcute(_excuteInfo.delayTime, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2, c3, c4, c5, c6, c7);
                 });
            }
            //是否注册了延迟帧
            else if (_excuteInfo.excuteFrame != ulong.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayFrameExcute(_excuteInfo.delayFrame, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2, c3, c4, c5, c6, c7);
                 });
            }
            #endregion
        }
    }
    public class SystemHandle<T1, T2, T3, T4, T5, T6, T7, T8> : SystemHandleBase where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent where T8 : class, IComponent
    {
        public static SystemHandle<T1, T2, T3, T4, T5, T6, T7, T8> None = new SystemHandle<T1, T2, T3, T4, T5, T6, T7, T8>() { };
        public SystemCallback<T1, T2, T3, T4, T5, T6, T7, T8> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count() == 0)
            {
                ObjectPool<SystemHandle<T1, T2, T3, T4, T5, T6, T7, T8>>.Instance.Push(this);
                return;
            }

            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            T6 t6 = null;
            T7 t7 = null;
            T8 t8 = null;
            for (int index = 0; index < result.Count(); index++)
            {
                foreach (var component in World.EntityManager.GetComponents(result.ElementAt(index)))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                    if (component is T5 c5) t5 = c5;
                    if (component is T6 c6) t6 = c6;
                    if (component is T7 c7) t7 = c7;
                    if (component is T8 c8) t8 = c8;
                }
                if (excuteInfo.selectId != int.MinValue)
                    CallbackExcute(result[index], excuteInfo, system, callback, t1, t2, t3, t4, t5, t6,t7,t8);
                else
                    callback(result[index], t1, t2, t3, t4, t5, t6, t7, t8);
            }
            ObjectPool<SystemHandle<T1, T2, T3, T4, T5, T6, T7, T8>>.Instance.Push(this);
        }
        void CallbackExcute(Entity entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<T1, T2, T3, T4, T5, T6, T7,T8> callback, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7,T8 c8)
        {

            if (!system.TryGetExcuteInfo(excuteInfo.selectId, out ExcuteInfo _excuteInfo))
            {
                _excuteInfo = excuteInfo;
                system.AddSelectId2ExcuteInfoMapping(excuteInfo.selectId, _excuteInfo);
            }
            //对于不同SelectId：
            //如果该实体已经注册过该ExcuteInfo则返回
            if (system.TryGetExcuteInfos(entity.Index, out List<int> checkList))
            {
                if (checkList.Contains(_excuteInfo.selectId))
                    return;
                else
                    checkList.Add(_excuteInfo.selectId);
            }
            //否则进行注册
            else
            {
                checkList = new List<int>() { _excuteInfo.selectId };
                system.AddEntity2SelectIdMapping(entity.Index, checkList);
            }
            #region ExcuteInfo 处理

            //是否存在事件
            if (!string.IsNullOrEmpty(_excuteInfo.eventName))
            {
                UnityAction action = null;
                action = () =>
                {
                    if (!entity.IsActived)
                    {
                        EventManager.Instance.RemoveActionListener(_excuteInfo.eventName, action);
                        system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                    }
                    else
                        callback(entity, c1, c2, c3, c4, c5, c6, c7,c8);
                };
                EventManager.Instance.AddActionListener(_excuteInfo.eventName, action);
            }
            //是否注册了延迟时间
            else if (_excuteInfo.excuteTime != double.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayTimeExcute(_excuteInfo.delayTime, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2, c3, c4, c5, c6, c7,c8);
                 });
            }
            //是否注册了延迟帧
            else if (_excuteInfo.excuteFrame != ulong.MaxValue)
            {
                ulong taskIndex = 0;
                taskIndex = TimerManager.Instance.DelayFrameExcute(_excuteInfo.delayFrame, 0, () =>
                 {
                     if (!entity.IsActived)
                     {
                         TimerManager.Instance.RemoveTask(taskIndex);
                         system.RemoveEntity2ExcuteInfosMapping(entity.Index);
                     }
                     else
                         callback(entity, c1, c2, c3, c4, c5, c6, c7,c8);
                 });
            }
            #endregion
        }
    
    }
    #endregion
}
