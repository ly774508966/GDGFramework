using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using UnityEngine.SceneManagement;

public static class UnityExtension
{
    #region GameObject
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
    #endregion
    #region  Transform
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
}
