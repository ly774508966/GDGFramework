using System;
using System.Collections;
using System.Collections.Generic;
using GDG.ModuleManager;
using GDG.Utils;
using UnityEngine;

namespace GDG.ECS
{
    public class Entity : IEquatable<Entity>
    {
        internal Action<Entity> setNameCallBack;
        internal Action initCallback;
        internal Action enableCallback;
        internal Action recycleCallback;
        internal Action destroyCallback;
        public string Name { get; set; }
        public ulong Index { get;private set; }
        public int Version { get;private set; }
        public uint TypeId{ get;private set; }
        public bool IsActived{ get;private set; }
        //public void SetName(string name) => this.Name = name;
        internal void SetIndex(ulong index) => this.Index = index;
        internal void SetVersion(int version) => this.Version = version;
        internal void SetTypeId(uint typeId) => this.TypeId = typeId;
        internal void SetActive(bool isActived) => this.IsActived = isActived;
        internal Entity(){}
        internal virtual void OnInit()
        {
            Version = 1;
            IsActived = true;

            initCallback?.Invoke();
            setNameCallBack?.Invoke(this);
        }
        internal virtual void OnEnable()
        {
            IsActived = true;
            enableCallback?.Invoke();
        }
        internal virtual void OnRecycle()
        {
            Version++;
            IsActived = false;
            recycleCallback?.Invoke();
        }
        internal virtual void OnDestroy()
        {
            IsActived = false;
            destroyCallback?.Invoke();
        }
    
        public bool Equals(Entity other) => Index == other.Index;

        public override bool Equals(object obj)
        {
            if (obj is Entity entity)
            {
                return Index == entity.Index && Version == entity.Version;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int tempHash = 0;
            tempHash ^= Index.GetHashCode();
            return tempHash;
        }
        public override string ToString() => $"Index: {this.Index}，Version: {this.Version}，Name: {Name}";
        public static bool operator ==(Entity lhs, Entity rhs) => lhs?.Index == rhs?.Index && lhs?.Version == rhs?.Version;
        public static bool operator !=(Entity lhs, Entity rhs) => lhs?.Index != rhs?.Index || lhs?.Version != rhs?.Version;
        public static Entity operator ++(Entity entity)
        {
            entity.SetVersion(entity.Version + 1);
            return entity;
        }
        public static Entity operator --(Entity entity)
        {
            entity.SetVersion(entity.Version - 1);
            return entity;
        }

    }
    public static class EntityExtension
    {
        public static bool TryGetComponent<T>(this Entity entity,out T component)where T:class,IComponent
        {
            component = null;
            foreach (var item in World.EntityManager.GetComponents(entity))
            {
                if(item is T t)
                {
                    component = t;
                    return true;
                }
            }
            return false;
        }
        public static T GetComponent<T>(this Entity entity)where T:class,IComponent
        {
            foreach (var item in World.EntityManager.GetComponents(entity))
            {
                if(item is T t)
                {
                    return t;
                }
            }
            return null;
        }
        public static void SetComponentData<T>(this Entity entity, T component) where T :class,IComponent
        {
            BaseWorld.Instance.EntityManager.SetComponentData(entity,component);
        }
        public static void SetComponentData<T>(this Entity entity,Action<T> action)where T:class,IComponent
        {
            BaseWorld.Instance.EntityManager.SetComponentData(entity,action);
        }
        public static bool RemoveComponet(this Entity entity, ComponentTypes componentTypes)
        {
            return BaseWorld.Instance.EntityManager.RemoveComponet(entity, componentTypes);
        }
        public static bool RemoveComponet<T>(this Entity entity) where T : class,IComponent
        {
            return BaseWorld.Instance.EntityManager.RemoveComponet<T>(entity);
        }
        public static T AddComponent<T>(this Entity entity) where T : class,IComponent, new()
        {
            return BaseWorld.Instance.EntityManager.AddComponent<T>(entity);
        }
        public static List<IComponent> AddComponent(this Entity entity, ComponentTypes componentTypes)
        {
            return BaseWorld.Instance.EntityManager.AddComponent(entity,componentTypes);
        }
        public static void Recycle(this Entity entity)
        {
            BaseWorld.Instance.EntityManager.RecycleEntity(entity);
        }
        public static void Destory(this Entity entity)
        {
            BaseWorld.Instance.EntityManager.DestroyEntity(entity);
        }
        public static bool IsExistComponent<T>(this Entity entity)where T:IComponent
        {
            foreach (var item in World.EntityManager.GetComponents(entity))
            {
                if (item is T)
                    return true;
            }
            return false;
        }
    }
}