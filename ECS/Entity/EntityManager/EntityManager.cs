using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GDG.ModuleManager;
using GDG.Utils;

namespace GDG.ECS
{
    public class EntityManager : IEntityCreateable, IEntityDestoryable, IEntityRecyclable
    {
        private Dictionary<uint, EntityPool> m_TypeId2EntityPoolMapping;
        private Dictionary<ulong, AbsEntity> m_Index2EnityMapping;
        private Dictionary<uint, ComponentTypes> m_TypeId2ComponentTypeMapping;
        private Dictionary<uint, List<ulong>> m_TypeId2IndexMapping;
        public List<AbsEntity> m_ActivedEntityList;
        private ulong entityMaxIndex;
        private uint maxTypeId;
        internal void EntityMaxIndexIncrease()
        {
            entityMaxIndex++;
        }
        internal ulong GetEntityMaxIndex()
        {
            return entityMaxIndex;
        }
        internal EntityManager()
        {
            m_TypeId2EntityPoolMapping = new Dictionary<uint, EntityPool>();
            m_Index2EnityMapping = new Dictionary<ulong, AbsEntity>();
            m_TypeId2ComponentTypeMapping = new Dictionary<uint, ComponentTypes>();
            m_TypeId2IndexMapping = new Dictionary<uint, List<ulong>>();
            m_ActivedEntityList = new List<AbsEntity>();//活跃实体列表，供世界使用
        }
        public AbsEntity CreateEntity(uint typeId)
        {
            AddTypeId2EntityPoolMapping(typeId, out EntityPool entityPool, (entityPool) => { }, (entityPool) => { });
            var entity = entityPool.PopEntity((obj) => { }, (obj) => { });

            if(entity.Version==1 && typeId!=0)
            {
                if(m_TypeId2ComponentTypeMapping.TryGetValue(typeId,out ComponentTypes types))
                {
                    AddComponent(entity, types);
                }
            }

            if (!m_Index2EnityMapping.ContainsKey(entity.Index))
                m_Index2EnityMapping.Add(entity.Index, entity);
            if (!m_ActivedEntityList.Contains(entity))
                m_ActivedEntityList.Add(entity);

            BaseWorld.Instance.UpdateEntitiesOfSystems(m_ActivedEntityList);

            return entity;
        }
        public T CreateEntity<T>(uint typeId = 0) where T : AbsEntity
        {
            if (typeof(T) == typeof(GameEntity))
            {
                return CreateGameEntity(typeId) as T;
            }
            return CreateEntity(typeId) as T;
        }
        public GameEntity CreateGameEntity(uint typeId = 0, bool isCreateGameObject = true)
        {
            AddTypeId2EntityPoolMapping(typeId, out EntityPool entityPool, (entityPool) => { }, (entityPool) => {});
            var entity = entityPool.PopEntity<GameEntity>((gameEntity) => { gameEntity.isCreateGameObject = isCreateGameObject; }, (obj) => { });
            if(entity.Version==1 && typeId!=0)
            {
                if(m_TypeId2ComponentTypeMapping.TryGetValue(typeId,out ComponentTypes types))
                {
                    AddComponent(entity, types);
                }
            }
            if (!m_Index2EnityMapping.ContainsKey(entity.Index))
                m_Index2EnityMapping.Add(entity.Index, entity);
            if (!m_ActivedEntityList.Contains(entity))
                m_ActivedEntityList.Add(entity);

            BaseWorld.Instance.UpdateEntitiesOfSystems(m_ActivedEntityList);
            return entity;
        }
        public void RecycleEntity(AbsEntity entity)
        {
            AddTypeId2EntityPoolMapping(entity.TypeId, out EntityPool entityPool, (pool) => { pool.PushEntity(entity); }, (pool) => { pool.PushEntity(entity); });
            m_ActivedEntityList.Remove(entity);
            BaseWorld.Instance.UpdateEntitiesOfSystems(m_ActivedEntityList);
        }
        public void DestroyEntity(AbsEntity entity)
        {
            EntityRefCountDecrementOne(entity);
            RemoveTypeId2IndexMapping(entity.TypeId, entity.Index, () => { });
            RemoveIndex2EnityMapping(entity.Index, () => { });
            m_ActivedEntityList.Remove(entity);

            BaseWorld.Instance.UpdateEntitiesOfSystems(m_ActivedEntityList);

            entity.OnDestroy();
            UnityEngine.Object.Destroy(entity);
        }
        public bool TrySelectEntityWithIndex<T>(ulong index, out T result) where T : AbsEntity
        {
            if (m_Index2EnityMapping.TryGetValue(index, out AbsEntity absEntity))
            {
                result = absEntity as T;
                return true;
            }
            result = null;
            return false;
        }
        public bool TrySelectActivedEntityWithIndex<T>(ulong index, out T result) where T : AbsEntity
        {
            if (m_Index2EnityMapping.TryGetValue(index, out AbsEntity absEntity))
            {
                if (!absEntity.IsActived)
                {
                    result = null;
                    return false;
                }
                result = absEntity as T;
                return true;
            }
            result = null;
            return false;
        }
        public bool TrySelectEntitiesWithTypeId<T>(uint typeId, out List<T> resultList) where T : AbsEntity
        {
            var indexList = GetEntityIndexesWithTypeId(typeId);
            List<T> entityList = new List<T>();
            foreach (var index in indexList)
            {
                AbsEntity entity;
                if (m_Index2EnityMapping.TryGetValue(index, out entity))
                {
                    if (entity is T tempEntity)
                    {
                        entityList.Add(tempEntity);
                    }
                }
            }
            resultList = entityList.Count > 0 ? null : entityList;
            return true;
        }
        public bool TrySelectActivedEntitiesWithTypeId<T>(uint typeId, out List<T> resultList) where T : AbsEntity
        {
            var indexList = GetEntityIndexesWithTypeId(typeId);
            List<T> entityList = new List<T>();
            foreach (var index in indexList)
            {
                AbsEntity entity;
                if (m_Index2EnityMapping.TryGetValue(index, out entity))
                {
                    if (entity is T tempEntity && entity.IsActived)
                    {
                        entityList.Add(tempEntity);
                    }
                }
            }
            resultList = entityList.Count > 0 ? null : entityList;
            return true;
        }



