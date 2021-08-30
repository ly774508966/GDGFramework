using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ModuleManager;
using System;
using System.Linq;
namespace GDG.ECS
{
    public abstract class AbsSystem<ST> : ISystem where ST : AbsSystem<ST>, new()
    {
        #region 单例
        private static ST instance;
        public static ST GetInstance() { if (instance == null) { instance = new ST(); instance.Init(); } return instance; }
        #endregion
        private List<AbsEntity> entities;
        
        public List<AbsEntity> Entities { get => this.entities; }

        private bool isActived = true;
        internal void Init()
        {
            entities = new List<AbsEntity>();
            BaseWorld.Instance.Systems.Add(this);
            OnStart();
        }
        public void SetEntities(List<AbsEntity> entities)
        {
            this.entities = entities;
        }
        public void SetActive(bool isActived)
        {
            this.isActived = isActived;
            if(isActived)
            {
                BaseWorld.Instance.AddOrRemoveSystemFromMonoWorld(this);
                OnEnable();
            }
            else
            {
                BaseWorld.Instance.AddOrRemoveSystemFromMonoWorld(this,false);
                OnDisable();
            }
        }
        public bool IsActived() => this.isActived;
        public virtual void OnStart() { }
        public virtual void OnEnable() { }
        public abstract void OnUpdate();
        public virtual void OnDisable() { }
        //组装Handle
        private SystemHandle AssembleSystemHandle(IEnumerable<AbsEntity> queryResult, Action<AbsEntity> callback)
        {
            SystemHandle handle = new SystemHandle();
            handle.result = queryResult;
            handle.callback = callback;
            return handle;
        }
        //组件匹配，如果有该类型的组件则匹配成功
        private bool IsComponentMatch(AbsEntity entity, IComponentData component)
        {
            var type = component.GetType();
            var query =
                from result in entity.Components
                where result.GetType() == type
                select result;

            if (query.Count<IComponentData>() != 0)
            {
                return true;
            }
            return false;
        }
        private SystemHandle ForEach(Action<AbsEntity> callback, params IComponentData[] components)
        {
            List<AbsEntity> query = new List<AbsEntity>();
            if (entities == null)
                return null;
            foreach (var item in entities)
            {
                var isSucced = true;
                if (components != null)
                {
                    foreach (var component in components)
                    {
                        if (!IsComponentMatch(item, component))
                        {
                            isSucced = false;
                            break;
                        }
                    }
                }
                if (isSucced)
                    query.Add(item);
            }
            Debug.Log(query.Count);
            return AssembleSystemHandle(query, callback);
        }
        #region Select    
        protected SystemHandle Select(Action<AbsEntity> callback)
        {
            return ForEach(callback, null);
        }
        # pragma warning disable 0170
        protected SystemHandle Select<T>(out T component, Action<AbsEntity> callback) where T : IComponentData
        {
            component = default(T);
            return ForEach(callback, component);
        }
        protected SystemHandle Select<T1, T2>(out T1 component1, out T2 component2, Action<AbsEntity> callback) where T1 : IComponentData where T2 : IComponentData
        {
            component1 = default(T1);
            component2 = default(T2);
            return ForEach(callback, component1, component2);
        }
        protected SystemHandle Select<T1, T2, T3>(out T1 component1, out T2 component2, out T3 component3, Action<AbsEntity> callback) where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData
        {
            component1 = default(T1);
            component2 = default(T2);
            component3 = default(T3);
            return ForEach(callback, component1, component2, component3);
        }
        protected SystemHandle Select<T1, T2, T3, T4>(out T1 component1, out T2 component2, out T3 component3, out T4 component4, Action<AbsEntity> callback) where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData
        {
            component1 = default(T1);
            component2 = default(T2);
            component3 = default(T3);
            component4 = default(T4);
            return ForEach(callback, component1, component2, component3, component4);
        }
        protected SystemHandle Select<T1, T2, T3, T4, T5>(out T1 component1, out T2 component2, out T3 component3, out T4 component4, out T5 component5, Action<AbsEntity> callback) where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData where T5 : IComponentData
        {
            component1 = default(T1);
            component2 = default(T2);
            component3 = default(T3);
            component4 = default(T4);
            component5 = default(T5);
            return ForEach(callback, component1, component2, component3, component4, component5);
        }
        protected SystemHandle Select<T1, T2, T3, T4, T5, T6>(out T1 component1, out T2 component2, out T3 component3, out T4 component4, out T5 component5, out T6 component6, Action<AbsEntity> callback) where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData where T5 : IComponentData where T6 : IComponentData
        {
            component1 = default(T1);
            component2 = default(T2);
            component3 = default(T3);
            component4 = default(T4);
            component5 = default(T5);
            component6 = default(T6);
            return ForEach(callback, component1, component2, component3, component4, component5, component6);
        }
        protected SystemHandle Select<T1, T2, T3, T4, T5, T6, T7>(out T1 component1, out T2 component2, out T3 component3, out T4 component4, out T5 component5, out T6 component6, out T7 component7, Action<AbsEntity> callback) where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData where T5 : IComponentData where T6 : IComponentData where T7 : IComponentData
        {
            component1 = default(T1);
            component2 = default(T2);
            component3 = default(T3);
            component4 = default(T4);
            component5 = default(T5);
            component6 = default(T6);
            component7 = default(T7);
            return ForEach(callback, component1, component2, component3, component4, component5, component6, component7);
        }
        protected SystemHandle Select<T1, T2, T3, T4, T5, T6, T7, T8>(out T1 component1, out T2 component2, out T3 component3, out T4 component4, out T5 component5, out T6 component6, out T7 component7, T8 component8, Action<AbsEntity> callback) where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData where T5 : IComponentData where T6 : IComponentData where T7 : IComponentData where T8 : IComponentData
        {
            component1 = default(T1);
            component2 = default(T2);
            component3 = default(T3);
            component4 = default(T4);
            component5 = default(T5);
            component6 = default(T6);
            component7 = default(T7);
            component8 = default(T8);
            return ForEach(callback, component1, component2, component3, component4, component5, component6, component7, component8);
        }
        protected SystemHandle Select<T1, T2, T3, T4, T5, T6, T7, T8, T9>(out T1 component1, out T2 component2, out T3 component3, out T4 component4, out T5 component5, out T6 component6, out T7 component7, T8 component8, T9 component9, Action<AbsEntity> callback) where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData where T5 : IComponentData where T6 : IComponentData where T7 : IComponentData where T8 : IComponentData where T9 : IComponentData
        {
            component1 = default(T1);
            component2 = default(T2);
            component3 = default(T3);
            component4 = default(T4);
            component5 = default(T5);
            component6 = default(T6);
            component7 = default(T7);
            component8 = default(T8);
            component9 = default(T9);
            return ForEach(callback, component1, component2, component3, component4, component5, component6, component7, component8, component9);
        }
        protected SystemHandle Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(out T1 component1, out T2 component2, out T3 component3, out T4 component4, out T5 component5, out T6 component6, out T7 component7, T8 component8, T9 component9, T10 component10, Action<AbsEntity> callback) where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData where T4 : IComponentData where T5 : IComponentData where T6 : IComponentData where T7 : IComponentData where T8 : IComponentData where T9 : IComponentData where T10 : IComponentData
        {
            component1 = default(T1);
            component2 = default(T2);
            component3 = default(T3);
            component4 = default(T4);
            component5 = default(T5);
            component6 = default(T6);
            component7 = default(T7);
            component8 = default(T8);
            component9 = default(T9);
            component10 = default(T10);
            return ForEach(callback, component1, component2, component3, component4, component5, component6, component7, component8, component9, component10);
        }
        #endregion
    }
}