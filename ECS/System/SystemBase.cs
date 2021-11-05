using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ModuleManager;
using System;
using System.Linq;
using UnityEngine.Events;

namespace GDG.ECS
{
    public abstract class SystemBase<ST> : ISystem where ST : SystemBase<ST>, new()
    {
        #region 单例
        private static ST instance;
        public static ST GetInstance() { if (instance == null) { instance = new ST(); instance.Init(); } return instance; }
        #endregion
        private bool isActived = true;
        private List<Entity> entities;
        private Dictionary<ulong, List<ExcuteInfo>> indexExcuteListMapping;
        private Dictionary<int, ExcuteInfo> selectIdExcuteInfoListMapping;
        private Dictionary<ExcuteInfo, List<ulong>> excuteInfoEntityListMapping;
        public Dictionary<int, ExcuteInfo> m_SelectId2ExcuteInfo { get => selectIdExcuteInfoListMapping; }
        public Dictionary<ulong, List<ExcuteInfo>> m_Index2ExcuteInfoListMapping{ get => indexExcuteListMapping; }
        public Dictionary<ExcuteInfo, List<ulong>> m_ExcuteInfo2EntityListMapping{ get => excuteInfoEntityListMapping; }
        public List<Entity> Entities { get => this.entities; }
        private void Init()
        {
            indexExcuteListMapping = new Dictionary<ulong, List<ExcuteInfo>>();
            selectIdExcuteInfoListMapping = new Dictionary<int, ExcuteInfo>();
            excuteInfoEntityListMapping = new Dictionary<ExcuteInfo, List<ulong>>();
            entities = new List<Entity>();
            BaseWorld.Instance.Systems.Add(typeof(ST), this);
        }
        public void SetEntities(List<Entity> entities)
        {
            this.entities = entities;
        }
        public void SetActive(bool isActived)
        {
            if(isActived == this.isActived)
                return;

            this.isActived = isActived;
            if (isActived)
            {
                BaseWorld.Instance.AddOrRemoveSystemFromMonoWorld(this);
                OnEnable();
            }
            else
            {
                BaseWorld.Instance.AddOrRemoveSystemFromMonoWorld(this, false);
                OnDisable();
            }
        }
        public void AddEntity(Entity entity)
        {
            this.entities.Add(entity);
        }
        public bool RemoveEntity(Entity entity)
        {
            if(m_Index2ExcuteInfoListMapping.TryGetValue(entity.Index,out List<ExcuteInfo> excuteInfos))
            {
                foreach(var excuteInfo in excuteInfos)
                {
                    if(m_ExcuteInfo2EntityListMapping.TryGetValue(excuteInfo,out List<ulong> indexList))
                    {
                        if(!indexList.Remove(entity.Index))
                            return false;
                    }                    
                }
            }
            else
                return false;
            if(!Entities.Remove(entity))
                return false;
            return true;
        }
        public bool IsActived() => this.isActived;
        public abstract void OnStart();
        public virtual void OnEnable() { }
        public virtual void OnFixedUpdate() { }
        public abstract void OnUpdate();
        public virtual void OnLateUpdate() { }
        public virtual void OnDisable() { }
        #region E
        private SystemHandle<E> AssembleSystemHandle<E>(IEnumerable<E> queryResult, SystemCallback<E> callback) where E : Entity
        {
            SystemHandle<E> handle = new SystemHandle<E>();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            return handle;
        }
        private SystemHandle<E> ForEach<E>(SystemCallback<E> callback) where E : Entity
        {
            if (entities == null)
                return null;
            var result =
            from obj in entities.AsParallel()
            select obj as E;
            return AssembleSystemHandle<E>(result, callback);
        }
        public SystemHandle<E> Select<E>(SystemCallback<E> callback) where E : Entity
        {
            if (entities == null)
                return null;
            return ForEach<E>(callback);
        }
        #endregion
        #region E,T
        private SystemHandle<E, T> AssembleSystemHandle<E, T>(IEnumerable<E> queryResult, SystemCallback<E, T> callback) where E : Entity where T : class, IComponent
        {
            SystemHandle<E, T> handle = new SystemHandle<E, T>();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            return handle;
        }
        private SystemHandle<E, T> ForEach<E, T>(SystemCallback<E, T> callback) where E : Entity where T : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T>.None;
            var result =
            from obj in entities.AsParallel()
            where obj.IsExistComponent<T>()
            select obj as E;

