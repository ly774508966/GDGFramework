using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using UnityEngine.SceneManagement;
using GDG.Utils;
using System.Text.RegularExpressions;

public static class UnityExtension
{
    #region GameObject
    public static T ClearNameWithClone<T>(this T obj)where T : Object
    {
        if(obj==null)
            return null;
        obj.name = Regex.Replace(obj.name, "\\(Clone\\)", "");
        return obj;
    }
    public static GameObject ClearNameWithClone(this GameObject gameObject)
    {
        if(gameObject==null)
            return null;
        gameObject.name = Regex.Replace(gameObject.name, "\\(Clone\\)", "");
        return gameObject;
    }
    /// <summary>
    /// 返回第一个为该名字的子物体
    /// </summary>
    public static GameObject FindChildWithName(this GameObject gameObject,string name,bool includeInactive = true)
    {
        foreach(var item in gameObject.GetComponentsInChildren<Transform>(includeInactive))
        {
            if(item.name.Equals(name))
            {
                return item.gameObject;
            }
        }
        return null;
    }
    public static T RequireComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }
    public static T RequireComponent<T>(this GameObject gameObject,out bool isAddComponent) where T : Component
    {
        isAddComponent = false;
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            isAddComponent = true;
            component = gameObject.AddComponent<T>();
        }
        return component;
    }
    public static void SetLayer(this GameObject gameObject, int layer, bool withChildren = true)
    {
        if (gameObject.layer != layer)
        {
            gameObject.layer = layer;
            if (withChildren)
            {
                foreach (var tran in gameObject.GetComponentsInChildren<Transform>(true))
                {
                    tran.gameObject.layer = layer;
                }
            }
        }
    }
    public static void RemoveFromDontDestoryOnLoad(this GameObject gameObject)
    {
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
    }
    public static float Distance(this GameObject gameObject,GameObject other)
    {
        return Distance(gameObject.transform.position , other.transform.position);
    }
    public static Entity GetEntity(this GameObject gameObject)
    {
        foreach(var item in World.EntityManager.GetAllEntity())
        {
            if(item.TryGetComponent<GameObjectComponent>(out GameObjectComponent game))
            {
                if(game.gameObject == gameObject)
                    return item;
            }
        }
        return null;
    }
    public static bool TryGetEntity(this GameObject gameObject,out Entity entity)
    {
        entity = null;
        foreach(var item in World.EntityManager.GetAllEntity())
        {
            if(item.TryGetComponent<GameObjectComponent>(out GameObjectComponent game))
            {
                if(game.gameObject == gameObject)
                {
                    entity = item;
                    return true;
                }
            }
        }
        return false;
    }
    
    #endregion
    #region  Transform
    public static float Distance(this Transform trans,Transform other)
    {
        return Distance(trans.position , other.position);
    }
    public static void ResetTransform(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    public static void ResetLocalTransform(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    public static void CopyFrom(this Transform transform,Transform other)
    {
        transform.position = other.position;
        transform.rotation = other.rotation;
        transform.localScale = other.localScale;
    }
    public static void CopyTo(this Transform transform,Transform other)
    {
        other.position = transform.position;
        other.rotation = transform.rotation;
        other.localScale = transform.localScale;
    }
    public static void LookAtDirection(this Transform trans, Vector3 direction)
    {
        var pos = trans.position + direction;
        trans.LookAt(pos);
    }
    public static Entity GetEntity(this Transform transform)
    {
        return transform.gameObject.GetEntity();
    }
    public static bool TryGetEntity(this Transform transform,out Entity entity)
    {
        return transform.gameObject.TryGetEntity(out entity);
    }
    /// <summary>
    /// 返回第一个为该名字的子物体
    /// </summary>
    public static Transform FindChildWithName(this Transform trans,string name,bool includeInactive = true)
    {
        foreach(var item in trans.GetComponentsInChildren<Transform>(includeInactive))
        {
            if(item.name.Equals(name))
            {
                return item.transform;
            }
        }
        return null;
    }
    #endregion
    #region RectTransform
    public static void Reset(this RectTransform rectTransform)
    {
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
    }
    #endregion
    #region
    public static float Distance(this Vector3 position,Vector3 other)
    {
        return (position - other).magnitude;
    }
    #endregion
}
