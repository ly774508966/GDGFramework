using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GDG.ModuleManager;
using System;
using GDG.Utils;
using GDG.Base;

namespace GDG.ECS
{
    public class BaseWorld : Singleton<BaseWorld>
    {
        public uint WorldID = 0;
        public string WorldName = "World";
        internal readonly EntityManager EntityManager;
        internal readonly Dictionary<Type, ISystem> Systems;
        internal MonoWorld monoWorld;
        public BaseWorld()
        {
            EntityManager = new EntityManager();
            Systems = new Dictionary<Type, ISystem>();
            GameObject gameObject = new GameObject(WorldName);
            monoWorld = gameObject.AddComponent<MonoWorld>();
            monoWorld.AddOrRemoveListener(AfterInit, "Start");
        }
        private void AfterInit()
        {
            SystemSingletonInit();
            foreach (var system in Systems.Values)
            {
                monoWorld.AddOrRemoveListener(system.OnUpdate , "Update");
                monoWorld.AddOrRemoveListener(system.OnLateUpdate, "LateUpdate");
                monoWorld.AddOrRemoveListener(system.OnFixedUpdate, "FixedUpdate");
                monoWorld.AddOrRemoveListener(system.OnStart, "BeforeUpdate");
            }
            UpdateEntitiesOfSystems(EntityManager.m_ActivedEntityList);
        }
        private void SystemSingletonInit()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //var assembly_Cshap = assemblies.First(assembly => assembly.GetName().Name == "Assembly-CSharp");
            foreach (var assembly in assemblies)
            {
                List<Type> types = new List<Type>(assembly.GetTypes().ToArray());
                foreach (var item in types)
                {
                    var baseType = item.BaseType;

                    if (baseType?.Name == "SystemBase`1")
                    {
                        var info = baseType.GetMethod("GetInstance");
                        info.Invoke(null, null);
                    }
                }
            }
        }
        public bool IsExistSystem<T>() where T : ISystem,new() => Systems.ContainsKey(typeof(T));
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
        internal void UpdateEntitiesOfSystems(List<Entity> entityList)
        {
            foreach (var system in Systems.Values)
            {
                if (system.Entities != null)
                    system.SetEntities(entityList);
                else
                    Log.Error($"Update Entities of Systems faield，Systems.Entites is null ! System: {system.GetType()}");
            }
        }
        public bool AddOrRemoveEntityFromSystems(Entity entity, bool isAdd = true)
        {
            //添加
            if (isAdd)
            {
                foreach (var system in Systems.Values)
                {
                    system.AddEntity(entity);
                }
                return true;
            }
            //删除
            if (Systems.Values.Count < 1)
                return false;
            if (Systems.Values.First().Entities.Contains(entity))
            {
                foreach (var system in Systems.Values)
                {
                    if (system.Entities == null)
                    {
                        Log.Error($"Remove Entity from Systems faield，Systems.Entites is null ! System: {system.GetType()}");
                        return false;
                    }
                    if (!system.RemoveEntity(entity))
                    {
                        Log.Error($"Remove Entity from Systems faield，cant't find Entity in Systems ! Index: {entity.Index}");
                        return false;
                    }
                }
            }
            return true;
        }
        public bool AddOrRemoveEntityFromSystem<T>(Entity entity, bool isAdd = true) where T : ISystem, new()
        {
            if (Systems.TryGetValue(typeof(T), out ISystem system))
            {
                //添加
                if (isAdd)
                {
                    if (system.Entities == null)
                    {
                        Log.Error($"Add Entity from {typeof(T)} faield !");
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
                            Log.Error($"Remove Entity from Systems faield ! Index: {entity.Index}");
                            return false;
                        }
                    }
                    else
                        return false;
                }
                else
                {
                    Log.Error($"Remove Entity from Systems faield，Systems.Entites is null ! System: {typeof(T)}");
                    return false;
                }
            }
            else
            {
                Log.Error($"Remove Entity from Systems faield，Can't find Typeof'{typeof(T)}' in Systems !");
                return false;
            }
            return true;
        }
    }
}