            return AssembleSystemHandle<E, T>(result, callback);
        }
        public SystemHandle<E, T> Select<E, T>(SystemCallback<E, T> callback) where E : Entity where T : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T>.None;
            return ForEach<E, T>(callback);
        }
        #endregion
        #region E,T1,T2
        private SystemHandle<E, T1, T2> AssembleSystemHandle<E, T1, T2>(IEnumerable<E> queryResult, SystemCallback<E, T1, T2> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent
        {
            SystemHandle<E, T1, T2> handle = new SystemHandle<E, T1, T2>();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            return handle;
        }
        private SystemHandle<E, T1, T2> ForEach<E, T1, T2>(SystemCallback<E, T1, T2> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E,T1,T2>.None;;
            var result =
            from obj in entities.AsParallel()
            where obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>()
            select obj as E;

            return AssembleSystemHandle<E, T1, T2>(result, callback);
        }
        public SystemHandle<E, T1, T2> Select<E, T1, T2>(SystemCallback<E, T1, T2> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T1,T2>.None;
            return ForEach<E, T1, T2>(callback);
        }
        #endregion
        #region E,T1,T2,T3
        private SystemHandle<E, T1, T2, T3> AssembleSystemHandle<E, T1, T2, T3>(IEnumerable<E> queryResult, SystemCallback<E, T1, T2, T3> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
        {
            SystemHandle<E, T1, T2, T3> handle = new SystemHandle<E, T1, T2, T3>();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            return handle;
        }
        private SystemHandle<E, T1, T2, T3> ForEach<E, T1, T2, T3>(SystemCallback<E, T1, T2, T3> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T1, T2, T3>.None;
            var result =
            from obj in entities.AsParallel()
            where obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>() && obj.IsExistComponent<T3>()
            select obj as E;

            return AssembleSystemHandle<E, T1, T2, T3>(result, callback);
        }
        public SystemHandle<E, T1, T2, T3> Select<E, T1, T2, T3>(SystemCallback<E, T1, T2, T3> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T1, T2, T3>.None;
            return ForEach<E, T1, T2, T3>(callback);
        }
        #endregion
        #region E,T1,T2,T3,T4
        private SystemHandle<E, T1, T2, T3, T4> AssembleSystemHandle<E, T1, T2, T3, T4>(IEnumerable<E> queryResult, SystemCallback<E, T1, T2, T3, T4> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
        {
            SystemHandle<E, T1, T2, T3, T4> handle = new SystemHandle<E, T1, T2, T3, T4>();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            return handle;
        }
        private SystemHandle<E, T1, T2, T3, T4> ForEach<E, T1, T2, T3, T4>(SystemCallback<E, T1, T2, T3, T4> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T1, T2, T3, T4>.None;
            var result =
            from obj in entities.AsParallel()
            where obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>() && obj.IsExistComponent<T3>() && obj.IsExistComponent<T4>()
            select obj as E;

            return AssembleSystemHandle<E, T1, T2, T3, T4>(result, callback);
        }
        public SystemHandle<E, T1, T2, T3, T4> Select<E, T1, T2, T3, T4>(SystemCallback<E, T1, T2, T3, T4> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T1, T2, T3, T4>.None;
            return ForEach<E, T1, T2, T3, T4>(callback);
        }
        #endregion
        #region E,T1,T2,T3,T4,T5
        private SystemHandle<E, T1, T2, T3, T4, T5> AssembleSystemHandle<E, T1, T2, T3, T4, T5>(IEnumerable<E> queryResult, SystemCallback<E, T1, T2, T3, T4, T5> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent
        {
            SystemHandle<E, T1, T2, T3, T4, T5> handle = new SystemHandle<E, T1, T2, T3, T4, T5>();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            return handle;
        }
        private SystemHandle<E, T1, T2, T3, T4, T5> ForEach<E, T1, T2, T3, T4, T5>(SystemCallback<E, T1, T2, T3, T4, T5> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T1, T2, T3, T4, T5>.None;
            var result =
            from obj in entities.AsParallel()
            where obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>() && obj.IsExistComponent<T3>() && obj.IsExistComponent<T4>() && obj.IsExistComponent<T5>()
            select obj as E;

            return AssembleSystemHandle<E, T1, T2, T3, T4, T5>(result, callback);
        }
        public SystemHandle<E, T1, T2, T3, T4, T5> Select<E, T1, T2, T3, T4, T5>(SystemCallback<E, T1, T2, T3, T4, T5> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T1, T2, T3, T4, T5>.None;
            return ForEach<E, T1, T2, T3, T4, T5>(callback);
        }
        #endregion
        #region E,T1,T2,T3,T4,T5,T6
        private SystemHandle<E, T1, T2, T3, T4, T5, T6> AssembleSystemHandle<E, T1, T2, T3, T4, T5, T6>(IEnumerable<E> queryResult, SystemCallback<E, T1, T2, T3, T4, T5, T6> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent
        {
            SystemHandle<E, T1, T2, T3, T4, T5, T6> handle = new SystemHandle<E, T1, T2, T3, T4, T5, T6>();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            return handle;
        }
        private SystemHandle<E, T1, T2, T3, T4, T5, T6> ForEach<E, T1, T2, T3, T4, T5, T6>(SystemCallback<E, T1, T2, T3, T4, T5, T6> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T1, T2, T3, T4, T5, T6>.None;
            var result =
            from obj in entities.AsParallel()
            where obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>() && obj.IsExistComponent<T3>() && obj.IsExistComponent<T4>() && obj.IsExistComponent<T5>() && obj.IsExistComponent<T6>()
            select obj as E;

            return AssembleSystemHandle<E, T1, T2, T3, T4, T5, T6>(result, callback);
        }
        public SystemHandle<E, T1, T2, T3, T4, T5, T6> Select<E, T1, T2, T3, T4, T5, T6>(SystemCallback<E, T1, T2, T3, T4, T5, T6> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T1, T2, T3, T4, T5, T6>.None;
            return ForEach<E, T1, T2, T3, T4, T5, T6>(callback);
        }
        #endregion
        #region E,T1,T2,T3,T4,T5,T6,T7
        private SystemHandle<E, T1, T2, T3, T4, T5, T6, T7> AssembleSystemHandle<E, T1, T2, T3, T4, T5, T6, T7>(IEnumerable<E> queryResult, SystemCallback<E, T1, T2, T3, T4, T5, T6, T7> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent
        {
            SystemHandle<E, T1, T2, T3, T4, T5, T6, T7> handle = new SystemHandle<E, T1, T2, T3, T4, T5, T6, T7>();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            return handle;
        }
        private SystemHandle<E, T1, T2, T3, T4, T5, T6, T7> ForEach<E, T1, T2, T3, T4, T5, T6, T7>(SystemCallback<E, T1, T2, T3, T4, T5, T6, T7> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T1, T2, T3, T4, T5, T6, T7>.None;
            var result =
            from obj in entities.AsParallel()
            where obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>() && obj.IsExistComponent<T3>() && obj.IsExistComponent<T4>() && obj.IsExistComponent<T5>() && obj.IsExistComponent<T6>() && obj.IsExistComponent<T7>()
            select obj as E;

            return AssembleSystemHandle<E, T1, T2, T3, T4, T5, T6, T7>(result, callback);
        }
        public SystemHandle<E, T1, T2, T3, T4, T5, T6, T7> Select<E, T1, T2, T3, T4, T5, T6, T7>(SystemCallback<E, T1, T2, T3, T4, T5, T6, T7> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T1, T2, T3, T4, T5, T6, T7>.None;
            return ForEach<E, T1, T2, T3, T4, T5, T6, T7>(callback);
        }
        #endregion
        #region E,T1,T2,T3,T4,T5,T6,T7,T8
        private SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8> AssembleSystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8>(IEnumerable<E> queryResult, SystemCallback<E, T1, T2, T3, T4, T5, T6, T7, T8> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent where T8 : class, IComponent
        {
            SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8> handle = new SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8>();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            return handle;
        }
        private SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8> ForEach<E, T1, T2, T3, T4, T5, T6, T7, T8>(SystemCallback<E, T1, T2, T3, T4, T5, T6, T7, T8> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent where T8 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8>.None;
            var result =
            from obj in entities.AsParallel()
            where obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>() && obj.IsExistComponent<T3>() && obj.IsExistComponent<T4>() && obj.IsExistComponent<T5>() && obj.IsExistComponent<T6>() && obj.IsExistComponent<T7>() && obj.IsExistComponent<T8>()
            select obj as E;

            return AssembleSystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8>(result, callback);
        }
        public SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8> Select<E, T1, T2, T3, T4, T5, T6, T7, T8>(SystemCallback<E, T1, T2, T3, T4, T5, T6, T7, T8> callback) where E : Entity where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent where T8 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<E, T1, T2, T3, T4, T5, T6, T7, T8>.None;
            return ForEach<E, T1, T2, T3, T4, T5, T6, T7, T8>(callback);
            #endregion
        }
    }
}