        public AbsEntity SelectEntityWithIndex(ulong index)
        {
            if (m_Index2EnityMapping.TryGetValue(index, out ECS.AbsEntity absEntity))
            {
                return absEntity;
            }
            return null;
        }
        public AbsEntity SelectActivedEntityWithIndex(ulong index)
        {
            if (m_Index2EnityMapping.TryGetValue(index, out AbsEntity absEntity))
            {
                if (!absEntity.IsActived)
                {
                    return null;
                }
                return absEntity;
            }
            return null;
        }
        public List<AbsEntity> SelectEntitiesWithTypeId(uint typeId)
        {
            var indexList = GetEntityIndexesWithTypeId(typeId);
            List<AbsEntity> entityList = new List<AbsEntity>();
            foreach (var index in indexList)
            {
                ECS.AbsEntity entity;
                if (m_Index2EnityMapping.TryGetValue(index, out entity))
                {
                    if (entity is AbsEntity tempEntity)
                    {
                        entityList.Add(tempEntity);
                    }
                }
            }
            return entityList.Count > 0 ? null : entityList;
        }


        public List<AbsEntity> SelectActivedEntitiesWithTypeId(uint typeId)
        {
            var indexList = GetEntityIndexesWithTypeId(typeId);
            if (indexList == null)
                return null;
            List<AbsEntity> entityList = new List<AbsEntity>();
            foreach (var index in indexList)
            {
                AbsEntity entity;
                if (m_Index2EnityMapping.TryGetValue(index, out entity))
                {
                    if (entity.IsActived)
                    {
                        entityList.Add(entity);
                    }
                }
            }
            return entityList.Count > 0 ? null : entityList;
        }
        public List<IComponentData> AddComponent(AbsEntity entity, ComponentTypes componentTypes)
        {
            if (!entity.IsActived)
            {
                LogManager.Instance.LogError("Illegal Operation! Entity is IsActived");
                return null;
            }

            AddComponentTypes(entity, componentTypes);
            return entity.Components;
        }
        public T AddComponent<T>(AbsEntity entity) where T : IComponentData, new()
        {
            // ComponentTypes type = new ComponentTypes(typeof(T));
            // GDGLogger.LogWarning(type.TypeId);
            if (!entity.IsActived)
            {
                LogManager.Instance.LogError("Illegal Operation! Entity is IsActived");
                return default(T);
            }
            return AddComponentTypes<T>(entity);
        }
        public bool RemoveComponet(AbsEntity entity, ComponentTypes componentTypes)
        {
            if (!entity.IsActived)
            {
                LogManager.Instance.LogError("Illegal Operation! Entity is IsActived");
                return false;
            }
            return RemoveComponentTypes(entity, componentTypes);
        }
        public bool RemoveComponet<T>(AbsEntity entity) where T : IComponentData
        {
            if (!entity.IsActived)
            {
                LogManager.Instance.LogError("Illegal Operation! Entity is IsActived");
                return false;
            }
            return RemoveComponentTypes<T>(entity);
        }
        public void SetComponentData<T>(Entity entity, T component) where T : IComponentData
        {
            foreach (var item in entity.Components)
            {
                if (item is T tempComponent)
                {
                    tempComponent = component;
                    return;
                }
            }
            LogManager.Instance.LogError($"Entity doesn't exist Component!Index:{entity.Index}, Component:{typeof(T)}");
        }
        #region detail
        internal uint GetTypeId(ComponentTypes componentTypes)
        {
            foreach (var item in m_TypeId2ComponentTypeMapping.Values)
            {
                if (item.Count != componentTypes.Count)
                    continue;

                if (item.Equals(componentTypes))
                {
                    return item.TypeId;
                }
            }
            return 0;
        }
        internal uint RequestTypeId(ComponentTypes componentTypes)
        {
            if (componentTypes.Count == 0)
                return 0;
            componentTypes.SetTypeId(++maxTypeId);
            m_TypeId2ComponentTypeMapping.Add(componentTypes.TypeId, componentTypes);
            return componentTypes.TypeId;
        }
        internal bool AddTypeId2EntityPoolMapping(uint typeId, out EntityPool entityPool, Action<EntityPool> sucessCallback, Action<EntityPool> failedCallback)
        {
            if (!m_TypeId2EntityPoolMapping.TryGetValue(typeId, out EntityPool outEntityPool))
            {
                entityPool = new EntityPool();
                entityPool.typeId = typeId;
                m_TypeId2EntityPoolMapping.Add(typeId, entityPool);
                sucessCallback(entityPool);
                return true;
            }
            entityPool = outEntityPool;
            failedCallback(entityPool);
            return false;
        }
        internal bool RemoveTypeId2EntityPoolMapping(uint typeId, Action sucessCallback)
        {
            if (m_TypeId2EntityPoolMapping.TryGetValue(typeId, out EntityPool outEntityPool))
            {
                m_TypeId2EntityPoolMapping.Remove(typeId);
                sucessCallback();
                return true;
            }
            return false;
        }
        internal bool AddIndex2EnityMapping(ulong typeId, AbsEntity entity, Action<AbsEntity> sucessCallback)
        {
            if (!m_Index2EnityMapping.TryGetValue(typeId, out AbsEntity outEntity))
            {
                m_Index2EnityMapping.Add(typeId, entity);
                sucessCallback(entity);
                return true;
            }
            return false;
        }
        internal bool RemoveIndex2EnityMapping(ulong typeId, Action sucessCallback)
        {
            if (m_Index2EnityMapping.TryGetValue(typeId, out AbsEntity outentity))
            {
                m_Index2EnityMapping.Remove(typeId);
                sucessCallback();
                return true;
            }
            return false;
        }
        internal ComponentTypes GetComponentTypesWithTypeId(uint typeId)
        {
            if (!m_TypeId2ComponentTypeMapping.TryGetValue(typeId, out ComponentTypes outComponentTypes))
            {
                LogManager.Instance.LogError($"Get ComponentTypes failed ! TypeId can't be found , TypeId:{typeId}");
            }
            return outComponentTypes;
        }
        internal List<ulong> GetEntityIndexesWithTypeId(uint typeId)
        {
            if (!m_TypeId2IndexMapping.TryGetValue(typeId, out List<ulong> indexList))
            {
                LogManager.Instance.LogError($"Get EntityIndexes failed ! TypeId can't be found , TypeId:{typeId}");
            }
            return indexList;
        }
        internal bool RemoveTypeId2IndexMapping(uint typeId, ulong index, Action sucessCallback)
        {
            if (m_TypeId2IndexMapping.TryGetValue(typeId, out List<ulong> indexList))
            {
                if (m_TypeId2IndexMapping[typeId].Contains(index))
                {
                    m_TypeId2IndexMapping[typeId].Remove(index);
                    sucessCallback();
                }
                else
                {
                    LogManager.Instance.LogError($"Remove index failed ! Index can't be found , Index:{index}");
                    return false;
                }
                return true;
            }
            return false;
        }
        internal bool RemoveTypeId2ComponentTypeMapping(uint typeId, Action<ComponentTypes> sucessCallback)
        {

            if (m_TypeId2ComponentTypeMapping.TryGetValue(typeId, out ComponentTypes outComponentTypes))
            {
                m_TypeId2ComponentTypeMapping.Remove(typeId);
                sucessCallback(outComponentTypes);
                return true;
            }
            return false;
        }
        internal bool AddTypeId2IndexMapping(uint typeId, ulong index, Action sucessCallback)
        {
            if (m_TypeId2IndexMapping.TryGetValue(typeId, out List<ulong> indexList))
            {
                if (!m_TypeId2IndexMapping[typeId].Contains(index))
                {
                    m_TypeId2IndexMapping[typeId].Add(index);
                    sucessCallback();
                }
                else
                {
                    LogManager.Instance.LogError($"Add index failed ! Index is repeated , Index:{index}");
                    return false;
                }
                return true;
            }
            else
            {
                m_TypeId2IndexMapping.Add(typeId, new List<ulong>() { index });
                sucessCallback();
            }
            return false;
        }
        internal bool AddTypeId2ComponentTypeMapping(uint typeId, ComponentTypes componentTypes, Action<ComponentTypes> sucessCallback)
        {
            if (!m_TypeId2ComponentTypeMapping.TryGetValue(typeId, out ComponentTypes outComponentTypes))
            {
                m_TypeId2ComponentTypeMapping.Add(typeId, componentTypes);
                sucessCallback(componentTypes);
                return true;
            }
            return false;
        }
        //实体组件类型在字典中的引用对实体引用计数-1，只用于对实体组件的修改件之前以及实体的销毁时使用
        private bool EntityRefCountDecrementOne(AbsEntity entity)
        {
            if (entity.TypeId == 0 || entity.Components.Count == 0)
                return false;
            ComponentTypes tempComponentTypes;
            if (m_TypeId2ComponentTypeMapping.TryGetValue(entity.TypeId, out tempComponentTypes))
            {
                tempComponentTypes.SetEntityRefCount(tempComponentTypes.EntityRefCount - 1 < 0 ? 0 : tempComponentTypes.EntityRefCount - 1);//引用自减1
                RemoveTypeId2IndexMapping(entity.TypeId, entity.Index, () => { });

                //引用计数为0，且不存在引用该组件的系统时移除该组件
                if (tempComponentTypes.EntityRefCount <= 0)
                {
                    tempComponentTypes.SetTypeId(0);//设置为未申请
                    if (m_TypeId2IndexMapping.Remove(entity.TypeId))//清空该组件对实体的映射
                        LogManager.Instance.LogError($"Remove entity failed ! Can't Find TypeId to Index Mapping:TypeId:{entity.TypeId},ComponentTypes:{entity.Index}");
                    m_TypeId2ComponentTypeMapping.Remove(entity.TypeId);//清空该typeId对组件的映射
                }
                return true;
            }
            else
                LogManager.Instance.LogError($"Remove component failed ! Can't Find ComponentTypes in World ! TypeId:{entity.TypeId}");
            return false;
        }
        private ComponentTypes AddComponentTypes(AbsEntity entity, ComponentTypes componentTypes)
        {
            //添加之前
            EntityRefCountDecrementOne(entity);
            //添加之后
            var typeId = componentTypes.TypeId;
            List<Type> types = new List<Type>();
            //添加组件
            foreach (var item in componentTypes)
            {
                entity.AddComponentToList(Activator.CreateInstance(item) as IComponentData);
            }
            //获取添加之后的组件类型
            foreach (var item in entity.Components)
            {
                types.Add(item.GetType());
            }
            //组装成ComponentTypes,并向世界申请
            ComponentTypes afterTypes = new ComponentTypes(types.ToArray());

            //如果未已向世界申请
            if (!afterTypes.IsRequested)
            {
                LogManager.Instance.LogError($"Add component failed ! ComponentTypes never be requested! TypeId:{afterTypes.TypeId}");
                return afterTypes;
            }
            entity.SetTypeId(afterTypes.TypeId);
            ComponentTypes outComponentTypes;
            //如果存在映射
            if (m_TypeId2ComponentTypeMapping.TryGetValue(entity.TypeId, out outComponentTypes))
            {
                outComponentTypes.SetEntityRefCount(outComponentTypes.EntityRefCount + 1);
                return outComponentTypes;
            }
            LogManager.Instance.LogError($"Add component failed ! Can't Find TypeId to ComponentType Mapping:TypeId:{entity.TypeId},ComponentTypes:{afterTypes.ToString()}");
            return null;
        }
        private bool RemoveComponentTypes(AbsEntity entity, ComponentTypes componentTypes)
        {
            var index = entity.Index;

            ComponentTypes outComponentTypes;
            //删除之前        
            EntityRefCountDecrementOne(entity);

            //遍历移除实体组件
            var componentList = entity.Components;
            List<Type> types = new List<Type>();
            for (int i = componentList.Count - 1; i >= 0; i--)
            {
                foreach (var item in componentTypes)
                {
                    if (componentList[i].GetType() == item)
                    {
                        if (!entity.RemoveComponentToList(componentList[i]))
                        {
                            LogManager.Instance.LogError($"Remove component failed ! Can't Find Component in Entity !Entity.Index:{index}");
                            return false;
                        }
                        continue;
                    }
                    types.Add(componentList[i].GetType());//没有被移除的组件
                }
            }
            ComponentTypes afterTypes = new ComponentTypes(types.ToArray());

            if (afterTypes.IsRequested)
            {
                LogManager.Instance.LogError($"Remove component failed ! ComponentTypes never be requested! TypeId:{afterTypes.TypeId}");
                return false;
            }
            entity.SetTypeId(afterTypes.TypeId);
            //如果存在映射
            if (m_TypeId2ComponentTypeMapping.TryGetValue(entity.TypeId, out outComponentTypes))
            {
                outComponentTypes.SetEntityRefCount(afterTypes.EntityRefCount + 1);//对之后的组件引用计数+1
                return true;
            }
            LogManager.Instance.LogError($"Remove component failed ! Can't Find TypeId to ComponentType Mapping:TypeId:{entity.TypeId},ComponentTypes:{afterTypes.ToString()}");
            return true;
        }
        private T AddComponentTypes<T>(AbsEntity entity) where T : IComponentData
        {
            //添加之前
            EntityRefCountDecrementOne(entity);
            //添加之后
            List<Type> types = new List<Type>();
            //添加组件
            var component = Activator.CreateInstance<T>();
            entity.AddComponentToList(Activator.CreateInstance<T>());
            //获取添加之后的组件类型
            foreach (var item in entity.Components)
            {
                types.Add(item.GetType());
            }
            //组装成ComponentTypes,并向世界申请
            ComponentTypes afterTypes = new ComponentTypes(types.ToArray());

            //如果未已向世界申请
            if (!afterTypes.IsRequested)
            {
                LogManager.Instance.LogError($"Add component failed ! ComponentTypes never be requested! TypeId:{afterTypes.TypeId}");
                return component;
            }
            entity.SetTypeId(afterTypes.TypeId);
            ComponentTypes outComponentTypes;
            //如果存在映射
            if (m_TypeId2ComponentTypeMapping.TryGetValue(entity.TypeId, out outComponentTypes))
            {
                outComponentTypes.SetEntityRefCount(outComponentTypes.EntityRefCount + 1);
                return component;
            }
            LogManager.Instance.LogError($"Add component failed ! Can't Find TypeId to ComponentType Mapping:TypeId:{entity.TypeId},ComponentTypes:{afterTypes.ToString()}");
            return default(T);
        }
        private bool RemoveComponentTypes<T>(AbsEntity entity) where T : IComponentData
        {
            var index = entity.Index;

            //删除之前        
            EntityRefCountDecrementOne(entity);
            //遍历移除实体组件
            var componentList = entity.Components;
            List<Type> types = new List<Type>();
            for (int i = componentList.Count - 1; i >= 0; i--)
            {
                if (componentList[i] is T item)
                {
                    if (!entity.RemoveComponentToList(item))
                    {
                        LogManager.Instance.LogError($"Remove component failed ! Can't Find Component in Entity !Entity.Index:{index}");
                        return false;
                    }
                }
                types.Add(componentList[i].GetType());//没有被移除的组件
            }
            ComponentTypes afterTypes = new ComponentTypes(types.ToArray());

            if (afterTypes.IsRequested)
            {
                LogManager.Instance.LogError($"Remove component failed ! ComponentTypes never be requested! TypeId:{afterTypes.TypeId}");
                return false;
            }
            entity.SetTypeId(afterTypes.TypeId);
            //如果存在映射
            ComponentTypes outComponentTypes;
            if (m_TypeId2ComponentTypeMapping.TryGetValue(entity.TypeId, out outComponentTypes))
            {
                outComponentTypes.SetEntityRefCount(afterTypes.EntityRefCount + 1);//对之后的组件引用计数+1
                return true;
            }
            LogManager.Instance.LogError($"Remove component failed ! Can't Find TypeId to ComponentType Mapping:TypeId:{entity.TypeId},ComponentTypes:{afterTypes.ToString()}");
            return true;
        }
        #endregion
    }
}