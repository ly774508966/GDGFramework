using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using GDG.ECS;
using System;

namespace GDG.ModuleManager
{
    public class ResourcesManager : LazySingleton<ResourcesManager>
    {
        public T LoadResource<T>(string prefabpath) where T : Object
        {
            T res = Resources.Load<T>(prefabpath);
            if (res == null)
            {
                LogManager.Instance.LogError($"Can't Find Resources , Path : '{prefabpath}'");
            }
            if (res is GameObject)
                return GameObject.Instantiate(res);
            else
                return res;
        }
        public Object LoadResource(string prefabpath, Type type)
        {
            var res = Resources.Load(prefabpath, type);
            if (res == null)
            {
                LogManager.Instance.LogError($"Can't Find Resources , Path : '{prefabpath}'");
            }
            if (res is GameObject)
                return GameObject.Instantiate(res);
            else
                return res;
        }
        public void LoadResourceAsync<T>(string prefabpath, UnityAction<T> callback) where T : Object
        {
            World.monoWorld.StartCoroutine(CoroutineLoadAsync(prefabpath, callback));
        }
        public void LoadResourceAsync(string prefabpath, Type type, UnityAction<Object> callback)
        {
            World.monoWorld.StartCoroutine(CoroutineLoadAsync(prefabpath, type, callback));
        }
        private IEnumerator CoroutineLoadAsync<T>(string prefabpath, UnityAction<T> callback) where T : Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(prefabpath);
            yield return request;
            //加载完后执行回调函数
            if (request.asset == null)
                LogManager.Instance.LogError($"Can't Find Resources , Path : '{prefabpath}'");
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
                LogManager.Instance.LogError($"Can't Find Resources , Path : '{prefabpath}'");
            if (request.asset is GameObject)
                callback(GameObject.Instantiate(request.asset as GameObject));
            else
                callback(request.asset);
        }
    }
}