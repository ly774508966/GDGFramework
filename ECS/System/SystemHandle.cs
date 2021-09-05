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
    public delegate void SystemCallback<E>(E entity) where E : AbsEntity;
    public delegate void SystemCallback<E, T>(E entity, T c) where E : AbsEntity where T : class, IComponentData;
    public delegate void SystemCallback<E, T1, T2>(E entity, T1 c1, T2 c2) where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData;
    public delegate void SystemCallback<E, T1, T2, T3>(E entity, T1 c1, T2 c2, T3 c3) where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData where T3 : class, IComponentData;
    public delegate void SystemCallback<E, T1, T2, T3, T4>(E entity, T1 c1, T2 c2, T3 c3, T4 c4) where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData where T3 : class, IComponentData where T4 : class, IComponentData;
    public delegate void SystemCallback<E, T1, T2, T3, T4, T5>(E entity, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5) where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData where T3 : class, IComponentData where T4 : class, IComponentData where T5 : class, IComponentData;
    public delegate void SystemCallback<E, T1, T2, T3, T4, T5, T6>(E entity, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6) where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData where T3 : class, IComponentData where T4 : class, IComponentData where T5 : class, IComponentData where T6 : class, IComponentData;
    public delegate void SystemCallback<E, T1, T2, T3, T4, T5, T6, T7>(E entity, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7) where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData where T3 : class, IComponentData where T4 : class, IComponentData where T5 : class, IComponentData where T6 : class, IComponentData where T7 : class, IComponentData;
    public delegate void SystemCallback<E, T1, T2, T3, T4, T5, T6, T7, T8>(E entity, T1 c1, T2 c2, T3 c3, T4 c4, T5 c5, T6 c6, T7 c7,T8 c8) where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData where T3 : class, IComponentData where T4 : class, IComponentData where T5 : class, IComponentData where T6 : class, IComponentData where T7 : class, IComponentData where T8 : class, IComponentData;
    public interface ISystemHandle<E>: IExcutable where E:AbsEntity
    {
        IEnumerable<E> result{ get; set; }
        string eventName{ get; set; }
    }
    internal static class EntityCallbackExcuteExtension
    {
        internal static bool CallbackExcute<E>(this E entity, string eventName, ISystem system, Action callback) where E : AbsEntity
        {
            if (eventName != null)
            {
                if (system.m_EventHandle2IndexMapping.TryGetValue(eventName, out List<ulong> indexList))
                {
                    if (indexList.Contains(entity.Index))
                        return false;

                    system.m_EventHandle2IndexMapping[eventName].Add(entity.Index);

                    UnityAction cb = null;
                    cb = () =>
                    {
                        callback();
                        EventManager.Instance.RemoveActionListener(eventName,cb);
                        system.m_EventHandle2IndexMapping[eventName].Remove(entity.Index);
                        cb = null;
                    };
                    EventManager.Instance.AddActionListener(eventName, cb);
                }
                else
                {
                    system.m_EventHandle2IndexMapping.Add(eventName, new List<ulong>() { entity.Index });
                    UnityAction cb = null;
                    cb = () =>
                    {
                        callback();
                        EventManager.Instance.RemoveActionListener(eventName, cb);
                        system.m_EventHandle2IndexMapping[eventName].Remove(entity.Index);
                        cb = null;
                    };
                    EventManager.Instance.AddActionListener(eventName, cb);
                }
            }
            else
                callback();
            return true;
        }
    }
    public class SystemHandle<E> : ISystemHandle<E> where E : AbsEntity
    {
        public IEnumerable<E> result{ get; set; }
        public ISystem system;
        public string eventName { get; set; }
        public SystemCallback<E> callback;
        public void Excute()
        {
            if (result == null || callback == null)
                return;
            foreach (var item in result)
            {
                item.CallbackExcute(eventName, system, () => { callback(item); });
                break;
            }
        }
    }
    public class SystemHandle<E, T> : ISystemHandle<E> where E : AbsEntity where T : class, IComponentData
    {
        public IEnumerable<E> result{ get; set; }
        public ISystem system;
        public string eventName { get; set; }
        public SystemCallback<E, T> callback;
        public void Excute()
        {
            if (result == null || callback == null)
                return;
            foreach (var item in result)
            {
                foreach (var component in item.Components)
                {
                    if (component is T c)
                    {
                        item.CallbackExcute(eventName, system, () => { callback(item, c); });
                        break;
                    }
                }

            }
        }
    }
    public class SystemHandle<E, T1, T2> : ISystemHandle<E> where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData
    {
        public IEnumerable<E> result{ get; set; }
        public ISystem system;
        public string eventName { get; set; }
        public SystemCallback<E, T1, T2> callback;
        public void Excute()
        {
            if (result == null || callback == null)
                return;
            T1 t1 = null;
            T2 t2 = null;
            foreach (var item in result)
            {
                foreach (var component in item.Components)
                {

                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    item.CallbackExcute(eventName, system, () => { callback(item, t1, t2); });
                }
            }
        }
    }
    public class SystemHandle<E, T1, T2, T3> : ISystemHandle<E> where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData where T3 : class, IComponentData
    {
        public IEnumerable<E> result{ get; set; }
        public ISystem system;
        public string eventName { get; set; }
        public SystemCallback<E, T1, T2, T3> callback;
        public void Excute()
        {
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            foreach (var item in result)
            {
                foreach (var component in item.Components)
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    item.CallbackExcute(eventName, system, () => { callback(item, t1, t2, t3); });
                }
            }
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4> : ISystemHandle<E> where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData where T3 : class, IComponentData where T4 : class, IComponentData
    {
        public IEnumerable<E> result{ get; set; }
        public ISystem system;
        public string eventName { get; set; }
        public SystemCallback<E, T1, T2, T3, T4> callback;
        public void Excute()
        {
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            foreach (var item in result)
            {
                foreach (var component in item.Components)
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                    item.CallbackExcute(eventName, system, () => { callback(item, t1, t2, t3, t4); });
                }
            }
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4, T5> : ISystemHandle<E> where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData where T3 : class, IComponentData where T4 : class, IComponentData where T5 : class, IComponentData
    {
        public IEnumerable<E> result{ get; set; }
        public ISystem system;
        public string eventName { get; set; }
        public SystemCallback<E, T1, T2, T3, T4, T5> callback;
        public void Excute()
        {
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            foreach (var item in result)
            {
                foreach (var component in item.Components)
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                    if (component is T5 c5) t5 = c5;
                    item.CallbackExcute(eventName, system, () => { callback(item, t1, t2, t3, t4, t5); });
                }
            }
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4, T5, T6> : ISystemHandle<E> where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData where T3 : class, IComponentData where T4 : class, IComponentData where T5 : class, IComponentData where T6 : class, IComponentData
    {
        public IEnumerable<E> result{ get; set; }
        public ISystem system;
        public string eventName { get; set; }
        public SystemCallback<E, T1, T2, T3, T4, T5, T6> callback;
        public void Excute()
        {
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            T6 t6 = null;
            foreach (var item in result)
            {
                foreach (var component in item.Components)
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                    if (component is T5 c5) t5 = c5;
                    if (component is T6 c6) t6 = c6;
                    item.CallbackExcute(eventName, system, () => { callback(item, t1, t2, t3, t4, t5, t6); });
                }
            }
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4, T5, T6, T7> : ISystemHandle<E> where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData where T3 : class, IComponentData where T4 : class, IComponentData where T5 : class, IComponentData where T6 : class, IComponentData where T7 : class, IComponentData
    {
        public IEnumerable<E> result{ get; set; }
        public ISystem system;
        public string eventName { get; set; }
        public SystemCallback<E, T1, T2, T3, T4, T5, T6, T7> callback;
        public void Excute()
        {
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            T6 t6 = null;
            T7 t7 = null;
            foreach (var item in result)
            {
                foreach (var component in item.Components)
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                    if (component is T5 c5) t5 = c5;
                    if (component is T6 c6) t6 = c6;
                    if (component is T7 c7) t7 = c7;
                    item.CallbackExcute(eventName, system, () => { callback(item, t1, t2, t3, t4, t5, t6, t7); });
                }
            }
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8> : ISystemHandle<E> where E : AbsEntity where T1 : class, IComponentData where T2 : class, IComponentData where T3 : class, IComponentData where T4 : class, IComponentData where T5 : class, IComponentData where T6 : class, IComponentData where T7 : class, IComponentData where T8 : class, IComponentData
    {
        public IEnumerable<E> result{ get; set; }
        public ISystem system;
        public string eventName { get; set; }
        public SystemCallback<E, T1, T2, T3, T4, T5, T6, T7, T8> callback;
        public void Excute()
        {
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            T6 t6 = null;
            T7 t7 = null;
            T8 t8 = null;
            foreach (var item in result)
            {
                foreach (var component in item.Components)
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                    if (component is T5 c5) t5 = c5;
                    if (component is T6 c6) t6 = c6;
                    if (component is T7 c7) t7 = c7;
                    if (component is T8 c8) t8 = c8;
                    item.CallbackExcute(eventName, system, () => { callback(item, t1, t2, t3, t4, t5, t6, t7, t8); });
                }
            }
        }
    }
}
