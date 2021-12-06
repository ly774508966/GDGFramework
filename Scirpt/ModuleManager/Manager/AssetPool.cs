using System.Collections;
using System.Collections.Generic;
using GDG.Utils;
using UnityEngine;
using UnityEngine.Events;
using GDG.Base;

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
        #region Pop
        public T Pop<T>(string bundlepath, string assetname, string path, string mainABName, string tag = "") where T : UnityEngine.Object
        {
            SmartClean();
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(assetname) && (PoolDic[assetname] as PoolListContainer<T>)?.poolList.poolstack.Count > 0)
                {
                    return (PoolDic[assetname] as PoolListContainer<T>).poolList.Get();
                }
                else
                {
                    var o = AssetManager.Instance.LoadAsset<T>(bundlepath, assetname, path, mainABName);
                    o.name = assetname;
                    return o;
                }
            }
            else
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(tag) && (PoolDic[tag] as PoolListContainer<T>)?.poolList.poolstack.Count > 0)
                {
                    return (PoolDic[tag] as PoolListContainer<T>).poolList.Get();
                }
                else
                {
                    var o = AssetManager.Instance.LoadAsset<T>(bundlepath, assetname, path, mainABName);
                    o.name = assetname;
                    return o;
                }
            }
        }
        public T Pop<T>(string bundlepath, string assetname, string tag = "") where T : UnityEngine.Object
        {
            SmartClean();
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(assetname) && (PoolDic[assetname] as PoolListContainer<T>)?.poolList.poolstack.Count > 0)
                {
                    return (PoolDic[assetname] as PoolListContainer<T>).poolList.Get();
                }
                else
                {
                    var o = AssetManager.Instance.LoadAsset<T>(bundlepath, assetname);
                    o.name = assetname;
                    return o;
                }
            }
            else
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(tag) && (PoolDic[tag] as PoolListContainer<T>)?.poolList.poolstack.Count > 0)
                {
                    return (PoolDic[tag] as PoolListContainer<T>).poolList.Get();
                }
                else
                {
                    var o = AssetManager.Instance.LoadAsset<T>(bundlepath, assetname);
                    o.name = assetname;
                    return o;
                }
            }
        }
        public T Pop<T>(string resourcepath, string tag = "") where T : UnityEngine.Object
        {
            SmartClean();
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(resourcepath) && (PoolDic[resourcepath] as PoolListContainer<T>)?.poolList.poolstack.Count > 0)
                {
                    return (PoolDic[resourcepath] as PoolListContainer<T>).poolList.Get();
                }
                else
                {
                    var o = ResourcesManager.Instance.LoadResource<T>(resourcepath);
                    o.name = resourcepath;
                    return o;

                }
            }
            else
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(tag) && (PoolDic[tag] as PoolListContainer<T>)?.poolList.poolstack.Count > 0)
                {
                    return (PoolDic[tag] as PoolListContainer<T>).poolList.Get();
                }
                else
                {
                    var o = ResourcesManager.Instance.LoadResource<T>(resourcepath);
                    o.name = tag;
                    return o;

                }
            }
        }
        public Object Pop(string resourcepath, string tag = "")
        {
            SmartClean();
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(resourcepath) && (PoolDic[resourcepath] as PoolListContainer<UnityEngine.Object>)?.poolList.poolstack.Count > 0)
                {
                    return (PoolDic[resourcepath] as PoolListContainer<Object>).poolList.Get();
                }
                else
                {
                    var o = ResourcesManager.Instance.LoadResource<Object>(resourcepath);
                    o.name = resourcepath;
                    return o;
                }
            }
            else
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(tag) && (PoolDic[tag] as PoolListContainer<Object>)?.poolList.poolstack.Count > 0)
                {
                    return (PoolDic[tag] as PoolListContainer<Object>).poolList.Get();
                }
                else
                {
                    var o = ResourcesManager.Instance.LoadResource<Object>(resourcepath);
                    o.name = tag;
                    return o;

                }
            }
        }
        public Object Pop(string bundlepath, string assetname, string tag = "")
        {
            SmartClean();
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(assetname) && (PoolDic[assetname] as PoolListContainer<Object>)?.poolList.poolstack.Count > 0)
                {
                    return (PoolDic[assetname] as PoolListContainer<Object>).poolList.Get();
                }
                else
                {
                    var o = AssetManager.Instance.LoadAsset<Object>(bundlepath, assetname);
                    o.name = assetname;
                    return o;
                }
            }
            else
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(tag) && (PoolDic[tag] as PoolListContainer<Object>)?.poolList.poolstack.Count > 0)
                {
                    return (PoolDic[tag] as PoolListContainer<Object>).poolList.Get();
                }
                else
                {
                    var o = AssetManager.Instance.LoadAsset<Object>(bundlepath, assetname);
                    o.name = tag;
                    return o;
                }
            }
        }
        public Object Pop(string bundlepath, string assetname, string path, string mainABName, string tag = "")
        {
            SmartClean();
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(assetname) && (PoolDic[assetname] as PoolListContainer<Object>)?.poolList.poolstack.Count > 0)
                {
                    return (PoolDic[assetname] as PoolListContainer<Object>).poolList.Get();
                }
                else
                {
                    var o = AssetManager.Instance.LoadAsset<Object>(bundlepath, assetname, path, mainABName);
                    o.name = assetname;
                    return o;

                }
            }
            else
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(tag) && (PoolDic[tag] as PoolListContainer<Object>)?.poolList.poolstack.Count > 0)
                {
                    return (PoolDic[tag] as PoolListContainer<Object>).poolList.Get();
                }
                else
                {
                    var o = AssetManager.Instance.LoadAsset<Object>(bundlepath, assetname, path, mainABName);
                    o.name = tag;
                    return o;
                }
            }
        }

        #endregion
        #region PopAsync
        public void PopAsync<T>(string bundlepath, string assetname, string path, string mainABName, UnityAction<T> callback = null, string tag = "") where T : UnityEngine.Object
        {
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(assetname) && (PoolDic[assetname] as PoolListContainer<T>)?.poolList.poolstack.Count > 0)
                {
                    callback?.Invoke((PoolDic[assetname] as PoolListContainer<T>).poolList.Get());
                }
                else
                {
                    AssetManager.Instance.LoadAssetAsync<T>(bundlepath, assetname, path, mainABName, (o) =>
                        {
                            if (o is GameObject)
                                o.name = assetname;
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
                    AssetManager.Instance.LoadAssetAsync<T>(bundlepath, assetname, path, mainABName, (o) =>
                        {
                            if (o is GameObject)
                                o.name = tag;
                            callback?.Invoke(o);
                        });
                }
            }
            SmartClean();
        }
        public void PopAsync<T>(string bundlepath, string assetname, UnityAction<T> callback = null, string tag = "") where T : UnityEngine.Object
        {
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(assetname) && (PoolDic[assetname] as PoolListContainer<T>)?.poolList.poolstack.Count > 0)
                {
                    callback?.Invoke((PoolDic[assetname] as PoolListContainer<T>).poolList.Get());
                }
                else
                {
                    AssetManager.Instance.LoadAssetAsync<T>(bundlepath, assetname, (o) =>
                      {
                          if (o is GameObject)
                              o.name = assetname;
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
                    AssetManager.Instance.LoadAssetAsync<T>(bundlepath, assetname, (o) =>
                      {
                          if (o is GameObject)
                              o.name = tag;
                          callback?.Invoke(o);
                      });
                }
            }
            SmartClean();
        }
        public void PopAsync<T>(string resourcepath, UnityAction<T> callback = null, string tag = "") where T : UnityEngine.Object
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
        public void PopAsync(string resourcepath, UnityAction<Object> callback = null, string tag = "")
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
        public void PopAsync(string bundlepath, string assetname, UnityAction<Object> callback = null, string tag = "")
        {
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(assetname) && (PoolDic[assetname] as PoolListContainer<Object>)?.poolList.poolstack.Count > 0)
                {
                    callback?.Invoke((PoolDic[assetname] as PoolListContainer<Object>).poolList.Get());
                }
                else
                {
                    AssetManager.Instance.LoadAssetAsync<Object>(bundlepath, assetname, (o) =>
                      {
                          if (o is GameObject)
                              o.name = assetname;
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
                    AssetManager.Instance.LoadAssetAsync<Object>(bundlepath, assetname, (o) =>
                      {
                          if (o is GameObject)
                              o.name = tag;
                          callback?.Invoke(o);
                      });
                }
            }
            SmartClean();

        }
        public void PopAsync(string bundlepath, string assetname, string path, string mainABName, UnityAction<Object> callback = null, string tag = "")
        {
            if (tag == "")
            {
                //若 字典有PoolList 且 PoolList栈不为空则从栈中拿出一个对象
                if (PoolDic.ContainsKey(assetname) && (PoolDic[assetname] as PoolListContainer<Object>)?.poolList.poolstack.Count > 0)
                {
                    callback?.Invoke((PoolDic[assetname] as PoolListContainer<Object>).poolList.Get());
                }
                else
                {
                    AssetManager.Instance.LoadAssetAsync<Object>(bundlepath, assetname, path, mainABName, (o) =>
                        {
                            if (o is GameObject)
                                o.name = assetname;
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
                    AssetManager.Instance.LoadAssetAsync<Object>(bundlepath, assetname, path, mainABName, (o) =>
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
        #region Push
        /// <summary>
        /// 从将一个Object对象放入对象池
        /// </summary>
        /// <param name="path">在AB包下Asset路径或预制体在Resources下的路径（取决于pop时传入的assetpath或prefabpath）</param>
        /// <param name="obj">需要放入对象池的Object</param>
        public void Push<T>(string path, T obj, UnityAction callback = null, bool isCreateParent = true,string tag = "") where T : UnityEngine.Object
        {
            if (callback == null)
                callback = () => { };
            if (tag != "")
                path = tag;

            if (obj is GameObject o)
            {
                //场景中若没用对象池则创建一个
                if (objPool == null && isCreateParent)
                    objPool = new GameObject("GameObjectPool");

                //若 字典不存在PoolList 则 创建PoolList
                if (!PoolDic.ContainsKey(path))
                {
                    if(isCreateParent)
                        PoolDic.Add(path, new PoolListContainer<GameObject>(objPool, o));
                    else
                        PoolDic.Add(path, new PoolListContainer<GameObject>(null, o));
                }
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
        public void Push(string path, Object obj, UnityAction callback = null, bool isCreateParent = true,string tag = "")
        {
            if (tag != "")
                path = tag;

            if (obj is GameObject o)
            {
                //场景中若没用对象池则创建一个
                if (objPool == null && isCreateParent)
                    objPool = new GameObject("GameObjectPool");

                //若 字典不存在PoolList 则 创建PoolList
                if (!PoolDic.ContainsKey(path))
                {
                    if(isCreateParent)
                        PoolDic.Add(path, new PoolListContainer<GameObject>(objPool, o));
                    else
                        PoolDic.Add(path, new PoolListContainer<GameObject>(null, o));
                }
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
                    TimerManager.Instance.RemoveTask(currentTaskIndex);
            }
            else
            {
                currentTaskIndex = TimerManager.Instance.DelayTimeExcute(secondTime, 0, ClearAll);
            }

        }
        void SmartClean()
        {
            if (!enableSmartClean || GCtime == -1)
                return;

            if (currentTaskIndex != 0)
                TimerManager.Instance.RemoveTask(currentTaskIndex);

            currentTaskIndex = TimerManager.Instance.DelayTimeExcute(GCtime, ClearAll);


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
            if(objPool!=null)
            {
                parentobj = new GameObject(obj.name + "_Pool");
                parentobj.transform.parent = objPool.transform;                
            }
            poolstack = new Stack<T>();
        }
        public PoolList(GameObject objPool)
        {
            if(objPool!=null)
            {
                parentobj.transform.parent = objPool.transform;
            }
            poolstack = new Stack<T>();
        }
        public PoolList()
        {
            poolstack = new Stack<T>();
        }
        public Stack<T> poolstack;
        public GameObject parentobj = null;

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
                // if (o.GetComponent<ECS.GameObjectToEntity>() != null)
                //     throw new CustomErrorException("Can't push GameObject with GameObjectToEntity component into the pool!");
                callback();
                o.SetActive(false);
                o.transform.parent = parentobj?.transform;
                poolstack.Push(o as T);
                return;
            }
            callback();
            poolstack.Push(obj);
        }
    }

}
