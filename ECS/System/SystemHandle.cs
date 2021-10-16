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
    public abstract class AbsSystemHandle<E> : IExcutable where E : Entity
    {
        internal IEnumerable<E> result;
        internal string eventName;
        internal ISystem system;
        public abstract void Excute();
    }
    internal static class EntityCallbackExcuteExtension
    {
        internal static void CallbackExcute<E>(this E entity, string eventName, ISystem system, SystemCallback<E> callback, bool isLastOne = false) where E : Entity
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                if (!system.m_Event2IndexListMapping.TryGetValue(eventName, out List<ulong> indexList))
                {
                    indexList = new List<ulong>();
                    system.m_Event2IndexListMapping.Add(eventName, indexList);
                }
                if(!indexList.Contains(entity.Index))
                {
                    indexList.Add(entity.Index);
                    system.m_Index2EventMapping.Add(entity.Index, eventName);
                }
                else
                    return;

                EventManager.Instance.AddActionListener_AutoRemoveAfterTrigger(eventName, () =>
                {
                    callback(entity);
                    system.m_Event2IndexListMapping[eventName].Remove(entity.Index);
                    system.m_Index2EventMapping.Remove(entity.Index);
                });
            }
            else
                callback(entity);
        }
        internal static void CallbackExcute<E, T>(this E entity, string eventName, ISystem system, SystemCallback<E, T> callback, T t, bool isLastOne = false) where E : Entity where T : class, IComponent
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                if (!system.m_Event2IndexListMapping.TryGetValue(eventName, out List<ulong> indexList))
                {
                    indexList = new List<ulong>();
                    system.m_Event2IndexListMapping.Add(eventName, indexList);
                }
                if(!indexList.Contains(entity.Index))
                {
                    indexList.Add(entity.Index);
                    system.m_Index2EventMapping.Add(entity.Index, eventName);
                }
                else
                    return;

                EventManager.Instance.AddActionListener_AutoRemoveAfterTrigger(eventName, () =>
                {
                    callback(entity, t);
                    system.m_Event2IndexListMapping[eventName].Remove(entity.Index);
                    system.m_Index2EventMapping.Remove(entity.Index);
                });
            }
            else
                callback(entity, t);
        }
        internal static void CallbackExcute<E, T1, T2>(this E entity, string eventName, ISystem system, SystemCallback<E, T1, T2> callback, T1 t1, T2 t2, bool isLastOne = false) where E : Entity where T1 : class, IComponent where T2 : class, IComponent
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                if (!system.m_Event2IndexListMapping.TryGetValue(eventName, out List<ulong> indexList))
                {
                    indexList = new List<ulong>();
                    system.m_Event2IndexListMapping.Add(eventName, indexList);
                }
                if(!indexList.Contains(entity.Index))
                {
                    indexList.Add(entity.Index);
                    system.m_Index2EventMapping.Add(entity.Index, eventName);
                }
                else
                    return;

                EventManager.Instance.AddActionListener_AutoRemoveAfterTrigger(eventName, () =>
                {
                    callback(entity, t1, t2);
                    system.m_Event2IndexListMapping[eventName].Remove(entity.Index);
                    system.m_Index2EventMapping.Remove(entity.Index);
                });
            }
            else
                callback(entity, t1, t2);
        }
        internal static void CallbackExcute<E, T1, T2, T3>(this E entity, string eventName, ISystem system, SystemCallback<E, T1, T2, T3> callback, T1 t1, T2 t2, T3 t3, bool isLastOne = false) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                if (!system.m_Event2IndexListMapping.TryGetValue(eventName, out List<ulong> indexList))
                {
                    indexList = new List<ulong>();
                    system.m_Event2IndexListMapping.Add(eventName, indexList);
                }
                if(!indexList.Contains(entity.Index))
                {
                    indexList.Add(entity.Index);
                    system.m_Index2EventMapping.Add(entity.Index, eventName);
                }
                else
                    return;

                EventManager.Instance.AddActionListener_AutoRemoveAfterTrigger(eventName, () =>
                {
                    callback(entity, t1, t2, t3);
                    system.m_Event2IndexListMapping[eventName].Remove(entity.Index);
                    system.m_Index2EventMapping.Remove(entity.Index);
                });
            }
            else
                callback(entity, t1, t2, t3);
        }
        internal static void CallbackExcute<E, T1, T2, T3, T4>(this E entity, string eventName, ISystem system, SystemCallback<E, T1, T2, T3, T4> callback, T1 t1, T2 t2, T3 t3, T4 t4, bool isLastOne = false) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                if (!system.m_Event2IndexListMapping.TryGetValue(eventName, out List<ulong> indexList))
                {
                    indexList = new List<ulong>();
                    system.m_Event2IndexListMapping.Add(eventName, indexList);
                }
                if(!indexList.Contains(entity.Index))
                {
                    indexList.Add(entity.Index);
                    system.m_Index2EventMapping.Add(entity.Index, eventName);
                }
                else
                    return;

                EventManager.Instance.AddActionListener_AutoRemoveAfterTrigger(eventName, () =>
                {
                    callback(entity, t1, t2, t3, t4);
                    system.m_Event2IndexListMapping[eventName].Remove(entity.Index);
                    system.m_Index2EventMapping.Remove(entity.Index);
                });
            }
            else
                callback(entity, t1, t2, t3, t4);
        }
        internal static void CallbackExcute<E, T1, T2, T3, T4, T5>(this E entity, string eventName, ISystem system, SystemCallback<E, T1, T2, T3, T4, T5> callback, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, bool isLastOne = false) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                if (!system.m_Event2IndexListMapping.TryGetValue(eventName, out List<ulong> indexList))
                {
                    indexList = new List<ulong>();
                    system.m_Event2IndexListMapping.Add(eventName, indexList);
                }
                if(!indexList.Contains(entity.Index))
                {
                    indexList.Add(entity.Index);
                    system.m_Index2EventMapping.Add(entity.Index, eventName);
                }
                else
                    return;

                EventManager.Instance.AddActionListener_AutoRemoveAfterTrigger(eventName, () =>
                {
                    callback(entity, t1, t2, t3, t4, t5);
                    system.m_Event2IndexListMapping[eventName].Remove(entity.Index);
                    system.m_Index2EventMapping.Remove(entity.Index);
                });
            }
            else
                callback(entity, t1, t2, t3, t4, t5);
        }
        internal static void CallbackExcute<E, T1, T2, T3, T4, T5, T6>(this E entity, string eventName, ISystem system, SystemCallback<E, T1, T2, T3, T4, T5, T6> callback, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, bool isLastOne = false) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                if (!system.m_Event2IndexListMapping.TryGetValue(eventName, out List<ulong> indexList))
                {
                    indexList = new List<ulong>();
                    system.m_Event2IndexListMapping.Add(eventName, indexList);
                }
                if(!indexList.Contains(entity.Index))
                {
                    indexList.Add(entity.Index);
                    system.m_Index2EventMapping.Add(entity.Index, eventName);
                }
                else
                    return;

                EventManager.Instance.AddActionListener_AutoRemoveAfterTrigger(eventName, () =>
                {
                    callback(entity, t1, t2, t3, t4, t5, t6);
                    system.m_Event2IndexListMapping[eventName].Remove(entity.Index);
                    system.m_Index2EventMapping.Remove(entity.Index);
                });
            }
            else
                callback(entity, t1, t2, t3, t4, t5, t6);
        }
        internal static void CallbackExcute<E, T1, T2, T3, T4, T5, T6, T7>(this E entity, string eventName, ISystem system, SystemCallback<E, T1, T2, T3, T4, T5, T6, T7> callback, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, bool isLastOne = false) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                if (!system.m_Event2IndexListMapping.TryGetValue(eventName, out List<ulong> indexList))
                {
                    indexList = new List<ulong>();
                    system.m_Event2IndexListMapping.Add(eventName, indexList);
                }
                if(!indexList.Contains(entity.Index))
                {
                    indexList.Add(entity.Index);
                    system.m_Index2EventMapping.Add(entity.Index, eventName);
                }
                else
                    return;

                EventManager.Instance.AddActionListener_AutoRemoveAfterTrigger(eventName, () =>
                {
                    callback(entity, t1, t2, t3, t4, t5, t6, t7);
                    system.m_Event2IndexListMapping[eventName].Remove(entity.Index);
                    system.m_Index2EventMapping.Remove(entity.Index);
                });
            }
            else
                callback(entity, t1, t2, t3, t4, t5, t6, t7);
        }
        internal static void CallbackExcute<E, T1, T2, T3, T4, T5, T6, T7, T8>(this E entity, string eventName, ISystem system, SystemCallback<E, T1, T2, T3, T4, T5, T6, T7, T8> callback, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, bool isLastOne = false) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent where T8 : class, IComponent
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                if (!system.m_Event2IndexListMapping.TryGetValue(eventName, out List<ulong> indexList))
                {
                    indexList = new List<ulong>();
                    system.m_Event2IndexListMapping.Add(eventName, indexList);
                }
                if(!indexList.Contains(entity.Index))
                {
                    indexList.Add(entity.Index);
                    system.m_Index2EventMapping.Add(entity.Index, eventName);
                }
                else
                    return;

                EventManager.Instance.AddActionListener_AutoRemoveAfterTrigger(eventName, () =>
                {
                    callback(entity, t1, t2, t3, t4, t5, t6, t7, t8);
                    system.m_Event2IndexListMapping[eventName].Remove(entity.Index);
                    system.m_Index2EventMapping.Remove(entity.Index);
                });
            }
            else
                callback(entity, t1, t2, t3, t4, t5, t6, t7, t8);
        }
    }
    public class SystemHandle<E> : AbsSystemHandle<E> where E : Entity
    {
        public static SystemHandle<E> None = new SystemHandle<E>() { };
        public SystemCallback<E> callback;
        public override void Excute()
        {
            if (result == null || callback == null)
                return;
            int count = result.Count();
            int i = 0;
            foreach (var item in result)
            {
                item.CallbackExcute(eventName, system, callback, i++ == count);
                break;
            }
        }
    }
    public class SystemHandle<E, T> : AbsSystemHandle<E> where E : Entity where T : class, IComponent
    {
        public static SystemHandle<E, T> None = new SystemHandle<E, T>() { };
        public SystemCallback<E, T> callback;
        public override void Excute()
        {
            if (result == null || callback == null)
                return;
            int count = result.Count();
            int i = 0;
            foreach (var item in result)
            {

                foreach (var component in World.EntityManager.GetComponents(item))
                {
                    if (component is T c)
                    {
                        item.CallbackExcute(eventName, system, callback, c, i++ == count);
                        break;
                    }
                }

            }
        }
    }
    public class SystemHandle<E, T1, T2> : AbsSystemHandle<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent
    {
        public static SystemHandle<E, T1, T2> None = new SystemHandle<E, T1, T2>() { };
        public SystemCallback<E, T1, T2> callback;
        public override void Excute()
        {
            if (result == null || callback == null)
                return;
            T1 t1 = null;
            T2 t2 = null;
            int count = result.Count();
            int i = 0;
            foreach (var item in result)
            {
                foreach (var component in World.EntityManager.GetComponents(item))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                }
                item.CallbackExcute(eventName, system, callback, t1, t2, ++i == count);
            }

        }
    }
    public class SystemHandle<E, T1, T2, T3> : AbsSystemHandle<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
    {
        public static SystemHandle<E, T1, T2, T3> None = new SystemHandle<E, T1, T2, T3>() { };
        public SystemCallback<E, T1, T2, T3> callback;
        public override void Excute()
        {
            if (result == null || callback == null)
                return;
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            int count = result.Count();
            int i = 0;
            foreach (var item in result)
            {
                foreach (var component in World.EntityManager.GetComponents(item))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                }
                item.CallbackExcute(eventName, system, callback, t1, t2, t3, i++ == count);
            }
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4> : AbsSystemHandle<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
    {
        public static SystemHandle<E, T1, T2, T3, T4> None = new SystemHandle<E, T1, T2, T3, T4>() { };
        public SystemCallback<E, T1, T2, T3, T4> callback;
        public override void Excute()
        {
            if (result == null || callback == null)
                return;
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            int count = result.Count();
            int i = 0;
            foreach (var item in result)
            {
                foreach (var component in World.EntityManager.GetComponents(item))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                }
                item.CallbackExcute(eventName, system, callback, t1, t2, t3, t4, i++ == count);
            }
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4, T5> : AbsSystemHandle<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent
    {
        public static SystemHandle<E, T1, T2, T3, T4, T5> None = new SystemHandle<E, T1, T2, T3, T4, T5>() { };
        public SystemCallback<E, T1, T2, T3, T4, T5> callback;
        public override void Excute()
        {
            if (result == null || callback == null)
                return;
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            int count = result.Count();
            int i = 0;
            foreach (var item in result)
            {
                foreach (var component in World.EntityManager.GetComponents(item))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                    if (component is T5 c5) t5 = c5;
                }
                item.CallbackExcute(eventName, system, callback, t1, t2, t3, t4, t5, i++ == count);
            }
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4, T5, T6> : AbsSystemHandle<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent
    {
        public static SystemHandle<E, T1, T2, T3, T4, T5, T6> None = new SystemHandle<E, T1, T2, T3, T4, T5, T6>() { };
        public SystemCallback<E, T1, T2, T3, T4, T5, T6> callback;
        public override void Excute()
        {
            if (result == null || callback == null)
                return;
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            T6 t6 = null;
            int count = result.Count();
            int i = 0;
            foreach (var item in result)
            {
                foreach (var component in World.EntityManager.GetComponents(item))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                    if (component is T5 c5) t5 = c5;
                    if (component is T6 c6) t6 = c6;
                }
                item.CallbackExcute(eventName, system, callback, t1, t2, t3, t4, t5, t6, i++ == count);
            }
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4, T5, T6, T7> : AbsSystemHandle<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent
    {
        public static SystemHandle<E, T1, T2, T3, T4, T5, T6, T7> None = new SystemHandle<E, T1, T2, T3, T4, T5, T6, T7>() { };
        public SystemCallback<E, T1, T2, T3, T4, T5, T6, T7> callback;
        public override void Excute()
        {
            if (result == null || callback == null)
                return;
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            T6 t6 = null;
            T7 t7 = null;
            int count = result.Count();
            int i = 0;
            foreach (var item in result)
            {
                foreach (var component in World.EntityManager.GetComponents(item))
                {
                    if (component is T1 c1) t1 = c1;
                    if (component is T2 c2) t2 = c2;
                    if (component is T3 c3) t3 = c3;
                    if (component is T4 c4) t4 = c4;
                    if (component is T5 c5) t5 = c5;
                    if (component is T6 c6) t6 = c6;
                    if (component is T7 c7) t7 = c7;
                }
                item.CallbackExcute(eventName, system, callback, t1, t2, t3, t4, t5, t6, t7, i++ == count);
            }
        }
    }
    public class SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8> : AbsSystemHandle<E> where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent where T8 : class, IComponent
    {
        public static SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8> None = new SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8>() { };
        public SystemCallback<E, T1, T2, T3, T4, T5, T6, T7, T8> callback;
        public override void Excute()
        {
            if (result == null || callback == null)
                return;
            T1 t1 = null;
            T2 t2 = null;
            T3 t3 = null;
            T4 t4 = null;
            T5 t5 = null;
            T6 t6 = null;
            T7 t7 = null;
            T8 t8 = null;
            int count = result.Count();
            int i = 0;
            foreach (var item in result)
            {
                foreach (var component in World.EntityManager.GetComponents(item))
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
                item.CallbackExcute(eventName, system, callback, t1, t2, t3, t4, t5, t6, t7, t8, i++ == count);
            }
        }
    }
}
