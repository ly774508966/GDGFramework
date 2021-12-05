using System;
using System.Collections.Generic;
using GDG.Async;
using GDG.Utils;
using UnityEngine;

namespace GDG.Aysnc
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