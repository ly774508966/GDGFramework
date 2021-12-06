using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using GDG.Base;
using System;
using GDG.Utils;

namespace GDG.ModuleManager
{
    public class ResourcesManager : LazySingleton<ResourcesManager>
    {
        public T LoadResource<T>(string prefabpath) where T : Object
        {
            T res = Resources.Load<T>(prefabpath);
            if (res == null)
            {
                Log.Error($"Can't Find Resources, Path : '{prefabpath}'");
            }
            if (res is GameObject)
                return GameObject.Instantiate(res).ClearNameWithClone();
            else
                return res;
        }
        public Object LoadResource(string prefabpath, Type type)
        {
            var res = Resources.Load(prefabpath, type);
            if (res == null)
            {
                Log.Error($"Can't Find Resources, Path : '{prefabpath}'");
            }
            if (res is GameObject)
                return GameObject.Instantiate(res).ClearNameWithClone();
            else
                return res;
        }
        public void LoadResourceAsync<T>(string prefabpath, UnityAction<T> callback) where T : Object
        {
            MonoWorld.Instance.StartCoroutine(CoroutineLoadAsync(prefabpath, callback));
        }
        public void LoadResourceAsync(string prefabpath, Type type, UnityAction<Object> callback)
        {
            MonoWorld.Instance.StartCoroutine(CoroutineLoadAsync(prefabpath, type, callback));
        }
        public void TryLoadResourceAsync<T>(string prefabpath,UnityAction<T> callback) where T:Object
        {
            MonoWorld.Instance.StartCoroutine(CoroutineTryLoadAsync(prefabpath, callback));
        }
        private IEnumerator CoroutineTryLoadAsync<T>(string prefabpath, UnityAction<T> callback) where T : Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(prefabpath);
            yield return request;
            //加载完后执行回调函数
            if (request.asset != null)
            {
                if (request.asset is GameObject)
                    callback(GameObject.Instantiate(request.asset as T));
                else
                    callback(request.asset as T);
            }
            else
                callback(null);
        }
        private IEnumerator CoroutineLoadAsync<T>(string prefabpath, UnityAction<T> callback) where T : Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(prefabpath);
            yield return request;
            //加载完后执行回调函数
            if (request.asset == null)
                Log.Error($"Can't Find Resources, Path : '{prefabpath}'");
            if (request.asset is GameObject)
                callback(GameObject.Instantiate(request.asset as T));
            else
                callback(request.asset as T);
        }
        private IEnumerator CoroutineLoadAsync(string prefabpath, Type type, UnityAction<Object> callback)
        {
            ResourceRequest request = Resources.LoadAsync(prefabpath, type);
            yield return request;

            //加载完后执行回调函数
            if (request.asset == null)
                Log.Error($"Can't Find Resources, Path : '{prefabpath}'");
            if (request.asset is GameObject)
                callback(GameObject.Instantiate(request.asset as GameObject));
            else
                callback(request.asset);
        }
    }
}