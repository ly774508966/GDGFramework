using UnityEngine;
using GDG.ModuleManager;

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
                AssetPool.Instance.Push<GameObject>(assetName, gameObject, null, false);
                gameObject.SetActive(false);
                gameObject?.transform.SetParent(World.monoWorld.EntityPool.transform);
            }
            else
            {
                AssetPool.Instance.Push<Object>(assetName, asset, null, false);
            }
        }
    }
}