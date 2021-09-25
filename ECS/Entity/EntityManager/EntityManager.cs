using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GDG.ModuleManager;
using GDG.Utils;
using UnityEngine.Events;

namespace GDG.ECS
{
    public class EntityManager : IEntityCreateable, IEntityDestoryable, IEntityRecyclable
    {
        private readonly Dictionary<uint, EntityPool> m_TypeId2EntityPoolMapping;
        private readonly Dictionary<ulong, Entity> m_Index2EnityMapping;
        private readonly Dictionary<uint, ComponentTypes> m_TypeId2ComponentTypeMapping;
        private readonly Dictionary<uint, List<ulong>> m_TypeId2IndexMapping;
        public readonly List<Entity> m_ActivedEntityList;
        private ulong entityMaxIndex;
        private uint maxTypeId;
        internal EntityManager()
        {
            m_TypeId2EntityPoolMapping = new Dictionary<uint, EntityPool>();
            m_Index2EnityMapping = new Dictionary<ulong, Entity>();
            m_TypeId2ComponentTypeMapping = new Dictionary<uint, ComponentTypes>();
            m_TypeId2IndexMapping = new Dictionary<uint, List<ulong>>();
            m_ActivedEntityList = new List<Entity>();//活跃实体列表，供世界使用
        }
        internal void EntityMaxIndexIncrease()
        {
            entityMaxIndex++;
        }
        internal ulong GetEntityMaxIndex()
        {
            return entityMaxIndex;
        }
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
        public void RecycleEntity(Entity entity)
        {
            AddTypeId2EntityPoolMapping(entity.TypeId, out EntityPool entityPool, (pool) => { pool.PushEntity(entity); }, (pool) => { pool.PushEntity(entity); });
            m_ActivedEntityList.Remove(entity);
            BaseWorld.Instance.UpdateEntitiesOfSystems(m_ActivedEntityList);
        }
        public void DestroyEntity(Entity entity)
        {
            EntityRefCountDecrementOne(entity);
            RemoveTypeId2IndexMapping(entity.TypeId, entity.Index, () => { });
            RemoveIndex2EnityMapping(entity.Index, () => { });
            m_ActivedEntityList.Remove(entity);
            BaseWorld.Instance.UpdateEntitiesOfSystems(m_ActivedEntityList);
            entity.OnDestroy();
        }

