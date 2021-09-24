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
        private UnityAction<Entity, EntityManager> proxyConverts;//在所有System初始结束后才会执行
        private void Awake()
        {
            proxyConverts = null;
            foreach (var item in this.gameObject.GetComponents<IEntityProxy>())
            {
                proxyConverts += item.Convert;
            }
            BaseWorld.Instance.monoWorld.AddOrRemoveListener(ProxyConvertExcute, "AfterUpdate");
        }
        void ProxyConvertExcute()
        {
            entity = World.EntityManager.CreateGameEntity(this.gameObject);

            if (proxyConverts != null)
            {
                proxyConverts(entity, World.EntityManager);
            }
            BaseWorld.Instance.monoWorld.AddOrRemoveListener(ProxyConvertExcute, "AfterUpdate", false);
        }
    }
}