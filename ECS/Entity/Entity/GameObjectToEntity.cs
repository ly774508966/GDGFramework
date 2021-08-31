using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GDG.ECS;
namespace GDG.ECS
{
    public class GameObjectToEntity : MonoBehaviour
    {
        private GameEntity entity;
        private UnityAction<GameEntity, EntityManager> proxyConverts;//在所有System初始结束后才会执行
        private void Awake()
        {
            proxyConverts = null;
            foreach (var item in this.gameObject.GetComponents<IEntityProxy>())
            {
                proxyConverts += item.Convert;
            }
            BaseWorld.Instance.monoWorld.AddOrRemoveListener(ProxyConvertExcute, "ProxyConvertExcute");
        }
        void ProxyConvertExcute()
        {
            entity = World.EntityManager.CreateGameEntity(0,false);
            entity.gameObject = this.gameObject;
            if (proxyConverts != null)
            {
                proxyConverts(entity, World.EntityManager);
            }
        }
    }
}