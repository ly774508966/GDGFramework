using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using UnityEngine.SceneManagement;

namespace GDG.ECS
{
    public class GameObjectComponent : IComponent
    {
        public GameObject gameObject{ get; internal set; }
        public Transform transform{ get => gameObject==null?null : gameObject.transform; }
        public void SetName(Entity entity)
        {
            if (gameObject != null)
            {
                entity.Name = gameObject.name;
            }
        }
    }
}