using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ModuleManager;
namespace GDG.ECS
{
    public class World
    {
        private static uint MaxWorldID;
        /// <summary>
        /// Entity的IOC容器
        /// </summary>
        public static EntityManager EntityManager
        {
            get => BaseWorld.Instance.EntityManager;
        }
        /// <summary>
        /// 所有注册到World的系统
        /// </summary>
        public static List<ISystem> Systems
        {
            get => BaseWorld.Instance.Systems.Values.ToList();
        }
        /// <summary>
        /// 场景中World所挂载的MonoWorld脚本
        /// </summary>
        public static MonoWorld monoWorld
        {
            get => BaseWorld.Instance.monoWorld;
        }
        public static T GetSystem<T>() where T : class
        {
            if(BaseWorld.Instance.Systems.TryGetValue(typeof(T),out ISystem system))
            {
                return system as T;
            }
            return null;
        }
        public static ulong GetEntityMaxIndex()
        {
            return BaseWorld.Instance.GetEntityMaxIndex();
        }
        public static bool IsExistSystem<T>() where T : ISystem, new()
        {
            return BaseWorld.Instance.IsExistSystem<T>();
        }
        public static bool AddOrRemoveEntityFromSystems(Entity entity, bool isAdd = true)
        {
            return BaseWorld.Instance.AddOrRemoveEntityFromSystems(entity, isAdd);
        }
        public static bool AddOrRemoveEntityFromSystem<T>(Entity entity, bool isAdd = true) where T : ISystem, new()
        {
            return BaseWorld.Instance.AddOrRemoveEntityFromSystem<T>(entity, isAdd);
        }
        public static BaseWorld CreateWorld(string worldName = "World")
        {
            ++MaxWorldID;
            return new BaseWorld() { WorldID = MaxWorldID,WorldName = worldName};
        }
    }
}