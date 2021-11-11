using System.Collections;
using System.Collections.Generic;
using GDG.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace GDG.ModuleManager
{
    public class AssetPool : LazySingleton<AssetPool>
    {
        //对象池字典
        private Dictionary<string, IPoolList> PoolDic = new Dictionary<string, IPoolList>();
        GameObject objPool; //用于存放IPoolList的父物体
        private bool enableSmartClean = true;
        private double GCtime = -1;
        private ulong currentTaskIndex;
        #region 取obj
        /// <summary>
        /// 从对象池中获得一个GameObject，若对象池中不存在该Object则返回一个新的实例
        /// </summary>
        /// <param name="bundlepath">Asset所在AB包的路径</param>
        /// <param name="assetpath">Asset在AB包下Asset路径</param>
        /// <param name="callback"></param>
        public void Pop<T>(string bundlepath, string assetpath, UnityAction<T> callback = null, string tag = "") where T : UnityEngine.Object
        {
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(assetpath) && (PoolDic[assetpath] as PoolListContainer<T>)?.poolList.poolstack.Count > 0)
                {
                    callback?.Invoke((PoolDic[assetpath] as PoolListContainer<T>).poolList.Get());
                }
                else
                {
                    AssetManager.Instance.LoadAssetAsync<T>(bundlepath, assetpath, (o) =>
                      {
                          if (o is GameObject)
                              o.name = assetpath;
                          callback?.Invoke(o);
                      });
                }
            }
            else
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(tag) && (PoolDic[tag] as PoolListContainer<T>)?.poolList.poolstack.Count > 0)
                {
                    callback?.Invoke((PoolDic[tag] as PoolListContainer<T>).poolList.Get());
                }
                else
                {
                    AssetManager.Instance.LoadAssetAsync<T>(bundlepath, assetpath, (o) =>
                      {
                          if (o is GameObject)
                              o.name = tag;
                          callback?.Invoke(o);
                      });
                }
            }
            SmartClean();

        }
        /// <summary>
        /// 从对象池中获得一个Object，若对象池中不存在该GameObject则返回一个新的实例
        /// </summary>
        /// <param name="resourcepath">为预制体在Resources下的路径</param>
        /// <param name="callback"></param>
        public void Pop<T>(string resourcepath, UnityAction<T> callback = null, string tag = "") where T : UnityEngine.Object
        {
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(resourcepath) && (PoolDic[resourcepath] as PoolListContainer<T>)?.poolList.poolstack.Count > 0)
                {
                    callback?.Invoke((PoolDic[resourcepath] as PoolListContainer<T>).poolList.Get());
                }
                else
                {
                    ResourcesManager.Instance.LoadResourceAsync<T>(resourcepath, (o) =>
                     {
                         if (o is GameObject)
                             o.name = resourcepath;
                         callback?.Invoke(o as T);
                     });
                }
            }
            else
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(tag) && (PoolDic[tag] as PoolListContainer<T>)?.poolList.poolstack.Count > 0)
                {
                    callback?.Invoke((PoolDic[tag] as PoolListContainer<T>).poolList.Get());
                }
                else
                {
                    ResourcesManager.Instance.LoadResourceAsync<T>(resourcepath, (o) =>
                     {
                         if (o is GameObject)
                             o.name = tag;
                         callback?.Invoke(o as T);
                     });
                }
            }
            SmartClean();
        }
        public void Pop(string resourcepath, UnityAction<UnityEngine.Object> callback = null, string tag = "")
        {
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(resourcepath) && (PoolDic[resourcepath] as PoolListContainer<UnityEngine.Object>)?.poolList.poolstack.Count > 0)
                {
                    callback?.Invoke((PoolDic[resourcepath] as PoolListContainer<Object>).poolList.Get());
                }
                else
                {
                    ResourcesManager.Instance.LoadResourceAsync<Object>(resourcepath, (o) =>
                     {
                         if (o is GameObject)
                             o.name = resourcepath;
                         callback?.Invoke(o as Object);
                     });
                }
            }
            else
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(tag) && (PoolDic[tag] as PoolListContainer<Object>)?.poolList.poolstack.Count > 0)
                {
                    callback?.Invoke((PoolDic[tag] as PoolListContainer<Object>).poolList.Get());
                }
                else
                {
                    ResourcesManager.Instance.LoadResourceAsync<Object>(resourcepath, (o) =>
                     {
                         if (o is GameObject)
                             o.name = tag;
                         callback?.Invoke(o as Object);
                     });
                }
            }
            SmartClean();
        }
        public void Pop(string bundlepath, string assetpath, UnityAction<Object> callback = null, string tag = "")
        {
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(assetpath) && (PoolDic[assetpath] as PoolListContainer<Object>)?.poolList.poolstack.Count > 0)
                {
                    callback?.Invoke((PoolDic[assetpath] as PoolListContainer<Object>).poolList.Get());
                }
                else
                {
                    AssetManager.Instance.LoadAssetAsync<Object>(bundlepath, assetpath, (o) =>
                      {
                          if (o is GameObject)
                              o.name = assetpath;
                          callback?.Invoke(o);
                      });
                }
            }
            else
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(tag) && (PoolDic[tag] as PoolListContainer<Object>)?.poolList.poolstack.Count > 0)
                {
                    callback?.Invoke((PoolDic[tag] as PoolListContainer<Object>).poolList.Get());
                }
                else
                {
                    AssetManager.Instance.LoadAssetAsync<Object>(bundlepath, assetpath, (o) =>
                      {
                          if (o is GameObject)
                              o.name = tag;
                          callback?.Invoke(o);
                      });
                }
            }
            SmartClean();

        }
        #endregion
        #region 放obj
        /// <summary>
        /// 从将一个Object对象放入对象池，默认的回调方式是：如果是GameObject则setActive为false，否则调用Destory
        /// </summary>
        /// <param name="path">在AB包下Asset路径或预制体在Resources下的路径（取决于pop时传入的assetpath或prefabpath）</param>
        /// <param name="obj">需要放入对象池的Object</param>

        public void Push<T>(string path, T obj, UnityAction callback = null, string tag = "") where T : UnityEngine.Object
        {
            if (callback == null)
                callback = () => { };
            if (tag != "")
                path = tag;

            if (obj is GameObject o)
            {
                //场景中若没用对象池则创建一个
                if (objPool == null)
                    objPool = new GameObject("GameObjectPool");

                //若 字典不存在PoolList 则 创建PoolList
                if (!PoolDic.ContainsKey(path))
                    PoolDic.Add(path, new PoolListContainer<GameObject>(objPool, o));

                (PoolDic[path] as PoolListContainer<GameObject>).poolList.Add(o, callback);
            }
            else
            {
                if (!PoolDic.ContainsKey(path))
                    PoolDic.Add(path, new PoolListContainer<T>());
                (PoolDic[path] as PoolListContainer<T>).poolList.Add(obj, callback);
            }
            SmartClean();
        }

        public void Push(string path, Object obj, UnityAction callback = null, string tag = "")
        {
            if (tag != "")
                path = tag;

            if (obj is GameObject o)
            {
                //场景中若没用对象池则创建一个
                if (objPool == null)
                    objPool = new GameObject("GameObjectPool");

                //若 字典不存在PoolList 则 创建PoolList
                if (!PoolDic.ContainsKey(path))
                    PoolDic.Add(path, new PoolListContainer<GameObject>(objPool, o));

                (PoolDic[path] as PoolListContainer<GameObject>).poolList.Add(o, callback);
            }
            else
            {
                if (!PoolDic.ContainsKey(path))
                    PoolDic.Add(path, new PoolListContainer<Object>());
                (PoolDic[path] as PoolListContainer<Object>).poolList.Add(obj, callback);
            }
            SmartClean();
        }

        #endregion
        #region 定时GC
        /// <summary>
        /// 设置定时进行对象池清空
        /// </summary>
        /// <param name="secondTime">每隔time秒进行一次清空一次对象池，当time为-1时关闭自动清空</param>
        /// <param name="enableSmartClean">是否启用智能GC，如果启用，则只会在对象池空闲时进行一次计时清空,否则会一直进行定时清空</param>
        public void SetAutoClean(double secondTime, bool enableSmartClean = true)
        {
            GCtime = secondTime;
            this.enableSmartClean = enableSmartClean;
            if (enableSmartClean)
            {
                if (currentTaskIndex != 0)
                    GDGTools.Timer.RemoveTask(currentTaskIndex);
            }
            else
            {
                currentTaskIndex = GDGTools.Timer.DelayTimeExcute(secondTime,0,ClearAll);
            }

        }
        void SmartClean()
        {
            if (!enableSmartClean || GCtime==-1)
                return;
            
            if(currentTaskIndex!=0)
                GDGTools.Timer.RemoveTask(currentTaskIndex);

            currentTaskIndex = GDGTools.Timer.DelayTimeExcute(GCtime,ClearAll);


        }
        #endregion
        public void ClearAll()
        {
            PoolDic.Clear();
            UnityEngine.Object.Destroy(objPool);
            objPool = null;
        }
    }
    interface IPoolList { }
    public class PoolListContainer<T> : IPoolList where T : UnityEngine.Object
    {
        public PoolListContainer()
        {
            poolList = new PoolList<T>();
        }
        public PoolListContainer(GameObject poolObj)
        {
            poolList = new PoolList<T>(poolObj);
        }
        public PoolListContainer(GameObject poolObj, GameObject obj)
        {
            poolList = new PoolList<T>(poolObj, obj);
        }
        public PoolList<T> poolList;
    }
    public class PoolList<T> where T : Object
    {
        public PoolList(GameObject objPool, GameObject obj)
        {
            parentobj = new GameObject(obj.name + "_Pool");
            parentobj.transform.parent = objPool.transform;
            poolstack = new Stack<T>();
            this.objPool = objPool;
        }
        public PoolList(GameObject objPool)
        {
            poolstack = new Stack<T>();
            this.objPool = objPool;
        }
        public PoolList()
        {
            poolstack = new Stack<T>();
        }
        public Stack<T> poolstack;
        public GameObject parentobj = null;
        public GameObject objPool = null;

        //从栈中pop
        public T Get()
        {
            if (typeof(T).IsAssignableFrom(typeof(GameObject)))
            {
                GameObject gameObject = poolstack.Pop() as GameObject;
                gameObject.SetActive(true);
                gameObject.transform.parent = null;
                return gameObject as T;
            }
            return poolstack.Pop();
        }

        //把对象push入栈
        public void Add(T obj, UnityAction callback)
        {
            if (obj is GameObject o)
            {
                if (o.GetComponent<ECS.GameObjectToEntity>() != null)
                            throw new CustomErrorException("Can't push GameObject with GameObjectToEntity component into the pool!");
                callback();
                o.SetActive(false);
                o.transform.parent = parentobj.transform;
                poolstack.Push(o as T);
                return;
            }
            callback();
            poolstack.Push(obj);
        }
    }

}