        #region CreateEntity
        /// <summary>
        /// 创建实体
        /// </summary>
        public Entity CreateEntity()
        {
            return CreateEntity(0);
        }
        public Entity CreateEntity(ComponentTypes componentTypes)
        {
            return CreateEntity(componentTypes.TypeId);
        }
        public Entity CreateEntity(uint typeId)
        {
            AddTypeId2EntityPoolMapping(typeId, out EntityPool entityPool, null, null);

            var entity = entityPool.PopEntity();

            if (entity.Version == 1 && typeId != 0)
            {
                if (m_TypeId2ComponentTypeMapping.TryGetValue(typeId, out ComponentTypes types))
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
        public Entity CreateEntity<T>() where T : class, IComponent
        {
            return CreateEntity<T>(null);
        }
        public Entity CreateEntity<T>(Action<T> initCallback) where T : class, IComponent
        {
            ComponentTypes componentTypes = new ComponentTypes(typeof(T));
            var entity = CreateEntity(componentTypes);
            if (!entity.TryGetComponent<T>(out T gameObjectComponent))
            {
                Log.Error($"Create Entity failed ! Can't find component after create it , Component Type : {typeof(T)}");
            }
            initCallback?.Invoke(gameObjectComponent);
            return entity;
        }

        #endregion
        #region CreateGameEntity
        public Entity CreateGameEntity(GameObject gameObject)
        {
            return CreateGameEntity(0, gameObject);
        }
        public Entity CreateGameEntity(uint typeId, GameObject gameObject)
        {
            var entity = CreateEntity<GameObjectComponent>();
            World.EntityManager.SetComponentData<GameObjectComponent>(entity, new GameObjectComponent()
            {
                gameObject = gameObject
            });
            return entity;
        }
        public Entity CreateGameEntity(ComponentTypes componentTypes, GameObject gameObject)
        {
            return CreateGameEntity(componentTypes.TypeId, gameObject);
        }
        public Entity CreateGameEntity<T>(GameObject gameObject) where T : IComponent
        {
            ComponentTypes componentTypes = new ComponentTypes(typeof(T));
            return CreateGameEntity(componentTypes, gameObject);
        }

        #endregion

        #region SelectEntity
        public bool TrySelectEntityWithIndex<T>(ulong index, out T result) where T : Entity
        {
            if (m_Index2EnityMapping.TryGetValue(index, out Entity absEntity))
            {
                result = absEntity as T;
                return true;
            }
            result = null;
            return false;
        }
        public bool TrySelectActivedEntityWithIndex<T>(ulong index, out T result) where T : Entity
        {
            if (m_Index2EnityMapping.TryGetValue(index, out Entity absEntity))
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
        public bool TrySelectEntitiesWithTypeId<T>(uint typeId, out List<T> resultList) where T : Entity
        {
            var indexList = GetEntityIndexesWithTypeId(typeId);
            List<T> entityList = new List<T>();
            foreach (var index in indexList)
            {
                Entity entity;
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
        public bool TrySelectActivedEntitiesWithTypeId<T>(uint typeId, out List<T> resultList) where T : Entity
        {
            var indexList = GetEntityIndexesWithTypeId(typeId);
            List<T> entityList = new List<T>();
            foreach (var index in indexList)
            {
                Entity entity;
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
        public Entity SelectEntityWithIndex(ulong index)
        {
            if (m_Index2EnityMapping.TryGetValue(index, out ECS.Entity absEntity))
            {
                return absEntity;
            }
            return null;
        }
        public Entity SelectActivedEntityWithIndex(ulong index)
        {
            if (m_Index2EnityMapping.TryGetValue(index, out Entity absEntity))
            {
                if (!absEntity.IsActived)
                {
                    return null;
                }
                return absEntity;
            }
            return null;
        }
        public List<Entity> SelectEntitiesWithTypeId(uint typeId)
        {
            var indexList = GetEntityIndexesWithTypeId(typeId);
            List<Entity> entityList = new List<Entity>();
            foreach (var index in indexList)
            {
                ECS.Entity entity;
                if (m_Index2EnityMapping.TryGetValue(index, out entity))
                {
                    if (entity is Entity tempEntity)
                    {
                        entityList.Add(tempEntity);
                    }
                }
            }
            return entityList.Count > 0 ? null : entityList;
        }
        public List<Entity> SelectActivedEntitiesWithTypeId(uint typeId)
        {
            var indexList = GetEntityIndexesWithTypeId(typeId);
            if (indexList == null)
                return null;
            List<Entity> entityList = new List<Entity>();
            foreach (var index in indexList)
            {
                Entity entity;
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
        #endregion
        #region Component
        public List<IComponent> AddComponent(Entity entity, ComponentTypes componentTypes)
        {
            if (!entity.IsActived)
            {
                LogManager.Instance.LogError("Illegal Operation! Entity is not actived");
                return null;
            }

            AddComponentTypes(entity, componentTypes);
            return entity.Components;
        }
        public T AddComponent<T>(Entity entity) where T : class,IComponent, new()
        {
            if (!entity.IsActived)
            {
                LogManager.Instance.LogError("Illegal Operation! Entity is not actived");
                return default(T);
            }
            return AddComponentTypes<T>(entity);
        }
        public bool RemoveComponet(Entity entity, ComponentTypes componentTypes)
        {
            if (!entity.IsActived)
            {
                LogManager.Instance.LogError("Illegal Operation! Entity is IsActived");
                return false;
            }
            return RemoveComponentTypes(entity, componentTypes);
        }
        public bool RemoveComponet<T>(Entity entity) where T : class,IComponent
        {
            if (!entity.IsActived)
            {
                LogManager.Instance.LogError("Illegal Operation! Entity is IsActived");
                return false;
            }
            return RemoveComponentTypes<T>(entity);
        }
        public void SetComponentData<T>(Entity entity, T component) where T :class,IComponent
        {
            for (var i = 0; i < entity.Components.Count; i++)
            {
                if (entity.Components[i] is T)
                {
                    entity.Components[i] = component;
                    return;
                }
            }
            LogManager.Instance.LogError($"Entity doesn't exist Component!Index:{entity.Index}, Component:{typeof(T)}");
        }
        public void SetComponentData<T>(Entity entity,Action<T> action)where T:class,IComponent
        {
            if(entity.TryGetComponent<T>(out T component))
            {
                action?.Invoke(component);
                return;
            }
            LogManager.Instance.LogError($"Entity doesn't exist Component!Index:{entity.Index}, Component:{typeof(T)}");
        }
        #region Details
        //实体组件类型在字典中的引用对实体引用计数-1，只用于对实体组件的修改件之前以及实体的销毁时使用
        private bool EntityRefCountDecrementOne(Entity entity)
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
        private ComponentTypes AddComponentTypes(Entity entity, ComponentTypes componentTypes)
        {
            //添加之前
            EntityRefCountDecrementOne(entity);
            //添加之后
            var typeId = componentTypes.TypeId;
            List<Type> types = new List<Type>();
            //添加组件
            foreach (var item in componentTypes)
            {
                entity.AddComponentToList(Activator.CreateInstance(item) as IComponent);
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
        private bool RemoveComponentTypes(Entity entity, ComponentTypes componentTypes)
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
        private T AddComponentTypes<T>(Entity entity) where T : IComponent
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
        private bool RemoveComponentTypes<T>(Entity entity) where T : IComponent
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
        #endregion
        #region Mapping
        internal bool AddTypeId2EntityPoolMapping(uint typeId, out EntityPool entityPool, Action<EntityPool> sucessCallback = null, Action<EntityPool> failedCallback = null)
        {
            if (!m_TypeId2EntityPoolMapping.TryGetValue(typeId, out EntityPool outEntityPool))
            {
                entityPool = new EntityPool();
                entityPool.typeId = typeId;
                m_TypeId2EntityPoolMapping.Add(typeId, entityPool);
                sucessCallback?.Invoke(entityPool);
                return true;
            }
            entityPool = outEntityPool;
            failedCallback?.Invoke(entityPool);
            return false;
        }
        internal bool RemoveTypeId2EntityPoolMapping(uint typeId, Action sucessCallback = null)
        {
            if (m_TypeId2EntityPoolMapping.TryGetValue(typeId, out EntityPool outEntityPool))
            {
                m_TypeId2EntityPoolMapping.Remove(typeId);
                sucessCallback?.Invoke();
                return true;
            }
            return false;
        }
        internal bool AddIndex2EnityMapping(ulong typeId, Entity entity, Action<Entity> sucessCallback = null)
        {
            if (!m_Index2EnityMapping.TryGetValue(typeId, out Entity outEntity))
            {
                m_Index2EnityMapping.Add(typeId, entity);
                sucessCallback?.Invoke(entity);
                return true;
            }
            return false;
        }
        internal bool RemoveIndex2EnityMapping(ulong typeId, Action sucessCallback = null)
        {
            if (m_Index2EnityMapping.TryGetValue(typeId, out Entity outentity))
            {
                m_Index2EnityMapping.Remove(typeId);
                sucessCallback?.Invoke();
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
        internal bool RemoveTypeId2IndexMapping(uint typeId, ulong index, Action sucessCallback = null)
        {
            if (m_TypeId2IndexMapping.TryGetValue(typeId, out List<ulong> indexList))
            {
                if (m_TypeId2IndexMapping[typeId].Contains(index))
                {
                    m_TypeId2IndexMapping[typeId].Remove(index);
                    sucessCallback?.Invoke();
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
        internal bool RemoveTypeId2ComponentTypeMapping(uint typeId, Action<ComponentTypes> sucessCallback = null)
        {

            if (m_TypeId2ComponentTypeMapping.TryGetValue(typeId, out ComponentTypes outComponentTypes))
            {
                m_TypeId2ComponentTypeMapping.Remove(typeId);
                sucessCallback?.Invoke(outComponentTypes);
                return true;
            }
            return false;
        }
        internal bool AddTypeId2IndexMapping(uint typeId, ulong index, Action sucessCallback = null)
        {
            if (m_TypeId2IndexMapping.TryGetValue(typeId, out List<ulong> indexList))
            {
                if (!m_TypeId2IndexMapping[typeId].Contains(index))
                {
                    m_TypeId2IndexMapping[typeId].Add(index);
                    sucessCallback?.Invoke();
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
                sucessCallback?.Invoke();
            }
            return false;
        }
        internal bool AddTypeId2ComponentTypeMapping(uint typeId, ComponentTypes componentTypes, Action<ComponentTypes> sucessCallback = null)
        {
            if (!m_TypeId2ComponentTypeMapping.TryGetValue(typeId, out ComponentTypes outComponentTypes))
            {
                m_TypeId2ComponentTypeMapping.Add(typeId, componentTypes);
                sucessCallback?.Invoke(componentTypes);
                return true;
            }
            return false;
        }
        #endregion
    }
}