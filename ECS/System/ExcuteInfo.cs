using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;

namespace GDG.ECS
{
    public class ExcuteInfo
    {
        public int selectId = int.MinValue;
        public bool canBeExcuted = true;
        public ulong excuteFrame = ulong.MaxValue;
        public double excuteTime = double.MaxValue;
        public ushort delayFrame = 0;
        public double delayTime = 0;
        public string eventName;
    }
}