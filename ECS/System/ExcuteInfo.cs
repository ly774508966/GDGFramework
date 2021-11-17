using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;

namespace GDG.ECS
{
    /// <summary>
    /// SystemHandle的Excute信息
    /// </summary>
    public class ExcuteInfo
    {
        public int selectId = int.MinValue;//select的唯一id
        public bool canBeExcuted = true;//是否允许执行Excute方法
        public ulong excuteFrame = ulong.MaxValue;//延迟后的执行帧
        public double excuteTime = double.MaxValue;//延迟后的执行时间
        public ushort delayFrame = 0;//延迟执行帧
        public double delayTime = 0;//延迟执行时间
        public string eventName;//监听事件名
    }
}