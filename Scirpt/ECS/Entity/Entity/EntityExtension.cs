using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            foreach (var item in World.EntityManager.GetAllEntity())
            {
                if (item.TryGetComponent<GameObjectComponent>(out GameObjectComponent game))
                {
                    if (game.gameObject == gameObject)
                        return item;
                }
            }
            return null;
        }
        public static bool TryGetEntity(this GameObject gameObject, out Entity entity)
        {
            entity = null;
            foreach (var item in World.EntityManager.GetAllEntity())
            {
                if (item.TryGetComponent<GameObjectComponent>(out GameObjectComponent game))
                {
                    if (game.gameObject == gameObject)
                    {
                        entity = item;
                        return true;
                    }
                }
            }
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
    }
}