using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using GDG.ModuleManager;
using GDG.Utils;

namespace GDG.ECS
{
    public delegate void SystemCallback<E>(E entity) where E : Entity;
    public delegate void SystemCallback<E, T>(E entity, T c) where E : Entity where T : class, IComponent;
    public delegate void SystemCallback<E, T1, T2>(E entity, T1 c1, T2 c2) where E : Entity where T1 : class, IComponent where T2 : class, IComponent;
    public delegate void SystemCallback<E, T1, T2, T3>(E entity, T1 c1, T2 c2, T3 c3) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent;
    public delegate void SystemCallback<E, T1, T2, T3, T4>(E entity, T1 c1, T2 c2, T3 c3, T4 c4) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent;
    public delegate void SystemCallback<E, T1, T2, T3, T4, T5>(E entity, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent;
    public delegate void SystemCallback<E, T1, T2, T3, T4, T5, T6>(E entity, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent;
    public delegate void SystemCallback<E, T1, T2, T3, T4, T5, T6, T7>(E entity, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent;
    public delegate void SystemCallback<E, T1, T2, T3, T4, T5, T6, T7, T8>(E entity, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7, T8 c8) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent where T8 : class, IComponent;
    public abstract class SystemHandleBase<E> : IExcutable where E : Entity
    {
        internal IEnumerable<E> result;
        internal ExcuteInfo excuteInfo = new ExcuteInfo();
        internal ISystem system;
        public abstract void Excute();
    }
    internal static class EntityCallbackExcuteExtension
    {
        private static void CallbackExcute(Entity entity, ExcuteInfo excuteInfo, ISystem system, Action callback)
        {
            if (excuteInfo.selectId != int.MinValue)
            {
                if (system.m_SelectId2ExcuteInfo.TryGetValue(excuteInfo.selectId, out ExcuteInfo _excuteInfo))
                {
                    if (_excuteInfo.excuteTime < GDGTools.Timer.CurrentTime || _excuteInfo.excuteFrame < GDGTools.Timer.CurrentFrame)
                    {
                        return;
                    }
                }
                else
                {
                    _excuteInfo = excuteInfo;
                    system.m_SelectId2ExcuteInfo.Add(excuteInfo.selectId, _excuteInfo);
                }

                if (system.m_Index2ExcuteInfoListMapping.TryGetValue(entity.Index,out List<ExcuteInfo> checkList))
                {
                    if(checkList.Contains(_excuteInfo))
                        return;
                }
                
                //检查是否注册了ExcuteInfo列表
                if (!system.m_ExcuteInfo2EntityListMapping.TryGetValue(_excuteInfo, out List<ulong> indexList))
                {
                    indexList = new List<ulong>();
                    system.m_ExcuteInfo2EntityListMapping.Add(_excuteInfo, indexList);
                }
                //检查Entity是否注册了ExcuteInfo
                if (!indexList.Contains(entity.Index))
                {
                    indexList.Add(entity.Index);
                    if(system.m_Index2ExcuteInfoListMapping.TryGetValue(entity.Index, out List<ExcuteInfo> excuteInfoList))
                    {
                        excuteInfoList.Add(_excuteInfo);
                    }
                    else
                    {
                        system.m_Index2ExcuteInfoListMapping.Add(entity.Index, new List<ExcuteInfo>(){_excuteInfo});
                    }
                }
                //是否存在事件
                if (!string.IsNullOrEmpty(_excuteInfo.eventName))
                {
                    UnityAction action = null;
                    action = () =>
                    {
                        if(!entity.IsActived)
                        {
                            GDGTools.EventCenter.RemoveActionListener(_excuteInfo.eventName, action);
                            system.m_Index2ExcuteInfoListMapping.Remove(entity.Index);
                            system.m_ExcuteInfo2EntityListMapping[_excuteInfo].Remove(entity.Index);
                        }
                        else
                            callback();
                    };
                    EventManager.Instance.AddActionListener(_excuteInfo.eventName, action);
                }
                //是否注册了延迟时间
                if (_excuteInfo.excuteTime != double.MaxValue)
                {
                    ulong taskIndex = 0;
                    taskIndex = GDGTools.Timer.DelayTimeExcute(_excuteInfo.delayTime,0, () =>
                    {
                        
                        if(!entity.IsActived)
                        {
                            GDGTools.Timer.RemoveTask(taskIndex);
                            system.m_Index2ExcuteInfoListMapping.Remove(entity.Index);
                            system.m_ExcuteInfo2EntityListMapping[_excuteInfo].Remove(entity.Index);
                        }
                        else
                            callback();
                    });
                }
                //是否注册了延迟帧
                if (_excuteInfo.excuteFrame != ulong.MaxValue)
                {
                    ulong taskIndex = 0;
                    taskIndex = GDGTools.Timer.DelayFrameExcute(_excuteInfo.delayFrame,0, () =>
                    {
                        if(!entity.IsActived)
                        {
                            GDGTools.Timer.RemoveTask(taskIndex);
                            system.m_Index2ExcuteInfoListMapping.Remove(entity.Index);
                            system.m_ExcuteInfo2EntityListMapping[_excuteInfo].Remove(entity.Index);
                        }
                        else
                            callback();
                    });
                }
            }
            else
                callback();
        }
        internal static void CallbackExcute<E>(this E entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<E> callback) where E : Entity
        {
            CallbackExcute(entity, excuteInfo, system, () => { callback(entity); });
        }
        internal static void CallbackExcute<E, T>(this E entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<E, T> callback, T t) where E : Entity where T : class, IComponent
        {
            CallbackExcute(entity, excuteInfo, system, () => { callback(entity,t); });
        }
        internal static void CallbackExcute<E, T1, T2>(this E entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<E, T1, T2> callback, T1 t1, T2 t2) where E : Entity where T1 : class, IComponent where T2 : class, IComponent
        {
            CallbackExcute(entity, excuteInfo, system, () => { callback(entity,t1,t2); });
        }
        internal static void CallbackExcute<E, T1, T2, T3>(this E entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<E, T1, T2, T3> callback, T1 t1, T2 t2, T3 t3) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
        {
            CallbackExcute(entity, excuteInfo, system, () => { callback(entity,t1,t2,t3); });
        }
        internal static void CallbackExcute<E, T1, T2, T3, T4>(this E entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<E, T1, T2, T3, T4> callback, T1 t1, T2 t2, T3 t3, T4 t4) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
        {
            CallbackExcute(entity, excuteInfo, system, () => { callback(entity,t1,t2,t3,t4); });
        }
        internal static void CallbackExcute<E, T1, T2, T3, T4, T5>(this E entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<E, T1, T2, T3, T4, T5> callback, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent
        {
            CallbackExcute(entity, excuteInfo, system, () => { callback(entity,t1,t2,t3,t4,t5); });
        }
        internal static void CallbackExcute<E, T1, T2, T3, T4, T5, T6>(this E entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<E, T1, T2, T3, T4, T5, T6> callback, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent
        {
            CallbackExcute(entity, excuteInfo, system, () => { callback(entity,t1,t2,t3,t4,t5,t6); });
        }
        internal static void CallbackExcute<E, T1, T2, T3, T4, T5, T6, T7>(this E entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<E, T1, T2, T3, T4, T5, T6, T7> callback, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent
        {
            CallbackExcute(entity, excuteInfo, system, () => { callback(entity,t1,t2,t3,t4,t5,t6,t7); });
        }
        internal static void CallbackExcute<E, T1, T2, T3, T4, T5, T6, T7, T8>(this E entity, ExcuteInfo excuteInfo, ISystem system, SystemCallback<E, T1, T2, T3, T4, T5, T6, T7, T8> callback, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent where T8 : class, IComponent
        {
            CallbackExcute(entity, excuteInfo, system, () => { callback(entity,t1,t2,t3,t4,t5,t6,t7,t8); });
        }
    }
    public class SystemHandle<E> : SystemHandleBase<E> where E : Entity
    {
        public static SystemHandle<E> None = new SystemHandle<E>() { };
        public SystemCallback<E> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count()==0 )
                return;
            
            
            for (int index = 0; index < result.Count();index++)
            {
                result.ElementAt(index).CallbackExcute(excuteInfo, system, callback);
            }
            
        }
    }
    public class SystemHandle<E, T> : SystemHandleBase<E> where E : Entity where T : class, IComponent
    {
        public static SystemHandle<E, T> None = new SystemHandle<E, T>() { };
        public SystemCallback<E, T> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count()==0)
                return;
            
            
            for (int index = 0; index < result.Count();index++)
            {
                foreach (var component in World.EntityManager.GetComponents(result.ElementAt(index)))
                {
                    if (component is T c)
                    {
                        result.ElementAt(index).CallbackExcute(excuteInfo, system, callback, c);
                        break;
                    }
                }
            }
            
        }
    }
    public class SystemHandle<E, T1, T2> : SystemHandleBase<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent
    {
        public static SystemHandle<E, T1, T2> None = new SystemHandle<E, T1, T2>() { };
        public SystemCallback<E, T1, T2> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count()==0 )
                return;
            T1 t1 = null;
            T2 t2 = null;
            
            
            for (int index = 0; index < result.Count();index++)
            {
                foreach (var component in World.EntityManager.GetComponents(result.ElementAt(index)))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                }
                result.ElementAt(index).CallbackExcute(excuteInfo, system, callback, t1, t2);
            }
            
        }
    }
    public class SystemHandle<E, T1, T2, T3> : SystemHandleBase<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
    {
        public static SystemHandle<E, T1, T2, T3> None = new SystemHandle<E, T1, T2, T3>() { };
        public SystemCallback<E, T1, T2, T3> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count()==0 )
                return;
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            
            
