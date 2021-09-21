using System.Linq;
using System;
using System.Collections.Generic;

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
    public class TimerManager:AbsLazySingleton<TimerManager>
    {
        private ulong maxIndex = 0;
        private static readonly object locker = new object();
        private DateTime beginTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private List<TimerHandle> m_TempList = new List<TimerHandle>();
        // private List<TimerHandle> m_RecycalList = new List<TimerHandle>();
        private List<TimerHandle> m_TimerHandleList = new List<TimerHandle>();
        private List<ulong> m_IndexList = new List<ulong>();
        public double GetCurrentTime()
        {
            TimeSpan ts = DateTime.UtcNow - beginTime;
            return ts.TotalMilliseconds;
        }
        public ulong CurrentFrame;
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
            //从缓存中拿出来
            foreach (var item in m_TempList)
            {
                m_TimerHandleList.Add(item);
            }
            m_TempList.Clear();

            //对于达到执行时间的handle，直接执行回调
            for (int i = m_TimerHandleList.Count - 1; i >= 0; i--)
            {
                var handle = m_TimerHandleList[i];

                //时间定时
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
                //帧定时
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
        /// 延迟时间执行任务
        /// </summary>
        /// <param name="delayTime">延迟时间</param>
        /// <param name="taskCallback">延迟执行的回调方法</param>
        /// <param name="timeUnit">时间单位，默认为秒</param>
        /// <returns>返回任务的index</returns>
        public ulong DelayTimeExcute(double delayTime, Action taskCallback, TimeUnit timeUnit = TimeUnit.Second)
        {
            delayTime = TimeUnitToMillisecond(delayTime, timeUnit);
            var index = GetIndex();
            m_TempList.Add(new TimeHandle(index, GetCurrentTime() + delayTime, delayTime, taskCallback, 1));
            return index;
        }
        /// <summary>
        /// 延迟时间执行任务
        /// </summary>
        /// <param name="delayTime">延迟时间</param>
        /// <param name="taskCallback">延迟执行的回调方法</param>
        /// <param name="LoopTimes">循环次数，无限循环为0</param>
        /// <param name="timeUnit">时间单位，默认为秒</param>
        /// <returns>返回任务的index</returns>
        public ulong DelayTimeExcute(double delayTime, uint LoopTimes ,Action taskCallback, TimeUnit timeUnit = TimeUnit.Second)
        {
            delayTime = TimeUnitToMillisecond(delayTime, timeUnit);
            var index = GetIndex();
            m_TempList.Add(new TimeHandle(index, GetCurrentTime() + delayTime, delayTime, taskCallback, LoopTimes));
            return index;
        }
        /// <summary>
        /// 延迟帧执行任务
        /// </summary>
        /// <param name="delayFrame">延迟帧数</param>
        /// <param name="taskCallback">延迟执行的回调方法</param>
        /// <returns>返回任务的index</returns>
        public ulong DelayFrameExcute(ushort delayFrame, Action taskCallback)
        {
            var index = GetIndex();
            m_TempList.Add(new FrameHandle(index, CurrentFrame + delayFrame, delayFrame, taskCallback, 1));
            return index;
        }
        /// <summary>
        /// 延迟帧执行任务
        /// </summary>
        /// <param name="delayFrame">延迟帧数</param>
        /// <param name="taskCallback">延迟执行的回调方法</param>
        /// <param name="loopTimes">循环次数，无限循环为0</param>
        /// <returns>返回任务的index</returns>
        public ulong DelayFrameExcute(ushort delayFrame,uint loopTimes,Action taskCallback)
        {
            var index = GetIndex();
            m_TempList.Add(new FrameHandle(index, CurrentFrame + delayFrame, delayFrame, taskCallback, loopTimes));
            return index;
        }
        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="index">要移除的任务的index</param>
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
        /// 替换任务
        /// </summary>
        /// <param name="index">要替换的任务的index</param>
        /// <param name="taskCallback">延迟执行的回调方法</param>
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