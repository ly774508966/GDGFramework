using System;
using System.Collections.Generic;
using GDG.Utils;
using UnityEngine;

namespace GDG.Async
{
    public class WaitTime : IYieldInstruction
    {
        public bool CanMoveNext { get => time < Time.realtimeSinceStartup; }
        private double time;

        public WaitTime(float delaySecondTime)
        {
            this.time = Time.realtimeSinceStartup + delaySecondTime;
        }
    }
}