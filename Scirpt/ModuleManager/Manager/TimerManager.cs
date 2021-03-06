using System.Linq;
using System;
using System.Collections.Generic;
using GDG.Base;

namespace GDG.ModuleManager
{
    public enum TimeUnit
    {
        Millisecond,
        Second,
        Minute,
        Hour,
        Day
    }
    public abstract class TimerHandle
    {
        public uint loopTimes;
        public ulong index;
        public Action callback;
    }
    public class TimeHandle : TimerHandle
    {
        public double excuteTime;
        public double delayTime;
        public TimeHandle(ulong index, double excuteTime, double delayTime, Action callback, uint loopTimes)
        {
            this.index = index;
            this.excuteTime = excuteTime;
            this.delayTime = delayTime;
            this.callback = callback;
            this.loopTimes = loopTimes;
        }
    }
    public class FrameHandle : TimerHandle
    {
        public ulong excuteFrame;
        public ushort delayFrame;
        public FrameHandle(ulong index, ulong excuteFrame, ushort delayFrame, Action callback, uint loopTimes)
        {
            this.index = index;
            this.excuteFrame = excuteFrame;
            this.delayFrame = delayFrame;
            this.callback = callback;
            this.loopTimes = loopTimes;
        }
    }
    public class TimerManager:LazySingleton<TimerManager>
    {
        public TimerManager()
        {
            MonoWorld.Instance.AddOrRemoveListener(OnUpdate, "Update");
        }
        private ulong maxIndex = 0;
        private static readonly object locker = new object();
        private DateTime beginTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private List<TimerHandle> m_TempList = new List<TimerHandle>();
        private List<TimerHandle> m_TimerHandleList = new List<TimerHandle>();
        private List<ulong> m_IndexList = new List<ulong>();
        private double GetCurrentTime()
        {
            TimeSpan ts = DateTime.UtcNow - beginTime;
            return ts.TotalMilliseconds;
        }
        public ulong CurrentFrame { get; private set; }
        public double CurrentTime { get => GetCurrentTime(); }
        private ulong GetIndex()
        {
            lock (locker)
            {
                maxIndex++;
            }
            return maxIndex;
        }
        internal void OnUpdate()
        {
            CurrentFrame++;
            //?????????????????????
            foreach (var item in m_TempList)
            {
                m_TimerHandleList.Add(item);
            }
            m_TempList.Clear();

            //???????????????????????????handle?????????????????????
            for (int i = m_TimerHandleList.Count - 1; i >= 0; i--)
            {
                var handle = m_TimerHandleList[i];

                //????????????
                if (handle is TimeHandle tHandle)
                {
                    if (GetCurrentTime() < tHandle.excuteTime)
                        continue;
                    else
                    {
                        tHandle.callback?.Invoke();
                        if (tHandle.loopTimes == 0)
                        {
                            tHandle.excuteTime += tHandle.delayTime;
                            continue;
                        }
                    }

                }
                //?????????
                else if (handle is FrameHandle fHandle)
                {
                    if (CurrentFrame < fHandle.excuteFrame)
                        continue;
                    else
                    {
                        fHandle.callback?.Invoke();
                        if (fHandle.loopTimes == 0)
                        {
                            fHandle.excuteFrame += fHandle.delayFrame;
                            continue;
                        }
                    }
                }

                if (--handle.loopTimes == 0)
                    m_TimerHandleList.RemoveAt(i);
                else
                {
                    if (handle is TimeHandle t)
                        t.excuteTime += t.delayTime;
                    else if (handle is FrameHandle f)
                        f.excuteFrame += f.excuteFrame;
                }
            }
        }
        public double TimeUnitToMillisecond(double time, TimeUnit timeUnit)
        {
            switch (timeUnit)
            {
                case TimeUnit.Second: return time * 1000;
                case TimeUnit.Minute: return time * 1000 * 60;
                case TimeUnit.Hour: return time * 1000 * 60 * 60;
                case TimeUnit.Day: return time * 1000 * 60 * 60 * 24;
                default: return time;
            }
        }
        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="delayTime">????????????</param>
        /// <param name="taskCallback">???????????????????????????</param>
        /// <param name="timeUnit">???????????????????????????</param>
        /// <returns>???????????????index</returns>
        public ulong DelayTimeExcute(double delayTime, Action taskCallback, TimeUnit timeUnit = TimeUnit.Second)
        {
            delayTime = TimeUnitToMillisecond(delayTime, timeUnit);
            var index = GetIndex();
            m_TempList.Add(new TimeHandle(index, GetCurrentTime() + delayTime, delayTime, taskCallback, 1));
            return index;
        }
        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="delayTime">????????????</param>
        /// <param name="taskCallback">???????????????????????????</param>
        /// <param name="LoopTimes">??????????????????????????????0</param>
        /// <param name="timeUnit">???????????????????????????</param>
        /// <returns>???????????????index</returns>
        public ulong DelayTimeExcute(double delayTime, uint LoopTimes ,Action taskCallback, TimeUnit timeUnit = TimeUnit.Second)
        {
            delayTime = TimeUnitToMillisecond(delayTime, timeUnit);
            var index = GetIndex();
            m_TempList.Add(new TimeHandle(index, GetCurrentTime() + delayTime, delayTime, taskCallback, LoopTimes));
            return index;
        }
        /// <summary>
        /// ?????????????????????
        /// </summary>
        /// <param name="delayFrame">????????????</param>
        /// <param name="taskCallback">???????????????????????????</param>
        /// <returns>???????????????index</returns>
        public ulong DelayFrameExcute(ushort delayFrame, Action taskCallback)
        {
            var index = GetIndex();
            m_TempList.Add(new FrameHandle(index, CurrentFrame + delayFrame, delayFrame, taskCallback, 1));
            return index;
        }
        /// <summary>
        /// ?????????????????????
        /// </summary>
        /// <param name="delayFrame">????????????</param>
        /// <param name="taskCallback">???????????????????????????</param>
        /// <param name="loopTimes">??????????????????????????????0</param>
        /// <returns>???????????????index</returns>
        public ulong DelayFrameExcute(ushort delayFrame,uint loopTimes,Action taskCallback)
        {
            var index = GetIndex();
            m_TempList.Add(new FrameHandle(index, CurrentFrame + delayFrame, delayFrame, taskCallback, loopTimes));
            return index;
        }
        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="index">?????????????????????index</param>
        public bool RemoveTask(ulong index)
        {
            for (int i = 0; i < m_TimerHandleList.Count; i++)
            {
                if(m_TimerHandleList[i].index == index)
                {
                    m_TimerHandleList.RemoveAt(i);
                    for (int j = 0; j <m_TempList.Count; j++)
                    {
                        if(m_TempList[j].index == index)
                            m_TempList.RemoveAt(j);
                    }
                    for (int k = 0; k < m_IndexList.Count;k++)
                    {
                        if(m_IndexList[k]==index)
                            m_IndexList.RemoveAt(k);
                    }
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="index">?????????????????????index</param>
        /// <param name="taskCallback">???????????????????????????</param>
        public void ReplaceTask(ulong index , Action taskCallback)
        {
            foreach(var item in m_TimerHandleList)
            {
                if(item.index == index)
                    item.callback = taskCallback;
            }
            foreach(var item in m_TempList)
            {
                if(item.index == index)
                    item.callback = taskCallback;
            }
        }
    }
}