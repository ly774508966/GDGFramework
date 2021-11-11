using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDG.ECS;
using GDG.Utils;
using UnityEditor;
using UnityEngine;

namespace GDG.RTS
{
	
	public class DeleteMissingScripts
	{
	    [MenuItem("GDGFramework/Tools/Remove Missing Script",false,4)]
	    static public void RemoveMissComponent()
	    {
	        var path = Application.dataPath;
	        bool isDeleteSuccessfully = true;
	        string[] getFoder = Directory.GetDirectories(path);
	        try
	        {
	            RemoveMissingScriptInScene();
	            RemoveMissingScriptInAssets();
	        }
	        catch (Exception)
	        {
	            isDeleteSuccessfully = false;
	            throw new CustomErrorException($"Delete failed!");
	        }
	        if(isDeleteSuccessfully)
	            Log.Editor("Delete successfully!");
	    }
	    //用于获取所有Hierarchy中的物体，包括被禁用的物体
	    static void RemoveMissingScriptInScene()
	    {
	        var transforms = Resources.FindObjectsOfTypeAll(typeof(Transform));
	        var previousSelection = Selection.objects;
	        Selection.objects = transforms.Cast<Transform>()
	            .Where(trans => trans != null)
	            .Select(trans => trans.gameObject)
	            .Cast<UnityEngine.Object>().ToArray();
	
	        var selectedTransforms = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
	        Selection.objects = previousSelection;
	        
	        var gameObjects =  selectedTransforms.Select(tr => tr.gameObject).ToList();
	
	        foreach(GameObject gameObject in gameObjects)
	        {
	            DeleteMissingScript(gameObject);
	        }
	    }
	    //BFS遍历Asset下所有文件夹并获取所有预制体
	    static void RemoveMissingScriptInAssets()
	    {
	        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
	
	        Queue<DirectoryInfo> dirInfoQueue = new Queue<DirectoryInfo>();
	        dirInfoQueue.Enqueue(directoryInfo);
	
	        DirectoryInfo currentDirInfo;
	
	        while(dirInfoQueue.Count > 0)
	        {
	            currentDirInfo = dirInfoQueue.Dequeue();
	            foreach(var dirInfo in currentDirInfo.GetDirectories())
	            {
	                dirInfoQueue.Enqueue(dirInfo);
	            }
	            foreach(var fileInfo in currentDirInfo.GetFiles())
	            {
	                if(fileInfo.Extension == ".prefab")
	                {
	                    var path = Regex.Replace(fileInfo.FullName, @".*Assets", "Assets");
	                    var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
	                    gameObject = GameObject.Instantiate<GameObject>(gameObject);
	                    if(gameObject != null)
	                    {
	                        DeleteMissingScript(gameObject);
	                        PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, path, InteractionMode.AutomatedAction);
	                        GameObject.DestroyImmediate(gameObject);
	                    }
	
	                }
	            }
	        }
	    }
	    static void DeleteMissingScript(GameObject gameObject)
	    {
	        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
	        foreach (Transform child in gameObject.transform)
	        {
	            DeleteMissingScript(child.gameObject);
	        }
	    }
	}
}