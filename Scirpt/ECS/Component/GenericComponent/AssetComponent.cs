using UnityEngine;
using GDG.ModuleManager;

namespace GDG.ECS
{
    public class AssetComponent : IComponent
    {
        public Object asset { get; internal set; }
        public string assetName;

    }
}