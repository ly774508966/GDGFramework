using System;
using System.Collections;
using System.Collections.Generic;
using GDG.ModuleManager;
using GDG.Utils;
using UnityEngine;

namespace GDG.ECS
{
    public abstract class AbsEntity : UnityEngine.Object,IEquatable<AbsEntity>, IComparable<AbsEntity>,IInitable,IDestroyable,IRecyclable,IEnable
    {
        private ulong index;
        private int version;
        private uint typeId;
        private bool isActived;
        private List<IComponentData> components;
        internal List<IComponentData> Components { get => components; }
        public ulong Index { get => index; }
        public int Version { get => version; }
        public uint TypeId{ get => typeId; }
        public bool IsActived{ get => isActived; }
        internal void SetIndex(ulong index) => this.index = index;
        internal void SetVersion(int version) => this.version = version;
        internal void SetTypeId(uint typeId) => this.typeId = typeId;
        internal void SetActive(bool isActived) => this.isActived = isActived;
        internal AbsEntity(){}

        public bool Equals(AbsEntity other) => index == other.Index;

        public override bool Equals(object obj)
        {
            if (obj is AbsEntity entity)
            {
                return index == entity.Index && version == entity.Version;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int tempHash = 0;
            tempHash ^= index.GetHashCode();
            return tempHash;
        }
        public override string ToString() => $"Index:{this.index}，Version:{this.version}";
        public int CompareTo(AbsEntity other) => (int)index - (int)other.Index;

        public static bool operator ==(AbsEntity lhs, AbsEntity rhs) => lhs.Index == rhs.Index && lhs.Version == rhs.Version;
        public static bool operator !=(AbsEntity lhs, AbsEntity rhs) => lhs.Index != rhs.Index || lhs.Version != rhs.Version;
        public static AbsEntity operator ++(AbsEntity entity)
        {
            entity.SetVersion(entity.Version + 1);
            return entity;
        }
        public static AbsEntity operator --(AbsEntity entity)
        {
            entity.SetVersion(entity.Version - 1);
            return entity;
        }
        public virtual void OnInit()
        {
            components = new List<IComponentData>();
            version = 1;
            isActived = true;
        }

        public virtual void OnDestroy()
        {
            isActived = false;
        }

        public virtual void OnRecycle()
        {
            version++;
            isActived = false;
        }
        public virtual void OnEnable()
        {
            isActived = true;
        }
        internal void AddComponentToList(IComponentData component)
        {
            if(!components.Contains(component))
                //LogManager.Instance.LogError($"AddComponent failed! Add a repeated Component:{component.GetType()}");
            components.Add(component);
        }
        internal bool RemoveComponentToList(IComponentData component)
        {
            return components.Remove(component);
        }
        public virtual void CopyTo(AbsEntity entity)
        {

        }
    }
}