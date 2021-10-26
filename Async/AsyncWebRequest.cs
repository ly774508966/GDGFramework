using System.Reflection;
using System.Net;
using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GDG.Async;
using System.Threading.Tasks;
using GDG.Utils;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

namespace GDG.Async
{
    public class DownloadInfo
    {
        public float progress;
        public string url;
        public bool isFinished = false;
        public bool isFailed = false;
    }
    public class UploadInfo
    {
        public float progress;
        public string url;
        public bool isFinished = false;
        public bool isFailed = false;
    }
    public class AsyncWebRequest
    {
        public static DownloadInfo DownloadTexture2D(string url, Action<Texture2D> callback, Action<UnityWebRequest> requestCallback = null)
        {
            var downloadInfo = new DownloadInfo();
            ECS.World.monoWorld.StartCoroutine(AsyncWaitDownloadFinish<Texture2D>(downloadInfo, url, callback, default(AudioType), null, requestCallback));
            return downloadInfo;
        }
        public static DownloadInfo DownloadAssetBundle(string url, Action<AssetBundle> callback, Action<UnityWebRequest> requestCallback = null)
        {
            var downloadInfo = new DownloadInfo();
            ECS.World.monoWorld.StartCoroutine(AsyncWaitDownloadFinish<AssetBundle>(downloadInfo, url, callback, default(AudioType), null, requestCallback));
            return downloadInfo;
        }
        public static DownloadInfo DownloadData(string url, Action<byte[]> callback, Action<UnityWebRequest> requestCallback = null)
        {
            var downloadInfo = new DownloadInfo();
            ECS.World.monoWorld.StartCoroutine(AsyncWaitDownloadFinish<byte[]>(downloadInfo, url, callback, default(AudioType), null, requestCallback));
            return downloadInfo;
        }
        public static DownloadInfo DownloadAudioClip(string url, AudioType audioType, Action<AudioClip> callback, Action<UnityWebRequest> requestCallback = null)
        {
            var downloadInfo = new DownloadInfo();
            ECS.World.monoWorld.StartCoroutine(AsyncWaitDownloadFinish<AudioClip>(downloadInfo, url, callback, audioType, null, requestCallback));
            return downloadInfo;
        }
        public static DownloadInfo DownloadFile(string url, string filePath, Action<UnityWebRequest> callback = null)
        {
            var downloadInfo = new DownloadInfo();
            ECS.World.monoWorld.StartCoroutine(AsyncWaitDownloadFinish<FileInfo>(downloadInfo, url, null, default(AudioType), filePath, callback));
            return downloadInfo;
        }
        public static UploadInfo Post(string url, string postData, Action<UnityWebRequest> requestCallback = null)
        {
            var uploadInfo = new UploadInfo();
            ECS.World.monoWorld.StartCoroutine(PostAsync(uploadInfo, url, requestCallback, postData));
            return uploadInfo;
        }
        public static UploadInfo Post(string url, WWWForm wWWForm, Action<UnityWebRequest> requestCallback = null)
        {
            var uploadInfo = new UploadInfo();
            ECS.World.monoWorld.StartCoroutine(PostAsync(uploadInfo, url, requestCallback, null, wWWForm));
            return uploadInfo;
        }
        public static UploadInfo Post(string url, List<IMultipartFormSection> multipartFormSections, Action<UnityWebRequest> requestCallback = null)
        {
            var uploadInfo = new UploadInfo();
            ECS.World.monoWorld.StartCoroutine(PostAsync(uploadInfo, url, requestCallback, null, null, multipartFormSections));
            return uploadInfo;
        }
        public static UploadInfo Post(string url, Dictionary<string, string> formFields, Action<UnityWebRequest> requestCallback = null)
        {
            var uploadInfo = new UploadInfo();
            ECS.World.monoWorld.StartCoroutine(PostAsync(uploadInfo, url, requestCallback, null, null, null, formFields));
            return uploadInfo;
        }
        public static UploadInfo Put(string url, string bodyDataString ,Action<UnityWebRequest> requestCallback = null)
        {
            var uploadInfo = new UploadInfo();
            ECS.World.monoWorld.StartCoroutine(PutAsync(uploadInfo, url, requestCallback, bodyDataString));
            return uploadInfo;
        }
        public static UploadInfo Put(string url,  byte[] bodyData ,Action<UnityWebRequest> requestCallback = null)
        {
            var uploadInfo = new UploadInfo();
            ECS.World.monoWorld.StartCoroutine(PutAsync(uploadInfo, url, requestCallback, null , bodyData));
            return uploadInfo;
        }
        private static IEnumerator AsyncWaitDownloadFinish<T>(DownloadInfo downloadInfo, string url, Action<T> callback, AudioType audioType = AudioType.WAV, string path = null, Action<UnityWebRequest> requestCallback = null)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            downloadInfo.url = url;
            T result = default(T);

