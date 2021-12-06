using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;

namespace GDG.ECS
{
    /// <summary>
    /// SystemHandle的Excute信息
    /// </summary>
    public struct ExcuteInfo
    {
        public int selectId;
        public bool canBeExcuted;
        public ulong excuteFrame;
        public double excuteTime;
        public ushort delayFrame;
        public double delayTime;
        public string eventName;//监听事件名
        public override string ToString()
        {
            return $"selectId: {selectId} \ndelayFrame: {delayFrame} \ndelayTime: {delayTime} \neventName: {eventName}";
        }

        public ExcuteInfo(int selectId, bool canBeExcuted, ulong excuteFrame, double excuteTime, ushort delayFrame, double delayTime, string eventName)
        {
            this.selectId = selectId;
            this.canBeExcuted = canBeExcuted;
            this.excuteFrame = excuteFrame;
            this.excuteTime = excuteTime;
            this.delayFrame = delayFrame;
            this.delayTime = delayTime;
            this.eventName = eventName;
        }
        public void Init()
        {
            selectId = int.MinValue;
            canBeExcuted =  true;
            excuteFrame = ulong.MaxValue;
            excuteTime = double.MaxValue;
            delayFrame = 0;
            delayTime = 0;
            eventName = null;    
        }
    }
}