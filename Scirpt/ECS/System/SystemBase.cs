using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ModuleManager;
using System;
using System.Linq;
using UnityEngine.Events;
using GDG.Utils;

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
        private Dictionary<ulong, List<int>> entity2SelectIdListMapping;
        private Dictionary<int, ExcuteInfo> selectId2ExcuteInfoListMapping;
        public List<Entity> Entities { get => this.entities; }
        private void Init()
        {
            entity2SelectIdListMapping = new Dictionary<ulong, List<int>>();
            selectId2ExcuteInfoListMapping = new Dictionary<int, ExcuteInfo>();
            entities = new List<Entity>();
            BaseWorld.Instance.Systems.Add(typeof(ST), this);
        }
        #region ISystem
        public void SetEntities(List<Entity> entities)
        {
            this.entities = entities;
        }
        public void SetActive(bool isActived)
        {
            if (isActived == this.isActived)
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
            if (Entities.Contains(entity))
            {
                if (!Entities.Remove(entity))
                    return false;
                if (entity2SelectIdListMapping.ContainsKey(entity.Index))
                {
                    return entity2SelectIdListMapping.Remove(entity.Index);
                }
            }
            return true;
        }
        public bool IsActived() => this.isActived;
        public abstract void OnStart();
        public virtual void OnEnable() { }
        public virtual void OnFixedUpdate() { }
        public abstract void OnUpdate();
        public virtual void OnLateUpdate() { }
        public virtual void OnDisable() { }
        public bool TryGetExcuteInfos(ulong index, out List<int> excuteInfo)
        {
            return entity2SelectIdListMapping.TryGetValue(index, out excuteInfo);
        }
        public bool TryGetExcuteInfo(int selectedId, out ExcuteInfo excuteInfo)
        {
            return selectId2ExcuteInfoListMapping.TryGetValue(selectedId, out excuteInfo);
        }
        public void AddSelectId2ExcuteInfoMapping(int selectedId, ExcuteInfo excuteInfo)
        {
            selectId2ExcuteInfoListMapping.Add(selectedId, excuteInfo);
        }
        public bool RemoveSelectId2ExcuteInfoMapping(int selectedId)
        {
            return selectId2ExcuteInfoListMapping.Remove(selectedId);
        }
        public void AddEntity2SelectIdMapping(ulong index, List<int> excuteInfos)
        {
            entity2SelectIdListMapping.Add(index, excuteInfos);
        }
        public bool RemoveEntity2ExcuteInfosMapping(ulong index)
        {
            return entity2SelectIdListMapping.Remove(index);
        }

        #endregion

        #region E
        List<Entity> result = new List<Entity>();
        private SystemHandle AssembleSystemHandle(List<Entity> queryResult, SystemCallback callback)
        {
            SystemHandle handle = ObjectPool<SystemHandle>.Instance.Pop();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            handle.excuteInfo.Init();
            return handle;
        }
        private SystemHandle ForEach(SystemCallback callback)
        {
            if (entities == null)
                return null;

            result.Clear();
            foreach (var obj in entities)
            {
                result.Add(obj);
            }
            return AssembleSystemHandle(result, callback);
        }
        public SystemHandle Select(SystemCallback callback)
        {
            if (entities == null)
                return null;
            return ForEach(callback);
        }
        #endregion
        #region E,T
        private SystemHandle<T> AssembleSystemHandle<T>(List<Entity> queryResult, SystemCallback<T> callback) where T : class, IComponent
        {
            SystemHandle<T> handle = ObjectPool<SystemHandle<T>>.Instance.Pop();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            handle.excuteInfo.Init();
            return handle;
        }
        private SystemHandle<T> ForEach<T>(SystemCallback<T> callback) where T : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T>.None;

            result.Clear();
            foreach (var obj in entities)
            {
                if (obj.IsExistComponent<T>())
                    result.Add(obj);
            }
            return AssembleSystemHandle<T>(result, callback);
        }
        public SystemHandle<T> Select<T>(SystemCallback<T> callback) where T : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T>.None;
            return ForEach<T>(callback);
        }
        #endregion
        #region E,T1,T2
        private SystemHandle<T1, T2> AssembleSystemHandle<T1, T2>(List<Entity> queryResult, SystemCallback<T1, T2> callback) where T1 : class, IComponent where T2 : class, IComponent
        {
            SystemHandle<T1, T2> handle = ObjectPool<SystemHandle<T1, T2>>.Instance.Pop();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            handle.excuteInfo.Init();
            return handle;
        }
        private SystemHandle<T1, T2> ForEach<T1, T2>(SystemCallback<T1, T2> callback) where T1 : class, IComponent where T2 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2>.None; ;

            result.Clear();
            foreach (var obj in entities)
            {
                if (obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>())
                    result.Add(obj);
            }

            return AssembleSystemHandle<T1, T2>(result, callback);
        }
        public SystemHandle<T1, T2> Select<T1, T2>(SystemCallback<T1, T2> callback) where T1 : class, IComponent where T2 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2>.None;
            return ForEach<T1, T2>(callback);
        }
        #endregion
        #region E,T1,T2,T3
        private SystemHandle<T1, T2, T3> AssembleSystemHandle<T1, T2, T3>(List<Entity> queryResult, SystemCallback<T1, T2, T3> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
        {
            SystemHandle<T1, T2, T3> handle = ObjectPool<SystemHandle<T1, T2, T3>>.Instance.Pop();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            handle.excuteInfo.Init();
            return handle;
        }
        private SystemHandle<T1, T2, T3> ForEach<T1, T2, T3>(SystemCallback<T1, T2, T3> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2, T3>.None;
            result.Clear();
            foreach (var obj in entities)
            {
                if (obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>() && obj.IsExistComponent<T3>())
                    result.Add(obj);
            }

            return AssembleSystemHandle<T1, T2, T3>(result, callback);
        }
        public SystemHandle<T1, T2, T3> Select<T1, T2, T3>(SystemCallback<T1, T2, T3> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2, T3>.None;
            return ForEach<T1, T2, T3>(callback);
        }
        #endregion
        #region E,T1,T2,T3,T4
        private SystemHandle<T1, T2, T3, T4> AssembleSystemHandle<T1, T2, T3, T4>(List<Entity> queryResult, SystemCallback<T1, T2, T3, T4> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
        {
            SystemHandle<T1, T2, T3, T4> handle = ObjectPool<SystemHandle<T1, T2, T3, T4>>.Instance.Pop();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            handle.excuteInfo.Init();
            return handle;
        }
        private SystemHandle<T1, T2, T3, T4> ForEach<T1, T2, T3, T4>(SystemCallback<T1, T2, T3, T4> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2, T3, T4>.None;
            result.Clear();
            foreach (var obj in entities)
            {
                if (obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>() && obj.IsExistComponent<T3>() && obj.IsExistComponent<T4>())
                    result.Add(obj);
            }

            return AssembleSystemHandle<T1, T2, T3, T4>(result, callback);
        }
        public SystemHandle<T1, T2, T3, T4> Select<T1, T2, T3, T4>(SystemCallback<T1, T2, T3, T4> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2, T3, T4>.None;
            return ForEach<T1, T2, T3, T4>(callback);
        }
        #endregion
        #region E,T1,T2,T3,T4,T5
        private SystemHandle<T1, T2, T3, T4, T5> AssembleSystemHandle<T1, T2, T3, T4, T5>(List<Entity> queryResult, SystemCallback<T1, T2, T3, T4, T5> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent
        {
            SystemHandle<T1, T2, T3, T4, T5> handle = ObjectPool<SystemHandle<T1, T2, T3, T4, T5>>.Instance.Pop();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            handle.excuteInfo.Init();
            return handle;
        }
        private SystemHandle<T1, T2, T3, T4, T5> ForEach<T1, T2, T3, T4, T5>(SystemCallback<T1, T2, T3, T4, T5> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2, T3, T4, T5>.None;
            result.Clear();
            foreach (var obj in entities)
            {
                if (obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>() && obj.IsExistComponent<T3>() && obj.IsExistComponent<T4>() && obj.IsExistComponent<T5>())
                    result.Add(obj);
            }

            return AssembleSystemHandle<T1, T2, T3, T4, T5>(result, callback);
        }
        public SystemHandle<T1, T2, T3, T4, T5> Select<T1, T2, T3, T4, T5>(SystemCallback<T1, T2, T3, T4, T5> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2, T3, T4, T5>.None;
            return ForEach<T1, T2, T3, T4, T5>(callback);
        }
        #endregion
        #region E,T1,T2,T3,T4,T5,T6
        private SystemHandle<T1, T2, T3, T4, T5, T6> AssembleSystemHandle<T1, T2, T3, T4, T5, T6>(List<Entity> queryResult, SystemCallback<T1, T2, T3, T4, T5, T6> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent
        {
            SystemHandle<T1, T2, T3, T4, T5, T6> handle = ObjectPool<SystemHandle<T1, T2, T3, T4, T5, T6>>.Instance.Pop();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            handle.excuteInfo.Init();
            return handle;
        }
        private SystemHandle<T1, T2, T3, T4, T5, T6> ForEach<T1, T2, T3, T4, T5, T6>(SystemCallback<T1, T2, T3, T4, T5, T6> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2, T3, T4, T5, T6>.None;
            result.Clear();
            foreach (var obj in entities)
            {
                if (obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>() && obj.IsExistComponent<T3>() && obj.IsExistComponent<T4>() && obj.IsExistComponent<T5>() && obj.IsExistComponent<T6>())
                    result.Add(obj);
            }

            return AssembleSystemHandle<T1, T2, T3, T4, T5, T6>(result, callback);
        }
        public SystemHandle<T1, T2, T3, T4, T5, T6> Select<T1, T2, T3, T4, T5, T6>(SystemCallback<T1, T2, T3, T4, T5, T6> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2, T3, T4, T5, T6>.None;
            return ForEach<T1, T2, T3, T4, T5, T6>(callback);
        }
        #endregion
        #region E,T1,T2,T3,T4,T5,T6,T7
        private SystemHandle<T1, T2, T3, T4, T5, T6, T7> AssembleSystemHandle<T1, T2, T3, T4, T5, T6, T7>(List<Entity> queryResult, SystemCallback<T1, T2, T3, T4, T5, T6, T7> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent
        {
            SystemHandle<T1, T2, T3, T4, T5, T6, T7> handle = ObjectPool<SystemHandle<T1, T2, T3, T4, T5, T6, T7>>.Instance.Pop();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            handle.excuteInfo.Init();
            return handle;
        }
        private SystemHandle<T1, T2, T3, T4, T5, T6, T7> ForEach<T1, T2, T3, T4, T5, T6, T7>(SystemCallback<T1, T2, T3, T4, T5, T6, T7> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2, T3, T4, T5, T6, T7>.None;
            result.Clear();
            foreach (var obj in entities)
            {
                if (obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>() && obj.IsExistComponent<T3>() && obj.IsExistComponent<T4>() && obj.IsExistComponent<T5>() && obj.IsExistComponent<T6>() && obj.IsExistComponent<T7>())
                    result.Add(obj);
            }

            return AssembleSystemHandle<T1, T2, T3, T4, T5, T6, T7>(result, callback);
        }
        public SystemHandle<T1, T2, T3, T4, T5, T6, T7> Select<T1, T2, T3, T4, T5, T6, T7>(SystemCallback<T1, T2, T3, T4, T5, T6, T7> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2, T3, T4, T5, T6, T7>.None;
            return ForEach<T1, T2, T3, T4, T5, T6, T7>(callback);
        }
        #endregion
        #region E,T1,T2,T3,T4,T5,T6,T7,T8
        private SystemHandle<T1, T2, T3, T4, T5, T6, T7, T8> AssembleSystemHandle<T1, T2, T3, T4, T5, T6, T7, T8>(List<Entity> queryResult, SystemCallback<T1, T2, T3, T4, T5, T6, T7, T8> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent where T8 : class, IComponent
        {
            SystemHandle<T1, T2, T3, T4, T5, T6, T7, T8> handle = ObjectPool<SystemHandle<T1, T2, T3, T4, T5, T6, T7, T8>>.Instance.Pop();
            handle.system = this;
            handle.result = queryResult;
            handle.callback = callback;
            handle.excuteInfo.Init();
            return handle;
        }
        private SystemHandle<T1, T2, T3, T4, T5, T6, T7, T8> ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(SystemCallback<T1, T2, T3, T4, T5, T6, T7, T8> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent where T8 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2, T3, T4, T5, T6, T7, T8>.None;
            result.Clear();
            foreach (var obj in entities)
            {
                if (obj.IsExistComponent<T1>() && obj.IsExistComponent<T2>() && obj.IsExistComponent<T3>() && obj.IsExistComponent<T4>() && obj.IsExistComponent<T5>() && obj.IsExistComponent<T6>() && obj.IsExistComponent<T7>() && obj.IsExistComponent<T8>())
                    result.Add(obj);
            }

            return AssembleSystemHandle<T1, T2, T3, T4, T5, T6, T7, T8>(result, callback);
        }
        public SystemHandle<T1, T2, T3, T4, T5, T6, T7, T8> Select<T1, T2, T3, T4, T5, T6, T7, T8>(SystemCallback<T1, T2, T3, T4, T5, T6, T7, T8> callback) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent where T5 : class, IComponent where T6 : class, IComponent where T7 : class, IComponent where T8 : class, IComponent
        {
            if (entities == null)
                return SystemHandle<T1, T2, T3, T4, T5, T6, T7, T8>.None;
            return ForEach<T1, T2, T3, T4, T5, T6, T7, T8>(callback);
            #endregion
        }
    }
}