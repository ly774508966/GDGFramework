using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;

public class GameObjectComponent : IComponent,IDestroyable,IRecyclable,IEnable
{
    public GameObject gameObject;

    public void OnDestroy()
    {
        if(gameObject!=null)
            GameObject.Destroy(gameObject);
    }
    public void OnEnable()
    {
        gameObject?.SetActive(true);
    }

    public void OnRecycle()
    {
        gameObject?.SetActive(false);
    }
}
