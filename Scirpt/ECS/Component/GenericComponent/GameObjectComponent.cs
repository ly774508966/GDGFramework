using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using UnityEngine.SceneManagement;

namespace GDG.ECS
{
    public class GameObjectComponent : IComponent, IEntityDestoryable, IEntityRecyclable, IEntityEnable, IEntityNamedable
    {
        public GameObject gameObject;
        public Transform transform{ get => gameObject==null?null : gameObject.transform; }
        public void DestroyEntity(Entity entity)
        {
            if (gameObject != null)
                GameObject.Destroy(gameObject);
        }
        public void EnableEntity(Entity entity)
        {   
            if(!entity.TryGetComponent<AssetComponent>(out AssetComponent asset))
            {
                gameObject?.transform.SetParent(null);
                gameObject?.RemoveFromDontDestoryOnLoad();
                this.gameObject?.SetActive(true);                
            }

        }

        public void RecycleEntity(Entity entity)
        {
            if(!entity.TryGetComponent<AssetComponent>(out AssetComponent asset))
            {
                this.gameObject?.SetActive(false);
                gameObject?.transform.SetParent(World.monoWorld.EntityPool.transform);
            }
        }
        public void SetName(Entity entity)
        {
            entity.Name = gameObject.name;
        }
    }
}