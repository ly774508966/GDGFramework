using System;
using System.Collections;
using System.Collections.Generic;
using GDG.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace GDG.ECS
{
    public class GameEntity:Entity
    {
        public GameObject gameObject;

        internal override void OnInit()
        {   
            base.OnInit();
        }
        internal override void OnEnable()
        {
            base.OnEnable();
            gameObject.SetActive(true);
        }
        internal override void OnRecycle()
        {
            base.OnRecycle();
            gameObject.SetActive(false);
        }
        internal override void OnDestroy()
        {
            base.OnDestroy();
            UnityEngine.Object.Destroy(gameObject);

        }
        internal override void AddComponentToList(IComponent component)
        {
            if(component is GameObjectComponent)
                return;
            base.AddComponentToList(component);
        }
        internal override bool RemoveComponentToList(IComponent component)
        {
            if(component is GameObjectComponent)
                return false;
            return base.RemoveComponentToList(component);

        }
    }
}