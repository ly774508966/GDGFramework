using System.Collections;
using System.Collections.Generic;
using GDG.ECS;
using GDG.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace GDG.ModuleManager
{
    /// <summary>
    /// 管理AssetBundle的工具类，提供了加载Asset和卸载Asset的方法，注意：若要使用此类，默认AssetBundle的文件夹路径为StreamingAssets的路径,并且根据打包平台的需求，分别在以下文件夹打包：</param>
    /// 安卓：Android</param>
    /// IOS：IOS</param>
    /// PC：PC</param>
    /// </summary>
    public class AssetManager : LazySingleton<AssetManager>
    {
        //AB包文件夹路径
        public string Path { get => Application.streamingAssetsPath; }
        //主包路径
        private string MainABName
        {
            get
            {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
                return "PC";
#endif
            }
        }

        //AB包字典，用于检查是否重复加载依赖包
        private Dictionary<string, AssetBundle> ABDic = new Dictionary<string, AssetBundle>();

        //主包
        private AssetBundle mainAB;
        //主包中的描述文件
        private AssetBundleManifest manifest;

        //加载主包和描述文件
        private void LoadMainAndManifest(string bundlename, string path = null, string mainABName = null)
        {
            if (mainABName == null)
                mainABName = MainABName;
            if (path == null)
                path = Path;
            //加载主包和描述文件
            if (mainAB == null)
            {
                mainAB = AssetBundle.LoadFromFile(path + "/" + mainABName);
                manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            //加载目标包
            AssetBundle bundle = null;
            if (!ABDic.ContainsKey(bundlename))
            {
                bundle = AssetBundle.LoadFromFile(path + "/" + bundlename);
                if (bundle == null)
                    Log.Error($"Load bundle'{path}{bundlename}' failed");
                ABDic.Add(bundlename, bundle);
            }
            //从描述文件获得目标包依赖
            string[] infos = manifest.GetAllDependencies(bundlename);

            //遍历加载依赖包
            foreach (var item in infos)
            {
                if (!ABDic.ContainsKey(item))
                {
                    AssetBundle.LoadFromFile(path + "/" + item);
                    ABDic.Add(item, bundle);
                }
            }
        }
        
        #region 同步加载
        public T LoadAsset<T>(string bundlename, string assetname, string path = null, string mainABName = null) where T : Object
        {
            LoadMainAndManifest(bundlename, path, mainABName);

            //加载目标包
            AssetBundle bundle = ABDic[bundlename];

            //返回目标包资源
            T obj = bundle.LoadAsset(assetname) as T;
            return obj is GameObject ? GameObject.Instantiate<T>(obj) : obj;
        }
        public Object LoadAsset(string bundlename, string assetname, System.Type type, string path = null, string mainABName = null)
        {
            LoadMainAndManifest(bundlename, path, mainABName);

            //加载目标包
            AssetBundle bundle = ABDic[bundlename];

            //返回目标包资源
            Object obj = bundle.LoadAsset(assetname, type);
            return obj is GameObject ? GameObject.Instantiate(obj) : obj;

        }
        #endregion
        #region 异步加载
        public void LoadAssetAsync<T>(string bundlename, string assetname, UnityAction<T> callback) where T : Object
        {
            World.monoWorld.StartCoroutine(CoroutineLoadAssetAsync<T>(bundlename, assetname, callback));
        }
        public void LoadAssetAsync<T>(string bundlename, string assetname, string path, string mainABName, UnityAction<T> callback) where T : Object
        {
            World.monoWorld.StartCoroutine(CoroutineLoadAssetAsync<T>(bundlename, assetname, path, mainABName, callback));
        }
        public void LoadAssetAsync(string bundlename, string assetname, System.Type type, UnityAction<Object> callback)
        {
            World.monoWorld.StartCoroutine(CoroutineLoadAssetAsync(bundlename, assetname, type, callback));
        }
        public void LoadAssetAsync(string bundlename, string assetname, string path, string mainABName, System.Type type, UnityAction<Object> callback)
        {
            World.monoWorld.StartCoroutine(CoroutineLoadAssetAsync(bundlename, assetname, path, mainABName, type, callback));
        }
        IEnumerator CoroutineLoadAssetAsync<T>(string bundlename, string assetname, UnityAction<T> callback) where T : Object
        {
            LoadMainAndManifest(bundlename);

            //异步加载目标包资源
            AssetBundleRequest request = ABDic[bundlename].LoadAssetAsync(assetname);
            yield return request;
            if (request.asset == null)
                Log.Error($"Load asset failed ! Bundle：'{bundlename}' Asset：'{assetname}'");
            if (request.asset is GameObject)
                callback(GameObject.Instantiate(request.asset as T));
            else
                callback(request.asset as T);
        }
        IEnumerator CoroutineLoadAssetAsync<T>(string bundlename, string assetname, string path, string mainABName, UnityAction<T> callback) where T : Object
        {
            LoadMainAndManifest(bundlename, path, mainABName);

            //异步加载目标包资源
            AssetBundleRequest request = ABDic[bundlename].LoadAssetAsync(assetname);
            yield return request;
            if (request.asset == null)
                Log.Error($"Load asset failed ! Bundle：'{bundlename}' Asset：'{assetname}'");
            if (request.asset is GameObject)
                callback(GameObject.Instantiate(request.asset as T));
            else
                callback(request.asset as T);
        }
        IEnumerator CoroutineLoadAssetAsync(string bundlename, string assetname, System.Type type, UnityAction<Object> callback)
        {
            LoadMainAndManifest(bundlename);

            //异步加载目标包资源
            AssetBundleRequest request = ABDic[bundlename].LoadAssetAsync(assetname, type);
            yield return request;
            if (request.asset == null)
                Log.Error($"Load asset failed ! Bundle：'{bundlename}' Asset：'{assetname}'");
            if (request.asset is GameObject)
                callback(GameObject.Instantiate(request.asset));
            else
                callback(request.asset);

        }
        IEnumerator CoroutineLoadAssetAsync(string bundlename, string assetname, string path, string mainABName, System.Type type, UnityAction<Object> callback)
        {
            LoadMainAndManifest(bundlename, path, mainABName);

            //异步加载目标包资源
            AssetBundleRequest request = ABDic[bundlename].LoadAssetAsync(assetname, type);
            yield return request;
            if (request.asset == null)
                Log.Error($"Load asset failed ! Bundle：'{bundlename}' Asset：'{assetname}'");
            if (request.asset is GameObject)
                callback(GameObject.Instantiate(request.asset));
            else
                callback(request.asset);

        }
        #endregion
        //卸载Asset
        public void UnloadAssetbundle(string bundlename)
        {
            if (ABDic.ContainsKey(bundlename))
            {
                ABDic[bundlename].Unload(false);
                ABDic.Remove(bundlename);
            }
            else
                Log.Error($"Unload bundle'{bundlename}' failed ！");
        }
        public void UnloadAssetbundleAll()
        {
            AssetBundle.UnloadAllAssetBundles(false);
            ABDic.Clear();
            mainAB = null;
            manifest = null;
        }
    }
}