            for (int index = 0; index < result.Count();index++)
            {
                foreach (var component in World.EntityManager.GetComponents(result.ElementAt(index)))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                }
                result.ElementAt(index).CallbackExcute(excuteInfo, system, callback, t1, t2, t3);
            }
            
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4> : SystemHandleBase<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
    {
        public static SystemHandle<E, T1, T2, T3, T4> None = new SystemHandle<E, T1, T2, T3, T4>() { };
        public SystemCallback<E, T1, T2, T3, T4> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count()==0 )
                return;
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            
            
            for (int index = 0; index < result.Count();index++)
            {
                foreach (var component in World.EntityManager.GetComponents(result.ElementAt(index)))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                }
                result.ElementAt(index).CallbackExcute(excuteInfo, system, callback, t1, t2, t3, t4);
            }
            
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4, T5> : SystemHandleBase<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent
    {
        public static SystemHandle<E, T1, T2, T3, T4, T5> None = new SystemHandle<E, T1, T2, T3, T4, T5>() { };
        public SystemCallback<E, T1, T2, T3, T4, T5> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count()==0 )
                return;
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            
            
            for (int index = 0; index < result.Count();index++)
            {
                foreach (var component in World.EntityManager.GetComponents(result.ElementAt(index)))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                    if (component is T5 c5) t5 = c5;
                }
                result.ElementAt(index).CallbackExcute(excuteInfo, system, callback, t1, t2, t3, t4, t5);
            }
            
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4, T5, T6> : SystemHandleBase<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent
    {
        public static SystemHandle<E, T1, T2, T3, T4, T5, T6> None = new SystemHandle<E, T1, T2, T3, T4, T5, T6>() { };
        public SystemCallback<E, T1, T2, T3, T4, T5, T6> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count()==0 )
                return;
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            T6 t6 = null;
            
            
            for (int index = 0; index < result.Count();index++)
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
                result.ElementAt(index).CallbackExcute(excuteInfo, system, callback, t1, t2, t3, t4, t5, t6);
            }
            
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4, T5, T6, T7> : SystemHandleBase<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent
    {
        public static SystemHandle<E, T1, T2, T3, T4, T5, T6, T7> None = new SystemHandle<E, T1, T2, T3, T4, T5, T6, T7>() { };
        public SystemCallback<E, T1, T2, T3, T4, T5, T6, T7> callback;
        public override void Excute()
        {
            if (!excuteInfo.canBeExcuted || result == null || callback == null || result.Count()==0 )
                return;
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            T6 t6 = null;
            T7 t7 = null;
            
            
            for (int index = 0; index < result.Count();index++)
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
                result.ElementAt(index).CallbackExcute(excuteInfo, system, callback, t1, t2, t3, t4, t5, t6, t7);
            }
            
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8> : SystemHandleBase<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent where T8 : class, IComponent
    {
        public static SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8> None = new SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8>() { };
        public SystemCallback<E, T1, T2, T3, T4, T5, T6, T7, T8> callback;
        public override void Excute()
        {
            if ( !excuteInfo.canBeExcuted || result == null || callback == null || result.Count()==0)
                return;
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            T6 t6 = null;
            T7 t7 = null;
            T8 t8 = null;   
            for (int index = 0; index < result.Count();index++)
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
                result.ElementAt(index).CallbackExcute(excuteInfo, system, callback, t1, t2, t3, t4, t5, t6, t7, t8);
            }
            
        }
    }
}
