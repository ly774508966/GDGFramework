using System.Threading.Tasks;
using System.Linq;
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
        internal readonly Dictionary<ulong, Entity> m_Index2EntityMapping;
        private readonly Dictionary<uint, ComponentTypes> m_TypeId2ComponentTypeMapping;
        private readonly Dictionary<uint, List<ulong>> m_TypeId2IndexMapping;
        private readonly Dictionary<ulong, List<IComponent>> m_Index2ComponentMapping;
        private readonly Dictionary<GameObject, Entity> m_GameObject2EntityMapping;

        internal readonly List<Entity> m_ActivedEntityList;
        private ulong entityMaxIndex;
        private uint maxTypeId;
        public List<Entity> GetAllEntity() => m_Index2EntityMapping.Values.ToList();
        internal EntityManager()
        {
            m_TypeId2EntityPoolMapping = new Dictionary<uint, EntityPool>();
            m_Index2EntityMapping = new Dictionary<ulong, Entity>();
            m_TypeId2ComponentTypeMapping = new Dictionary<uint, ComponentTypes>();
            m_TypeId2IndexMapping = new Dictionary<uint, List<ulong>>();
            m_ActivedEntityList = new List<Entity>();//活跃实体列表，供世界使用
            m_Index2ComponentMapping = new Dictionary<ulong, List<IComponent>>();
            m_GameObject2EntityMapping = new Dictionary<GameObject, Entity>();
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
        internal Entity FindEntityInGameObjectMapping(GameObject gameObject)
        {
            if (m_GameObject2EntityMapping.TryGetValue(gameObject, out Entity entity))
            {
                return entity;
            }
            return null;
        }
        /// <summary>
        /// 回收实体
        /// </summary>
        public void RecycleEntity(Entity entity)
        {
            if (entity.TryGetComponent<AssetComponent>(out AssetComponent asset))
            {
                if (asset.asset is GameObject gameObject)
                {
                    if (gameObject != null)
                    {
                        //gameObject.SetActive(false);
                        AssetPool.Instance.Push<GameObject>(asset.assetName, gameObject, null, false);
                        gameObject.transform.SetParent(World.monoWorld.EntityPool.transform);
                        m_GameObject2EntityMapping.Remove(gameObject);
                    }
                }
            }
            else if (entity.TryGetComponent<GameObjectComponent>(out GameObjectComponent game))
            {
                if (game.gameObject != null)
                {
                    //game.gameObject.SetActive(false);
                    AssetPool.Instance.Push<GameObject>(game.gameObject.name, game.gameObject, null, false);
                    game.gameObject.transform.SetParent(World.monoWorld.EntityPool.transform);
                    m_GameObject2EntityMapping.Remove(game.gameObject);
                }
            }
            AddTypeId2EntityPoolMapping(entity.TypeId, out EntityPool entityPool, (pool) => { pool.PushEntity(entity); }, (pool) => { pool.PushEntity(entity); });
            m_ActivedEntityList.Remove(entity);
            BaseWorld.Instance.UpdateEntitiesOfSystems(m_ActivedEntityList);
        }
        /// <summary>
        /// 销毁实体
        /// </summary>
        public void DestroyEntity(Entity entity)
        {
            if (entity.TryGetComponent<GameObjectComponent>(out GameObjectComponent game))
            {
                if (game.gameObject != null)
                {
                    m_GameObject2EntityMapping.Remove(game.gameObject);
                    GameObject.Destroy(game.gameObject);
                }
            }
            else if (entity.TryGetComponent<AssetComponent>(out AssetComponent asset))
            {
                if (asset.asset != null)
                {
                    if (asset.asset is GameObject gameObject)
                    {
                        m_GameObject2EntityMapping.Remove(gameObject);
                    }
                    UnityEngine.Object.Destroy(asset.asset);
                }
            }
            entity.SetActive(false);
            EntityRefCountDecrementOne(entity);
            RemoveTypeId2IndexMapping(entity.TypeId, entity.Index);
            RemoveIndex2EnityMapping(entity.Index);
            RemoveIndex2ComponentMapping(entity.Index);
            m_ActivedEntityList.Remove(entity);
            foreach (var system in World.Systems)
            {
                system.RemoveEntity(entity);
                system.SetEntities(m_ActivedEntityList);
            }
            entity.OnDestroy();
        }
        /// <summary>
        /// 销毁未激活的实体
        /// </summary>
        public void ClearInactiveEntities()
        {
            foreach (var pool in m_TypeId2EntityPoolMapping.Values)
            {
                foreach (var entity in pool.entityStack)
                {
                    DestroyEntity(entity);
                }
                pool.entityStack.Clear();

            }
        }
        #region CreateEntity
        /// <summary>
        /// 创建实体
        /// </summary>
        public Entity CreateEntity(string rename = null)
        {
            return CreateEntity(0, rename);
        }
        public Entity CreateEntity(ComponentTypes componentTypes, string rename = null)
        {
            if (componentTypes == null)
                return CreateEntity(0, rename);
            return CreateEntity(componentTypes.TypeId, rename);
        }
        public Entity CreateEntity(uint typeId, string rename = null)
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

            if (!m_ActivedEntityList.Contains(entity))
                m_ActivedEntityList.Add(entity);

            BaseWorld.Instance.UpdateEntitiesOfSystems(m_ActivedEntityList);
            if (rename != null)
                entity.Name = rename;
            return entity;
        }
        public Entity CreateEntity<T>(string rename = null) where T : class, IComponent
        {
            return CreateEntity<T>(null, rename);
        }
        public Entity CreateEntity<T>(Action<T> initCallback, string rename = null) where T : class, IComponent
        {
            ComponentTypes componentTypes = new ComponentTypes(typeof(T));
            var entity = CreateEntity(componentTypes, rename);
            if (!entity.TryGetComponent<T>(out T component))
            {
                Log.Error($"Create Entity failed ! Can't find component after create it , Component Type : {typeof(T)}");
            }
            initCallback?.Invoke(component);
            return entity;
        }

        #endregion
        #region CreateGameEntity
        public Entity CreateGameEntity(string rename = null)
        {
            return CreateGameEntity(null, null, rename);
        }
        public Entity CreateGameEntity(GameObject gameObject, string rename = null)
        {
            return CreateGameEntity(null, gameObject, rename);
        }
        public Entity CreateGameEntity(ComponentTypes componentTypes, GameObject gameObject, string rename = null)
        {
            if (componentTypes == null)
                componentTypes = new ComponentTypes(typeof(GameObjectComponent));
            componentTypes.Add(typeof(GameObjectComponent));
            var entity = CreateEntity(componentTypes, rename);

            if (entity.Version == 1)
            {
                if (gameObject == null)
                {
                    if (rename == null)
                        gameObject = new GameObject("Entity" + entity.Index);
                    else
                        gameObject = new GameObject(rename);
                }
                else if (rename != null)
                    gameObject.name = rename;
            }
            entity.SetComponentData<GameObjectComponent>((game) =>
            {
                game.gameObject = gameObject;
                m_GameObject2EntityMapping.Add(gameObject, entity);
            });
            entity.Name = gameObject.name;
            return entity;
        }
        public Entity CreateGameEntity<T>(string rename = null) where T : IComponent
        {
            ComponentTypes componentTypes = new ComponentTypes(typeof(T));
            return CreateGameEntity(componentTypes, null, rename);
        }
        public Entity CreateGameEntity<T>(GameObject gameObject, string rename = null) where T : IComponent
        {
            ComponentTypes componentTypes = new ComponentTypes(typeof(T));
            return CreateGameEntity(componentTypes, gameObject, rename);
        }
        #endregion
        #region CreateEntityFromResources
        /// <summary>
        /// 从Resources文件夹中创建实体
        /// </summary>
        /// <param name="componentTypes">组件类型</param>
        /// <param name="path">资源所在的Resources下路径</param>
        /// <param name="rename">重命名</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Entity CreateEntityFromResources<T>(ComponentTypes componentTypes, string path, string rename) where T : UnityEngine.Object
        {
            if (componentTypes == null)
                componentTypes = new ComponentTypes(typeof(AssetComponent));
            else
                componentTypes.Add(typeof(AssetComponent));

            if (typeof(T) == typeof(GameObject))
                componentTypes.Add(typeof(GameObjectComponent));

            var entity = CreateEntity(componentTypes);

            T res = AssetPool.Instance.Pop<T>(path);
            if (rename != null)
                res.name = rename;

            if (res is GameObject gameObject)
            {
                entity.SetComponentData<GameObjectComponent>((game) => { game.gameObject = gameObject; });

                gameObject.transform.SetParent(null);
                gameObject.RemoveFromDontDestoryOnLoad();
                gameObject.SetActive(true);
                m_GameObject2EntityMapping.Add(gameObject, entity);
            }
            entity.Name = res.name;
            entity.SetComponentData<AssetComponent>((Asset) => { Asset.asset = res; Asset.assetName = path; });

            return entity;
        }
        public Entity CreateEntityFromResources<T>(ComponentTypes componentTypes, string path) where T : UnityEngine.Object
        {
            return CreateEntityFromResources<T>(componentTypes, path, null);
        }
        public Entity CreateEntityFromResources<T>(string path, string rename) where T : UnityEngine.Object
        {
            return CreateEntityFromResources<T>(null, path, rename);
        }
        public Entity CreateEntityFromResources<T>(string path) where T : UnityEngine.Object
        {
            return CreateEntityFromResources<T>(null, path, null);
        }
        //异步
        public void CreateEntityFromResourcesAsync<T>(ComponentTypes componentTypes, string path, string rename, UnityAction<Entity, T> callback) where T : UnityEngine.Object
        {
            if (componentTypes == null)
                componentTypes = new ComponentTypes(typeof(AssetComponent));
            else
                componentTypes.Add(typeof(AssetComponent));

            if (typeof(T) == typeof(GameObject))
                componentTypes.Add(typeof(GameObjectComponent));

            var entity = CreateEntity(componentTypes);
            BaseWorld.Instance.AddOrRemoveEntityFromSystems(entity, false);

            AssetPool.Instance.PopAsync<T>(path, (res) =>
                {
                    if (rename != null)
                        res.name = rename;

                    if (res is GameObject gameObject)
                    {
                        entity.SetComponentData<GameObjectComponent>((game) => { game.gameObject = gameObject; });

                        gameObject.transform.SetParent(null);
                        gameObject.RemoveFromDontDestoryOnLoad();
                        gameObject.SetActive(true);
                        m_GameObject2EntityMapping.Add(gameObject, entity);
                    }
                    entity.Name = res.name;
                    entity.SetComponentData<AssetComponent>((Asset) => { Asset.asset = res; Asset.assetName = path; });
                    callback?.Invoke(entity, res);
                    BaseWorld.Instance.AddOrRemoveEntityFromSystems(entity, true);
                });

        }
        public void CreateEntityFromResourcesAsync<T>(ComponentTypes componentTypes, string path, UnityAction<Entity, T> callback) where T : UnityEngine.Object
        {
            CreateEntityFromResourcesAsync<T>(componentTypes, path, null, callback);
        }
        public void CreateEntityFromResourcesAsync<T>(string path, string rename, UnityAction<Entity, T> callback) where T : UnityEngine.Object
        {
            CreateEntityFromResourcesAsync<T>(null, path, rename, callback);
        }
        public void CreateEntityFromResourcesAsync<T>(string path, UnityAction<Entity, T> callback) where T : UnityEngine.Object
        {
            CreateEntityFromResourcesAsync<T>(null, path, null, callback);
        }
        #endregion
        #region CreateEntityFromAssetBundle
        /// <summary>
        /// 从AB包中创建实体
        /// </summary>
        /// <param name="componentTypes">组件类型</param>
        /// <param name="assetName">资源名</param>
        /// <param name="bundleName">包名</param>
        /// <param name="mainABName">主包名</param>
        /// <param name="path">主包所在文件夹路径</param>
        /// <param name="rename">重命名</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Entity CreateEntityFromAssetBundle<T>(ComponentTypes componentTypes, string bundleName, string assetName, string mainABName, string path, string rename) where T : UnityEngine.Object
        {
            if (componentTypes == null)
                componentTypes = new ComponentTypes(typeof(AssetComponent));
            else
                componentTypes.Add(typeof(AssetComponent));

            if (typeof(T) == typeof(GameObject))
                componentTypes.Add(typeof(GameObjectComponent));

            var entity = CreateEntity(componentTypes);
            if (entity.Version == 1)
            {
                var asset = AssetPool.Instance.Pop<T>(bundleName, assetName, path, mainABName);

                if (rename != null)
                    asset.name = rename;

                if (asset is GameObject gameObject)
                {
                    entity.SetComponentData<GameObjectComponent>((game) => { game.gameObject = gameObject; });
                    gameObject.transform.SetParent(null);
                    gameObject.RemoveFromDontDestoryOnLoad();
                    gameObject.SetActive(true);
                    m_GameObject2EntityMapping.Add(gameObject, entity);
                }
                entity.Name = asset.name;
                entity.SetComponentData<AssetComponent>((Asset) => { Asset.asset = asset; Asset.assetName = asset.name; });
            }
            return entity;
        }
        public Entity CreateEntityFromAssetBundle<T>(ComponentTypes componentTypes, string bundleName, string assetName, string mainABName, string rename) where T : UnityEngine.Object
        {
            return CreateEntityFromAssetBundle<T>(componentTypes, bundleName, assetName, mainABName, null, rename);
        }
        public Entity CreateEntityFromAssetBundle<T>(ComponentTypes componentTypes, string bundleName, string assetName, string rename) where T : UnityEngine.Object
        {
            return CreateEntityFromAssetBundle<T>(componentTypes, bundleName, assetName, null, null, rename);
        }
        public Entity CreateEntityFromAssetBundle<T>(ComponentTypes componentTypes, string bundleName, string assetName) where T : UnityEngine.Object
        {
            return CreateEntityFromAssetBundle<T>(componentTypes, bundleName, assetName, null, null, null);
        }
        public Entity CreateEntityFromAssetBundle<T>(string bundleName, string assetName, string mainABName, string path, string rename) where T : UnityEngine.Object
        {
            return CreateEntityFromAssetBundle<T>(null, bundleName, assetName, mainABName, path, rename);
        }
        public Entity CreateEntityFromAssetBundle<T>(string bundleName, string assetName, string mainABName, string rename) where T : UnityEngine.Object
        {
            return CreateEntityFromAssetBundle<T>(null, bundleName, assetName, mainABName, null, rename);
        }
        public Entity CreateEntityFromAssetBundle<T>(string bundleName, string assetName, string rename) where T : UnityEngine.Object
        {
            return CreateEntityFromAssetBundle<T>(null, bundleName, assetName, null, null, rename);
        }
        public Entity CreateEntityFromAssetBundle<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            return CreateEntityFromAssetBundle<T>(null, bundleName, assetName, null, null, null);
        }

        //异步
        public void CreateEntityFromAssetBundleAsync<T>(ComponentTypes componentTypes, string bundleName, string assetName, string mainABName, string path, string rename, UnityAction<Entity, T> callback) where T : UnityEngine.Object
        {
            if (componentTypes == null)
                componentTypes = new ComponentTypes(typeof(AssetComponent));
            else
                componentTypes.Add(typeof(AssetComponent));

            if (typeof(T) == typeof(GameObject))
                componentTypes.Add(typeof(GameObjectComponent));

            var entity = CreateEntity(componentTypes);
            BaseWorld.Instance.AddOrRemoveEntityFromSystems(entity, false);

            AssetPool.Instance.PopAsync<T>(bundleName, assetName, path, mainABName, (asset) =>
             {
                 if (rename != null)
                     asset.name = rename;

                 if (asset is GameObject gameObject)
                 {
                     entity.SetComponentData<GameObjectComponent>((game) => { game.gameObject = gameObject; });

                     gameObject.transform.SetParent(null);
                     gameObject.RemoveFromDontDestoryOnLoad();
                     gameObject.SetActive(true);
                     m_GameObject2EntityMapping.Add(gameObject, entity);
                 }
                 entity.Name = asset.name;
                 entity.SetComponentData<AssetComponent>((Asset) => { Asset.asset = asset; Asset.assetName = assetName; });
                 callback?.Invoke(entity, asset);
                 BaseWorld.Instance.AddOrRemoveEntityFromSystems(entity, true);
             });
        }
        public void CreateEntityFromAssetBundleAsync<T>(ComponentTypes componentTypes, string bundleName, string assetName, string mainABName, string rename, UnityAction<Entity, T> callback) where T : UnityEngine.Object
        {
            CreateEntityFromAssetBundleAsync<T>(componentTypes, bundleName, assetName, mainABName, null, rename, callback);
        }
        public void CreateEntityFromAssetBundleAsync<T>(ComponentTypes componentTypes, string bundleName, string assetName, string rename, UnityAction<Entity, T> callback) where T : UnityEngine.Object
        {
            CreateEntityFromAssetBundleAsync<T>(componentTypes, bundleName, assetName, null, null, rename, callback);
        }
        public void CreateEntityFromAssetBundleAsync<T>(ComponentTypes componentTypes, string bundleName, string assetName, UnityAction<Entity, T> callback) where T : UnityEngine.Object
        {
            CreateEntityFromAssetBundleAsync<T>(componentTypes, bundleName, assetName, null, null, null, callback);
        }
        public void CreateEntityFromAssetBundleAsync<T>(string bundleName, string assetName, string mainABName, string path, string rename, UnityAction<Entity, T> callback) where T : UnityEngine.Object
        {
            CreateEntityFromAssetBundleAsync<T>(null, bundleName, assetName, mainABName, path, rename, callback);
        }
        public void CreateEntityFromAssetBundleAsync<T>(string bundleName, string assetName, string mainABName, string rename, UnityAction<Entity, T> callback) where T : UnityEngine.Object
        {
            CreateEntityFromAssetBundleAsync<T>(null, bundleName, assetName, mainABName, null, rename, callback);
        }
        public void CreateEntityFromAssetBundleAsync<T>(string bundleName, string assetName, string rename, UnityAction<Entity, T> callback) where T : UnityEngine.Object
        {
            CreateEntityFromAssetBundleAsync<T>(null, bundleName, assetName, null, null, rename, callback);
        }
        public void CreateEntityFromAssetBundleAsync<T>(string bundleName, string assetName, UnityAction<Entity, T> callback) where T : UnityEngine.Object
        {
            CreateEntityFromAssetBundleAsync<T>(null, bundleName, assetName, null, null, null, callback);
        }


        #endregion
        #region SelectEntity
        public bool TrySelectEntityWithIndex<T>(ulong index, out T result) where T : Entity
        {
            if (m_Index2EntityMapping.TryGetValue(index, out Entity absEntity))
            {
                result = absEntity as T;
                return true;
            }
            result = null;
            return false;
        }
        public bool TrySelectActivedEntityWithIndex<T>(ulong index, out T result) where T : Entity
        {
            if (m_Index2EntityMapping.TryGetValue(index, out Entity absEntity))
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
                if (m_Index2EntityMapping.TryGetValue(index, out entity))
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
                if (m_Index2EntityMapping.TryGetValue(index, out entity))
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
            if (m_Index2EntityMapping.TryGetValue(index, out ECS.Entity absEntity))
            {
                return absEntity;
            }
            return null;
        }
        public Entity SelectActivedEntityWithIndex(ulong index)
        {
            if (m_Index2EntityMapping.TryGetValue(index, out Entity absEntity))
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
                if (m_Index2EntityMapping.TryGetValue(index, out entity))
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
                if (m_Index2EntityMapping.TryGetValue(index, out entity))
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
        internal void InitEntityComponent(ulong index)
        {
            if (!m_Index2ComponentMapping.ContainsKey(index))
                m_Index2ComponentMapping.Add(index, new List<IComponent>());
            else
                Log.Error($"InitEntityComponent failed ! The same mapping already exists here ! Index:{index}");
        }
        public List<IComponent> GetComponents(Entity entity)
        {
            if (!m_Index2ComponentMapping.TryGetValue(entity.Index, out List<IComponent> Components))
            {
                Log.Error($"AddComponentToList failed ! Cant't Find Index in m_Index2ComponentMapping, Index: {entity.Index}");
                return null;
            }
            return Components;
        }
        public List<IComponent> GetComponents(ulong Index)
        {
            if (!m_Index2ComponentMapping.TryGetValue(Index, out List<IComponent> Components))
            {
                Log.Error($"AddComponentToList failed ! Cant't Find Index in m_Index2ComponentMapping, Index: {Index}");
                return null;
            }
            return Components;
        }
        private void AddComponentToList(ulong index, IComponent component)
        {
            if (!m_Index2ComponentMapping.TryGetValue(index, out List<IComponent> Components))
            {
                Log.Error($"AddComponentToList failed ! Cant't Find Index in m_Index2ComponentMapping, Index: {index}");
                return;
            }
            if (!m_Index2EntityMapping.TryGetValue(index, out Entity entity))
            {
                Log.Error($"AddComponentToList failed ! Cant't Find Index in m_Index2EntityMapping, Index: {index}");
                return;
            }
            if (Components.Contains(component))
                return;
            if (component is IEntityNamedable setname)
                entity.setNameCallBack += setname.SetName;
            if (component is IEntityInitable init)
                entity.initCallback += init.InitEntity;
            if (component is IEntityEnable enable)
                entity.enableCallback += enable.EnableEntity;
            if (component is IEntityRecyclable recycle)
                entity.recycleCallback += recycle.RecycleEntity;
            if (component is IEntityDestoryable destroy)
                entity.destroyCallback += destroy.DestroyEntity;


            Components.Add(component);
        }
        private bool RemoveComponentToList(ulong index, IComponent component)
        {
            if (!m_Index2ComponentMapping.TryGetValue(index, out List<IComponent> Components))
            {
                Log.Error($"AddComponentToList failed ! Cant't Find Index in m_Index2ComponentMapping, Index: {index}");
                return false;
            }
            if (!m_Index2EntityMapping.TryGetValue(index, out Entity entity))
            {
                Log.Error($"AddComponentToList failed ! Cant't Find Index in m_Index2EntityMapping, Index: {index}");
                return false;
            }
            if (component is IEntityNamedable setname)
                entity.setNameCallBack -= setname.SetName;
            if (component is IEntityInitable init)
                entity.initCallback -= init.InitEntity;
            if (component is IEntityEnable enable)
                entity.enableCallback -= enable.EnableEntity;
            if (component is IEntityRecyclable recycle)
                entity.recycleCallback -= recycle.RecycleEntity;
            if (component is IEntityDestoryable destroy)
                entity.destroyCallback -= destroy.DestroyEntity;

            return Components.Remove(component);
        }
        public List<IComponent> AddComponent(Entity entity, ComponentTypes componentTypes)
        {
            if (!entity.IsActived)
            {
                Log.Error("Illegal Operation! Entity is not actived");
                return null;
            }
            AddComponentTypes(entity, componentTypes);
            if (!m_Index2ComponentMapping.TryGetValue(entity.Index, out List<IComponent> Components))
            {
                Log.Error($"AddComponent failed ! Cant't Find Index in m_Index2ComponentMapping, Index: {entity.Index}");
                return null;
            }
            return GetComponents(entity);
        }
        public T AddComponent<T>(Entity entity) where T : class, IComponent, new()
        {
            if (!entity.IsActived)
            {
                Log.Error("Illegal Operation! Entity is not actived");
                return default(T);
            }
            AddComponentTypes<T>(entity);
            return entity.GetComponent<T>();
        }
        public bool RemoveComponet(Entity entity, ComponentTypes componentTypes)
        {
            if (!entity.IsActived)
            {
                Log.Error("Illegal Operation! Entity is IsActived");
                return false;
            }
            return RemoveComponentTypes(entity, componentTypes);
        }
        public bool RemoveComponet<T>(Entity entity) where T : class, IComponent
        {
            if (!entity.IsActived)
            {
                Log.Error("Illegal Operation! Entity is IsActived");
                return false;
            }
            return RemoveComponentTypes<T>(entity);
        }
        public void SetComponentData<T>(Entity entity, T component) where T : class, IComponent
        {
            if (!m_Index2ComponentMapping.TryGetValue(entity.Index, out List<IComponent> Components))
            {
                Log.Error($"SetComponentData failed ! Cant't Find Index in m_Index2ComponentMapping, Index: {entity.Index}");
                return;
            }
            for (var i = 0; i < Components.Count; i++)
            {
                if (Components[i] is T)
                {
                    Components[i] = component;
                    return;
                }
            }
            Log.Error($"Entity doesn't exist Component!Index:{entity.Index}, Component:{typeof(T)}");
        }
        public void SetComponentData<T>(Entity entity, Action<T> action) where T : class, IComponent
        {
            if (entity.TryGetComponent<T>(out T component))
            {
                action?.Invoke(component);
                return;
            }
            Log.Error($"Entity doesn't exist Component!Index:{entity.Index}, Component:{typeof(T)}");
        }
        #region Details
        //实体组件类型在字典中的引用对实体引用计数-1，只用于对实体组件的修改件之前以及实体的销毁时使用
        private bool EntityRefCountDecrementOne(Entity entity)
        {
            if (!m_Index2ComponentMapping.TryGetValue(entity.Index, out List<IComponent> Components))
            {
                Log.Error($"EntityRefCountDecrementOne failed ! Cant't Find Index in m_Index2ComponentMapping, Index: {entity.Index}");
                return false;
            }
            if (entity.TypeId == 0 || Components.Count == 0)
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
                        Log.Error($"Remove entity failed ! Can't Find TypeId to Index Mapping:TypeId:{entity.TypeId},ComponentTypes:{entity.Index}");
                    m_TypeId2ComponentTypeMapping.Remove(entity.TypeId);//清空该typeId对组件的映射
                }
                return true;
            }
            else
                Log.Error($"Remove component failed ! Can't Find ComponentTypes in World ! TypeId:{entity.TypeId}");
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
                AddComponentToList(entity.Index, Activator.CreateInstance(item) as IComponent);
            }
            if (!m_Index2ComponentMapping.TryGetValue(entity.Index, out List<IComponent> Components))
            {
                Log.Error($"AddComponent failed ! Cant't Find Index in m_Index2ComponentMapping, Index: {entity.Index}");
                return null;
            }

            //获取添加之后的组件类型
            foreach (var item in Components)
            {
                types.Add(item.GetType());
            }
            //组装成ComponentTypes,并向世界申请
            ComponentTypes afterTypes = new ComponentTypes(types.ToArray());

            //如果未已向世界申请
            if (!afterTypes.IsRequested)
            {
                Log.Error($"Add component failed ! ComponentTypes never be requested! TypeId:{afterTypes.TypeId}");
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
            Log.Error($"Add component failed ! Can't Find TypeId to ComponentType Mapping:TypeId:{entity.TypeId},ComponentTypes:{afterTypes.ToString()}");
            return null;
        }
        private bool RemoveComponentTypes(Entity entity, ComponentTypes componentTypes)
        {
            var index = entity.Index;

            ComponentTypes outComponentTypes;
            //删除之前        
            EntityRefCountDecrementOne(entity);

            //遍历移除实体组件
            if (!m_Index2ComponentMapping.TryGetValue(entity.Index, out List<IComponent> Components))
            {
                Log.Error($"RemoveComponent failed ! Cant't Find Index in m_Index2ComponentMapping, Index: {entity.Index}");
                return false;
            }
            var componentList = Components;
            List<Type> types = new List<Type>();
            for (int i = componentList.Count - 1; i >= 0; i--)
            {
                foreach (var item in componentTypes)
                {
                    if (componentList[i].GetType() == item)
                    {
                        if (!RemoveComponentToList(entity.Index, componentList[i]))
                        {
                            Log.Error($"Remove component failed ! Can't Find Component in Entity !Entity.Index:{index}");
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
                Log.Error($"Remove component failed ! ComponentTypes never be requested! TypeId:{afterTypes.TypeId}");
                return false;
            }
            entity.SetTypeId(afterTypes.TypeId);
            //如果存在映射
            if (m_TypeId2ComponentTypeMapping.TryGetValue(entity.TypeId, out outComponentTypes))
            {
                outComponentTypes.SetEntityRefCount(afterTypes.EntityRefCount + 1);//对之后的组件引用计数+1
                return true;
            }
            Log.Error($"Remove component failed ! Can't Find TypeId to ComponentType Mapping:TypeId:{entity.TypeId},ComponentTypes:{afterTypes.ToString()}");
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
            AddComponentToList(entity.Index, Activator.CreateInstance<T>());

            if (!m_Index2ComponentMapping.TryGetValue(entity.Index, out List<IComponent> Components))
            {
                Log.Error($"AddComponent failed ! Cant't Find Index in m_Index2ComponentMapping, Index: {entity.Index}");
                return default(T);
            }

            //获取添加之后的组件类型
            foreach (var item in Components)
            {
                types.Add(item.GetType());
            }
            //组装成ComponentTypes,并向世界申请
            ComponentTypes afterTypes = new ComponentTypes(types.ToArray());

            //如果未已向世界申请
            if (!afterTypes.IsRequested)
            {
                Log.Error($"Add component failed ! ComponentTypes never be requested! TypeId:{afterTypes.TypeId}");
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
            Log.Error($"Add component failed ! Can't Find TypeId to ComponentType Mapping:TypeId:{entity.TypeId},ComponentTypes:{afterTypes.ToString()}");
            return default(T);
        }
        private bool RemoveComponentTypes<T>(Entity entity) where T : IComponent
        {
            var index = entity.Index;

            //删除之前        
            EntityRefCountDecrementOne(entity);

            //遍历移除实体组件
            if (!m_Index2ComponentMapping.TryGetValue(entity.Index, out List<IComponent> Components))
            {
                Log.Error($"RemoveComponent failed ! Cant't Find Index in m_Index2ComponentMapping, Index: {entity.Index}");
                return false;
            }
            var componentList = Components;
            List<Type> types = new List<Type>();
            for (int i = componentList.Count - 1; i >= 0; i--)
            {
                if (componentList[i] is T item)
                {
                    if (!RemoveComponentToList(entity.Index, item))
                    {
                        Log.Error($"Remove component failed ! Can't Find Component in Entity !Entity.Index:{index}");
                        return false;
                    }
                }
                types.Add(componentList[i].GetType());//没有被移除的组件
            }
            ComponentTypes afterTypes = new ComponentTypes(types.ToArray());

            if (afterTypes.IsRequested)
            {
                Log.Error($"Remove component failed ! ComponentTypes never be requested! TypeId:{afterTypes.TypeId}");
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
            Log.Error($"Remove component failed ! Can't Find TypeId to ComponentType Mapping:TypeId:{entity.TypeId},ComponentTypes:{afterTypes.ToString()}");
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
            if (!m_Index2EntityMapping.TryGetValue(typeId, out Entity outEntity))
            {
                m_Index2EntityMapping.Add(typeId, entity);
                sucessCallback?.Invoke(entity);
                return true;
            }
            return false;
        }
        internal bool RemoveIndex2EnityMapping(ulong typeId, Action sucessCallback = null)
        {
            if (m_Index2EntityMapping.TryGetValue(typeId, out Entity outentity))
            {
                m_Index2EntityMapping.Remove(typeId);
                sucessCallback?.Invoke();
                return true;
            }
            return false;
        }
        internal bool RemoveIndex2ComponentMapping(ulong typeId, Action sucessCallback = null)
        {
            if (m_Index2ComponentMapping.TryGetValue(typeId, out List<IComponent> outComponents))
            {
                m_Index2ComponentMapping.Remove(typeId);
                sucessCallback?.Invoke();
                return true;
            }
            return false;
        }
        internal ComponentTypes GetComponentTypesWithTypeId(uint typeId)
        {
            if (!m_TypeId2ComponentTypeMapping.TryGetValue(typeId, out ComponentTypes outComponentTypes))
            {
                Log.Error($"Get ComponentTypes failed ! TypeId can't be found , TypeId:{typeId}");
            }
            return outComponentTypes;
        }
        internal List<ulong> GetEntityIndexesWithTypeId(uint typeId)
        {
            if (!m_TypeId2IndexMapping.TryGetValue(typeId, out List<ulong> indexList))
            {
                Log.Error($"Get EntityIndexes failed ! TypeId can't be found , TypeId:{typeId}");
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
                    Log.Error($"Remove index failed ! Index can't be found , Index:{index}");
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
                    Log.Error($"Add index failed ! Index is repeated , Index:{index}");
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