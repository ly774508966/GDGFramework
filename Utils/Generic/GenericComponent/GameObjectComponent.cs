using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using UnityEngine.SceneManagement;

namespace GDG.Utils
{
    public class GameObjectComponent : IComponent, IDestroyable, IRecyclable, IEnable, ISetNameable
    {
        public GameObject gameObject;
        public Transform transform{ get => gameObject?.transform; }
        public void OnDestroy()
        {
            if (gameObject != null)
                GameObject.Destroy(gameObject);
        }
        public void OnEnable()
        {   
            gameObject?.transform.SetParent(null);
            gameObject?.RemoveFromDontDestoryOnLoad();
            this.gameObject?.SetActive(true);
        }

        public void OnRecycle()
        {
            this.gameObject?.SetActive(false);
            gameObject?.transform.SetParent(World.monoWorld.EntityPool.transform);
        }
        public void SetName(Entity entity)
        {
            entity.Name = gameObject.name;
        }
    }
}