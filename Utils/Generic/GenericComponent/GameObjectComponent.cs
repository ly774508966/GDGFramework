using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;

namespace GDG.Utils
{
    public class GameObjectComponent : IComponent, IDestroyable, IRecyclable, IEnable, ISetNameable
    {
        public GameObject gameObject;

        public void OnDestroy()
        {
            if (gameObject != null)
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
        public void SetName(Entity entity)
        {
            entity.Name = gameObject.name;
        }
    }
}