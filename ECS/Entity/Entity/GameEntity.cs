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

        public override void OnInit()
        {   
            base.OnInit();
        }
        public override void OnEnable()
        {
            base.OnEnable();
            gameObject.SetActive(true);
        }
        public override void OnRecycle()
        {
            base.OnRecycle();
            gameObject.SetActive(false);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            UnityEngine.Object.Destroy(gameObject);

        }

    }
}