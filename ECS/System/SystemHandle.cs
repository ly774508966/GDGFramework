using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using GDG.ModuleManager;

namespace GDG.ECS
{
    public class SystemHandle : IExcutable
    {
        public IEnumerable<AbsEntity> result;
        public UnityAction<AbsEntity> callback;
        public IComponentData[] components;
        public ISystem system;
        public string eventName = null;
        public ulong Version = 0;
        public void Excute()
        {
            if (callback == null || result == null)
                return;
            if (components == null && result != null)
            {
                foreach (var entity in result)
                {
                    CallbackExcute(entity);
                }
                return;
            }
            foreach (var entity in result)
            {
                foreach (var component in components)
                {
                    entity.ComponentMatch(component);
                }
                CallbackExcute(entity);
            }
        }
        private bool CallbackExcute(AbsEntity entity)
        {
                if (eventName != null)
                {
                    if (system.m_EventHandle2IndexMapping.TryGetValue(eventName, out List<ulong> indexList))
                    {
                        if (indexList.Contains(entity.Index))
                            return false;
                    
                        system.m_EventHandle2IndexMapping[eventName].Add(entity.Index);

                        UnityAction<AbsEntity> cb = null;
                        cb = (obj) =>
                        {
                            callback(entity);
                            EventManager.Instance.RemoveActionListener<AbsEntity>(eventName, cb);
                            system.m_EventHandle2IndexMapping[eventName].Remove(entity.Index);
                            cb = null;
                        };
                        EventManager.Instance.AddActionListener<AbsEntity>(eventName, cb);
                    }
                    else
                    {
                        system.m_EventHandle2IndexMapping.Add(eventName, new List<ulong>(){entity.Index});
                        UnityAction<AbsEntity> cb = null;
                        cb = (obj) =>
                        {
                            callback(entity);
                            EventManager.Instance.RemoveActionListener<AbsEntity>(eventName, cb);
                            system.m_EventHandle2IndexMapping[eventName].Remove(entity.Index);
                            cb = null;
                        };
                        EventManager.Instance.AddActionListener<AbsEntity>(eventName, cb);
                    }
                }
                else
                    callback(entity);
                return true;
        }
    }
    
}
