using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ModuleManager;
namespace GDG.ECS
{
    public class World
    {
        private static uint MaxWorldID;
        public static EntityManager EntityManager
        {
            get => BaseWorld.Instance.EntityManager;
        }
        public static List<ISystem> Systems
        {
            get => BaseWorld.Instance.Systems;
        }
        public static MonoWorld monoWorld
        {
            get => BaseWorld.Instance.monoWorld;
        }
        public static ulong GetEntityMaxIndex()
        {
            return BaseWorld.Instance.GetEntityMaxIndex();
        }
        public static bool IsExistSystem<T>() where T : ISystem, new()
        {
            return BaseWorld.Instance.IsExistSystem<T>();
        }
        public static bool AddOrRemoveEntityFromSystems(AbsEntity entity, bool isAdd = true)
        {
            return BaseWorld.Instance.AddOrRemoveEntityFromSystems(entity, isAdd);
        }
        public static bool AddOrRemoveEntityFromSystems<T>(AbsEntity entity, bool isAdd = true) where T : ISystem, new()
        {
            return BaseWorld.Instance.AddOrRemoveEntityFromSystems<T>(entity, isAdd);
        }
        public static BaseWorld CreateWorld(string worldName = "World")
        {
            ++MaxWorldID;
            return new BaseWorld() { WorldID = MaxWorldID,WorldName = worldName};
        }
    }
}