            if (typeof(T) == typeof(Texture2D)) request.downloadHandler = new DownloadHandlerTexture();
            else if (typeof(T) == typeof(AudioClip)) request.downloadHandler = new DownloadHandlerAudioClip(url, audioType);
            else if (typeof(T) == typeof(AssetBundle)) request.downloadHandler = new DownloadHandlerAssetBundle(url, uint.MaxValue);
            else if (typeof(T) == typeof(byte[])) request.downloadHandler = new DownloadHandlerBuffer();
            else if (typeof(T) == typeof(FileInfo)) request.downloadHandler = new DownloadHandlerFile(path);

            request.SendWebRequest();
            while (!request.isDone)
            {
                yield return null;
                downloadInfo.progress = request.downloadProgress;
            }

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Log.Error("Download from Web source failed ! Please check the network, or download the address correct !");
                downloadInfo.isFailed = true;
                downloadInfo.isFinished = true;
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Log.Error("Download from Web source failed ! Received an error as defined by the connection protocol !");
                downloadInfo.isFailed = true;
                downloadInfo.isFinished = true;
            }
            else if (request.result == UnityWebRequest.Result.DataProcessingError)
            {
                Log.Error("Download from Web source failed ! Maybe the data was corrupted or not in the correct format !");
                downloadInfo.isFailed = true;
                downloadInfo.isFinished = true;
            }
            else
            {
                downloadInfo.isFinished = true;

                if (request.downloadHandler.data is T t1) result = t1;
                else if (DownloadHandlerTexture.GetContent(request) is T t2) result = t2;
                else if (DownloadHandlerAudioClip.GetContent(request) is T t3) result = t3;
                else if (DownloadHandlerAssetBundle.GetContent(request) is T t4) result = t4;

                callback?.Invoke(result);
                requestCallback?.Invoke(request);
            }
            request.Dispose();
        }
        private static IEnumerator PostAsync(UploadInfo uploadInfo, string url, Action<UnityWebRequest> requestCallback, string postData = null, WWWForm wwwForm = null, List<IMultipartFormSection> multipartFormSections = null, Dictionary<string, string> formFields = null)
        {
            UnityWebRequest request = null;
            uploadInfo.url = url;

            if (postData != null)
                request = UnityWebRequest.Post(url, postData);
            else if (multipartFormSections != null)
                request = UnityWebRequest.Post(url, multipartFormSections);
            else if (wwwForm != null)
                request = UnityWebRequest.Post(url, wwwForm);
            else if (formFields != null)
                request = UnityWebRequest.Post(url, formFields);
           
            request.SendWebRequest();

            while (!request.isDone)
            {
                yield return null;
                uploadInfo.progress = request.uploadProgress;
            }
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Log.Error("Download from Web source failed ! Please check the network, or download the address correct !");
                uploadInfo.isFailed = true;
                uploadInfo.isFinished = true;
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Log.Error("Download from Web source failed ! Received an error as defined by the connection protocol !");
                uploadInfo.isFailed = true;
                uploadInfo.isFinished = true;
            }
            else if (request.result == UnityWebRequest.Result.DataProcessingError)
            {
                Log.Error("Download from Web source failed ! Maybe the data was corrupted or not in the correct format !");
                uploadInfo.isFailed = true;
                uploadInfo.isFinished = true;
            }
            else
            {
                uploadInfo.isFailed = true;
                requestCallback?.Invoke(request);
            }
            request.Dispose();
        }
        private static IEnumerator PutAsync(UploadInfo uploadInfo, string url, Action<UnityWebRequest> requestCallback, string bodyDataString = null, byte[] bodyData = null)
        {
            UnityWebRequest request = null;
            uploadInfo.url = url;

            if (bodyData != null)
                request = UnityWebRequest.Put(url, bodyData);
            if (bodyDataString != null)
                request = UnityWebRequest.Put(url, bodyDataString);
            
            request.SendWebRequest();

            while (!request.isDone)
            {
                yield return null;
                uploadInfo.progress = request.uploadProgress;
            }
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Log.Error("Download from Web source failed ! Please check the network, or download the address correct !");
                uploadInfo.isFailed = true;
                uploadInfo.isFinished = true;
            }
            else if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Log.Error("Download from Web source failed ! Received an error as defined by the connection protocol !");
                uploadInfo.isFailed = true;
                uploadInfo.isFinished = true;
            }
            else if (request.result == UnityWebRequest.Result.DataProcessingError)
            {
                Log.Error("Download from Web source failed ! Maybe the data was corrupted or not in the correct format !");
                uploadInfo.isFailed = true;
                uploadInfo.isFinished = true;
            }
            else
            {
                uploadInfo.isFailed = true;
                requestCallback?.Invoke(request);
            }
            request.Dispose();

        }
    }
}