using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.Utils;
namespace GDG.ECS
{
    public static class EntityExtension
    {
        public static bool TryGetComponent<T>(this Entity entity, out T component) where T : class, IComponent
        {
            component = null;
            foreach (var item in World.EntityManager.GetComponents(entity))
            {
                if (item is T t)
                {
                    component = t;
                    return true;
                }
            }
            return false;
        }
        public static T GetComponent<T>(this Entity entity) where T : class, IComponent
        {
            foreach (var item in World.EntityManager.GetComponents(entity))
            {
                if (item is T t)
                {
                    return t;
                }
            }
            return null;
        }
        public static void SetComponentData<T>(this Entity entity, T component) where T : class, IComponent
        {
            BaseWorld.Instance.EntityManager.SetComponentData(entity, component);
        }
        public static void SetComponentData<T>(this Entity entity, Action<T> action) where T : class, IComponent
        {
            BaseWorld.Instance.EntityManager.SetComponentData(entity, action);
        }
        public static bool RemoveComponet(this Entity entity, ComponentTypes componentTypes)
        {
            return BaseWorld.Instance.EntityManager.RemoveComponet(entity, componentTypes);
        }
        public static bool RemoveComponet<T>(this Entity entity) where T : class, IComponent
        {
            return BaseWorld.Instance.EntityManager.RemoveComponet<T>(entity);
        }
        public static T AddComponent<T>(this Entity entity) where T : class, IComponent, new()
        {
            return BaseWorld.Instance.EntityManager.AddComponent<T>(entity);
        }
        public static List<IComponent> AddComponent(this Entity entity, ComponentTypes componentTypes)
        {
            return BaseWorld.Instance.EntityManager.AddComponent(entity, componentTypes);
        }
        public static void Recycle(this Entity entity)
        {
            BaseWorld.Instance.EntityManager.RecycleEntity(entity);
        }
        public static void Destory(this Entity entity)
        {
            BaseWorld.Instance.EntityManager.DestroyEntity(entity);
        }
        public static bool IsExistComponent<T>(this Entity entity) where T : IComponent
        {
            foreach (var item in World.EntityManager.GetComponents(entity))
            {
                if (item is T)
                    return true;
            }
            return false;
        }
        public static Entity GetEntity(this GameObject gameObject)
        {
            return World.EntityManager.FindEntityInGameObjectMapping(gameObject);
        }
        public static bool TryGetEntity(this GameObject gameObject, out Entity entity)
        {
            entity = World.EntityManager.FindEntityInGameObjectMapping(gameObject);
            if(entity!=null)
                return true;
            return false;
        }
        public static Entity GetEntity(this Transform transform)
        {
            return transform.gameObject.GetEntity();
        }
        public static bool TryGetEntity(this Transform transform, out Entity entity)
        {
            return transform.gameObject.TryGetEntity(out entity);
        }
        public static T GetMonoComponent<T>(this Entity entity) where T:MonoBehaviour
        {
            if(entity.TryGetComponent<GameObjectComponent>(out GameObjectComponent game))
            {
                if(game.gameObject != null)
                {
                    return game.gameObject.GetComponent<T>();
                }
                else
                {
                    Log.Error("GameObject is null !");
                }
            }
            else
            {
                Log.Error($"Entity doesn't have GameObjectComponent ! Entity: {entity}");
            }
            return null;
        }
        public static bool TryGetMonoComponent<T>(this Entity entity,out T monoComponent) where T:MonoBehaviour
        {
            monoComponent = null;
            if(entity.TryGetComponent<GameObjectComponent>(out GameObjectComponent game))
            {
                if(game.gameObject != null)
                {
                    monoComponent = game.gameObject.GetComponent<T>();
                    return true;
                }
            }
            return false;
        }
        public static T GetMonoComponent<T>(this GameObjectComponent game) where T:MonoBehaviour
        {
            if(game.gameObject != null)
            {
                return game.gameObject.GetComponent<T>();
            }
            else
            {
                Log.Error("GameObject is null !");
            }
            return null;
        }
        public static bool TryGetMonoComponent<T>(this GameObjectComponent game,out T monoComponent) where T:MonoBehaviour
        {
            monoComponent = null;
            if(game.gameObject != null)
            {
                monoComponent = game.gameObject.GetComponent<T>();
                return true;
            }
            return false;
        }
    }
}