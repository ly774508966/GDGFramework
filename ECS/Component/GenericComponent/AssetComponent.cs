using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using GDG.Utils;

namespace GDG.ECS
{
    public class AssetComponent : IComponent,IEntityRecyclable
    {
        public Object asset;
        public string assetName;
        public void RecycleEntity(Entity entity)
        {
            if(asset is GameObject gameObject)
            {    
                GDGTools.AssetPool.Push<GameObject>(assetName, gameObject, null, false);
                gameObject.SetActive(false);
                gameObject?.transform.SetParent(World.monoWorld.EntityPool.transform);
            }
            else
            {
                GDGTools.AssetPool.Push<Object>(assetName, asset, null, false);
            }
        }
    }
}