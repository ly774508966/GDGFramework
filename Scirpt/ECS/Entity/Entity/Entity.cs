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
        internal Action<Entity> initCallback;
        internal Action<Entity> enableCallback;
        internal Action<Entity> recycleCallback;
        internal Action<Entity> destroyCallback;
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
            
            initCallback?.Invoke(this);
            setNameCallBack?.Invoke(this);
            Version = 1;
            IsActived = true;
        }
        internal virtual void OnEnable()
        {
            enableCallback?.Invoke(this);
            IsActived = true;
            
        }
        internal virtual void OnRecycle()
        {
            recycleCallback?.Invoke(this);
            Version++;
            IsActived = false;
        }
        internal virtual void OnDestroy()
        {
            destroyCallback?.Invoke(this);
            IsActived = false;
            
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
}