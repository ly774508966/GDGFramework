using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GDG.ECS;
namespace GDG.ECS
{
    public class GameObjectToEntity : MonoBehaviour
    {
        private Entity entity;
        private UnityAction<Entity, EntityManager> proxyConverts;//在 LateUpdate 中执行一次
        private void Awake()
        {
            proxyConverts = null;
            foreach (var item in this.gameObject.GetComponents<IEntityProxy>())
            {
                proxyConverts += item.Convert;
            }
            BaseWorld.Instance.monoWorld.AddOrRemoveListener(ProxyConvertExcute, "LateUpdate");
        }
        void ProxyConvertExcute()
        {
            entity = World.EntityManager.CreateEntity<GameObjectComponent>();
            World.EntityManager.SetComponentData<GameObjectComponent>(entity, new GameObjectComponent() { gameObject = this.gameObject });

            if (proxyConverts != null)
            {
                proxyConverts(entity, World.EntityManager);
            }
            BaseWorld.Instance.monoWorld.AddOrRemoveListener(ProxyConvertExcute, "LateUpdate",false);
        }
    }
}