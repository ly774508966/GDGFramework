using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ECS
{
    public class GameEntity:Entity
    {
        public GameObject gameObject;
        internal bool isCreateGameObject = false;
        public override void OnInit()
        {
            base.OnInit();
            if(isCreateGameObject)
            {
                gameObject = new GameObject();
                gameObject.name = $"Entity {this.Index}";                
            }
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
            Object.Destroy(gameObject);

        }

    }
}