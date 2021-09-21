using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using GDG.ModuleManager;
namespace GDG.ECS
{
    public class ComponentTypes : ICollection<Type>,IEquatable<ComponentTypes>
    {
        public ComponentTypes(params Type[] componentTypes)
        {
            ComponentTypesList = new List<Type>();
            foreach(var item in componentTypes)
            {
                if(ComponentTypesList.Contains(item))
                    continue;

                if(!typeof(IComponent).IsAssignableFrom(item))
                    LogManager.Instance.LogError($"Try to add a wrong type into ComponentTypes! Type:{item.GetType()}");
                ComponentTypesList.Add(item);
            }

            RequestTypeId();
        }

        public List<Type> ComponentTypesList{ get; private set; }
        private int entityRefCount;//实体对组件的引用计数
        private uint typeId;
        public int EntityRefCount{ get => entityRefCount; }
        public uint TypeId{ get => typeId; }//组件类型唯一ID,为0时是未向世界申请的组件
        public bool IsRequested{ get => typeId != 0; }//是否已向世界申请typeId
        public int Count {get=>ComponentTypesList.Count;}
        public bool IsReadOnly { get; }
        internal void SetTypeId(uint typeId) => this.typeId = typeId;
        internal void SetEntityRefCount(int entityRefCount) => this.entityRefCount = entityRefCount;
        public void RequestTypeId()
        {
            var tempTypeId = BaseWorld.Instance.EntityManager.GetTypeId(this);
            if(tempTypeId>0)
            {
                 typeId = tempTypeId;
            }
            else
            {
                typeId = BaseWorld.Instance.EntityManager.RequestTypeId(this);
            }
        }
        public void Add(Type item)
        {
            if(!typeof(IComponent).IsAssignableFrom(item)|| Contains(item))
                return;
            ComponentTypesList.Add(item);

            RequestTypeId();
        }
        public bool Remove(Type item)
        {
            if(Contains(item))
            {
                return  ComponentTypesList.Remove(item);
            }
            return false;
        }
        /// <summary>
        /// 使用此方法将会导致组件变成未申请状态
        /// </summary>
        public void Clear()
        {
            ComponentTypesList.Clear();
            typeId = 0;
            entityRefCount = 0;
        }

        public bool Contains(Type item)
        {
            return ComponentTypesList.Contains(item);
        }

        public void CopyTo(Type[] array, int arrayIndex)
        {
            ComponentTypesList.CopyTo(array, arrayIndex);
        }
        public IEnumerator<Type> GetEnumerator()
        {
            foreach(var component in ComponentTypesList) yield return component;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach(var component in ComponentTypesList) yield return component;
        }

        public bool Equals(ComponentTypes other)
        {
            var otherList = other.ComponentTypesList;
            if (otherList.Count != ComponentTypesList.Count)
                return false;
            foreach(var item in other.ComponentTypesList)
            {
                if(!ComponentTypesList.Contains(item))
                    return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            int tempHash = 0;
            tempHash ^= TypeId.GetHashCode();
            return tempHash;
        }
        public override string ToString()
        {
            string str = "{";
            foreach(var item in ComponentTypesList)
            {
                str += item + ", ";
            }
            str = Regex.Replace(str, ", (?!.*, )", "");
            return str + "}";
        }
    }
}