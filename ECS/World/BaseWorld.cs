using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ModuleManager;
using System;
using System.Reflection;
using System.Threading;

namespace GDG.ECS
{
    public class BaseWorld : AbsSingleton<BaseWorld>
    {
        public uint WorldID = 0;
        public string WorldName = "World";
        internal readonly EntityManager EntityManager;
        internal readonly List<ISystem> Systems;
        private Dictionary<Type, ISystem> m_SystemTypeMapping;
        internal MonoWorld monoWorld;
        private ulong entityMaxIndex;
        private uint maxTypeId;
        internal void EntityMaxIndexIncrease()
        {
            entityMaxIndex++;
        }
        internal ulong GetEntityMaxIndex()
        {
            return entityMaxIndex;
        }
        public BaseWorld()
        {
            EntityManager = new EntityManager();
            Systems = new List<ISystem>();
            m_SystemTypeMapping = new Dictionary<Type, ISystem>();

            var gameObject = new GameObject(WorldName);
            monoWorld = gameObject.AddComponent<MonoWorld>();
            monoWorld.AddOrRemoveListener(AfterInit, "Start");
        }
        private void AfterInit()
        {
            SystemSingletonInit();
            foreach (var system in Systems)
            {
                m_SystemTypeMapping.Add(system.GetType(), system);
                monoWorld.AddOrRemoveListener(system.OnUpdate, "Update");
                monoWorld.AddOrRemoveListener(system.OnStart, "Start");
            }
        }
        private void SystemSingletonInit()
        {

            List<Type> types = new List<Type>(Assembly.GetCallingAssembly().GetTypes().ToArray());

            foreach (var item in types)
            {
                var baseType = item.BaseType;

                if (baseType?.Name == "AbsSystem`1")
                {
                    var info = baseType.GetMethod("GetInstance");
                    info.Invoke(null, null);
                }
            }
        }
        public bool IsExistSystem<T>() where T : ISystem,new() => m_SystemTypeMapping.ContainsKey(typeof(T));
        internal void AddOrRemoveSystemFromMonoWorld(ISystem system, bool isAdd = true)
        {
            if (isAdd)
            {
                system.SetActive(true);
                monoWorld.AddOrRemoveListener(system.OnUpdate, "Update");
            }
            else
            {
                system.SetActive(false);
                monoWorld.AddOrRemoveListener(system.OnUpdate, "Update", false);
            }
                
        }
        internal void UpdateEntitiesOfSystems(List<AbsEntity> entityList)
        {
            foreach (var system in Systems)
            {
                if (system.Entities != null)
                    system.SetEntities(entityList);
                else
                    LogManager.Instance.LogError($"Update Entities of Systems faield，Systems.Entites is null ! System: {system.GetType()}");
            }
        }

        public bool AddOrRemoveEntityFromSystems(AbsEntity entity, bool isAdd = true)
        {
            //添加
            if (isAdd)
            {
                foreach (var system in Systems)
                {
                    system.Entities.Add(entity);
                }
                return true;
            }
            //删除
            if (Systems.Count < 1)
                return false;
            if (Systems[0].Entities.Contains(entity))
            {
                foreach (var system in Systems)
                {
                    if (system.Entities == null)
                    {
                        LogManager.Instance.LogError($"Remove Entity from Systems faield，Systems.Entites is null ! System: {system.GetType()}");
                        return false;
                    }
                    if (!system.Entities.Remove(entity))
                    {
                        LogManager.Instance.LogError($"Remove Entity from Systems faield，cant't find Entity in Systems ! Index: {entity.Index}");
                        return false;
                    }
                }
            }
            return true;
        }
        public bool AddOrRemoveEntityFromSystems<T>(AbsEntity entity, bool isAdd = true) where T : ISystem, new()
        {
            if (m_SystemTypeMapping.TryGetValue(typeof(T), out ISystem system))
            {
                //添加
                if (isAdd)
                {
                    if (system.Entities == null)
                    {
                        LogManager.Instance.LogError($"Add Entity from {typeof(T)} faield !");
                        return false;
                    }
                    system.Entities.Add(entity);
                    return true;
                }
                //删除
                if (system.Entities != null)
                {
                    if (system.Entities.Contains(entity))
                    {
                        if (!system.Entities.Remove(entity))
                        {
                            LogManager.Instance.LogError($"Remove Entity from Systems faield ! Index: {entity.Index}");
                            return false;
                        }
                    }
                    else
                        return false;
                }
                else
                {
                    LogManager.Instance.LogError($"Remove Entity from Systems faield，Systems.Entites is null ! System: {typeof(T)}");
                    return false;
                }
            }
            else
            {
                LogManager.Instance.LogError($"Remove Entity from Systems faield，Can't find Typeof'{typeof(T)}' in Systems !");
                return false;
            }
            return true;
        }
    }
}