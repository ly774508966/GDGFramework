using System.Collections;
using System.Collections.Generic;
using GDG.ECS;
using GDG.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace GDG.ModuleManager
{
    /// <summary>
    /// 管理AssetBundle的工具类，提供了加载Asset和卸载Asset的方法，注意：若要使用此类，需将AssetBundle的文件夹路径设为StreamingAssets的路径,并且根据打包平台的需求，分别在以下文件夹打包：</param>
    /// 安卓：Android</param>
    /// IOS：IOS</param>
    /// PC：PC</param>
    /// </summary>
    public class AssetManager : AbsLazySingleton<AssetManager>
    {
        public AssetManager()
        {
            UserFileManager.BuildFolder_Async("PC", Application.streamingAssetsPath);
            UserFileManager.BuildFolder_Async("Android", Application.streamingAssetsPath);
            UserFileManager.BuildFolder_Async("IOS", Application.streamingAssetsPath);
        }
        //AB包文件夹路径
        public string Path { get => Application.streamingAssetsPath + "/"; }
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
        private void LoadMainAndManifest(string bundlename)
        {
            //加载主包和描述文件
            if (mainAB == null)
            {
                mainAB = AssetBundle.LoadFromFile(Path + MainABName);
                manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }

            //加载目标包
            AssetBundle bundle = null;
            if (!ABDic.ContainsKey(bundlename))
            {
                bundle = AssetBundle.LoadFromFile(Path + bundlename);
                if (bundle == null)
                    Log.Error($"Load bundle'{Path}{bundlename}' failed");
                ABDic.Add(bundlename, bundle);
            }


            //从描述文件获得目标包依赖
            string[] infos = manifest.GetAllDependencies(bundlename);

            //遍历加载依赖包
            foreach (var item in infos)
            {
                if (!ABDic.ContainsKey(item))
                {
                    AssetBundle.LoadFromFile(Path + item);
                    ABDic.Add(item, bundle);
                }
            }
        }

        //同步加载
        /// <summary>
        /// 通过泛型同步加载Asset
        /// bundlename: Asset所在包名
        /// asset：asset名称(为了避免因为重名情况导致的问题，建议添加文件后缀名)
        /// </summary>
        /// <param name="bundlename"></param>
        /// <param name="assetname"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadAsset<T>(string bundlename, string assetname) where T : Object
        {
            LoadMainAndManifest(bundlename);

            //加载目标包
            AssetBundle bundle = ABDic[bundlename];

            //返回目标包资源
            T obj = bundle.LoadAsset(assetname) as T;
            return obj is GameObject ? GameObject.Instantiate<T>(obj) : obj;
        }
        /// <summary>
        /// 通过参数指定类型同步加载Asset
        /// bundlename: Asset所在包名
        /// asset：asset名称(为了避免因为重名情况导致的问题，建议添加文件后缀名)
        /// type：asset的类型
        /// </summary>
        /// <param name="bundlename"></param>
        /// <param name="assetname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object LoadAsset(string bundlename, string assetname, System.Type type)
        {
            LoadMainAndManifest(bundlename);

            //加载目标包
            AssetBundle bundle = ABDic[bundlename];

            //返回目标包资源
            Object obj = bundle.LoadAsset(assetname, type);
            return obj is GameObject ? GameObject.Instantiate(obj) : obj;

        }


        //异步加载
        public void LoadAssetAsync<T>(string bundlename, string assetname, UnityAction<T> callback) where T : Object
        {
            World.monoWorld.StartCoroutine(CoroutineLoadAssetAsync<T>(bundlename, assetname, callback));
        }
        public void LoadAssetAsync(string bundlename, string assetname, System.Type type, UnityAction<Object> callback)
        {
            World.monoWorld.StartCoroutine(CoroutineLoadAssetAsync(bundlename, assetname, type, callback));
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