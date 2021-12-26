using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GDG.ModuleManager;
using UnityEngine.Events;
using GDG.Utils;

namespace GDG.ECS
{
    public static class SystemHandleExtension
    {
        #region WithAll
        public static SystemHandleBase WithAll<T>(this SystemHandleBase handle) where T : IComponent
        {
            for (int i = handle.result.Count - 1; i >= 0; i--)
            {
                if (!handle.result[i].IsExistComponent<T>())
                    handle.result.RemoveAt(i);
            }
            return handle;
        }
        public static SystemHandleBase WithAll<T1, T2>(this SystemHandleBase handle) where T1 : IComponent where T2 : IComponent
        {
            for (int i = handle.result.Count - 1; i >= 0; i--)
            {
                if (!handle.result[i].IsExistComponent<T1>() || !handle.result[i].IsExistComponent<T2>())
                    handle.result.RemoveAt(i);
            }
            return handle;
        }
        public static SystemHandleBase WithAll<T1, T2, T3>(this SystemHandleBase handle) where T1 : IComponent where T2 : IComponent where T3 : IComponent
        {
            for (int i = handle.result.Count - 1; i >= 0; i--)
            {
                if (!handle.result[i].IsExistComponent<T1>() || !handle.result[i].IsExistComponent<T2>() || !handle.result[i].IsExistComponent<T3>())
                    handle.result.RemoveAt(i);
            }
            return handle;
        }
        public static SystemHandleBase WithAll<T1, T2, T3, T4>(this SystemHandleBase handle) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
        {
            for (int i = handle.result.Count - 1; i >= 0; i--)
            {
                if (!handle.result[i].IsExistComponent<T1>() || !handle.result[i].IsExistComponent<T2>() || !handle.result[i].IsExistComponent<T3>() || !handle.result[i].IsExistComponent<T4>())
                    handle.result.RemoveAt(i);
            }
            return handle;
        }
        #endregion
        #region WithNone
        public static SystemHandleBase WithNone<T>(this SystemHandleBase handle) where T : IComponent
        {
            for (int i = handle.result.Count - 1; i >= 0; i--)
            {
                if (handle.result[i].IsExistComponent<T>())
                    handle.result.RemoveAt(i);
            }
            return handle;
        }
        public static SystemHandleBase WithNone<T1, T2>(this SystemHandleBase handle) where T1 : IComponent where T2 : IComponent
        {
            for (int i = handle.result.Count - 1; i >= 0; i--)
            {
                if (handle.result[i].IsExistComponent<T1>() || handle.result[i].IsExistComponent<T2>())
                    handle.result.RemoveAt(i);
            }
            return handle;
        }
        public static SystemHandleBase WithNone<T1, T2, T3>(this SystemHandleBase handle) where T1 : IComponent where T2 : IComponent where T3 : IComponent
        {
            for (int i = handle.result.Count - 1; i >= 0; i--)
            {
                if (handle.result[i].IsExistComponent<T1>() || handle.result[i].IsExistComponent<T2>() || handle.result[i].IsExistComponent<T3>())
                    handle.result.RemoveAt(i);
            }
            return handle;
        }
        public static SystemHandleBase WithNone<T1, T2, T3, T4>(this SystemHandleBase handle) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
        {
            for (int i = handle.result.Count - 1; i >= 0; i--)
            {
                if (handle.result[i].IsExistComponent<T1>() || handle.result[i].IsExistComponent<T2>() || handle.result[i].IsExistComponent<T3>() || handle.result[i].IsExistComponent<T4>())
                    handle.result.RemoveAt(i);
            }
            return handle;
        }
        #endregion
        #region WithAny
        public static SystemHandleBase WithAny<T1, T2>(this SystemHandleBase handle) where T1 : IComponent where T2 : IComponent
        {
            for (int i = handle.result.Count - 1; i >= 0; i--)
            {
                if (!handle.result[i].IsExistComponent<T1>() && !handle.result[i].IsExistComponent<T2>())
                    handle.result.RemoveAt(i);
            }
            return handle;
        }
        public static SystemHandleBase WithAny<T1, T2, T3>(this SystemHandleBase handle) where T1 : IComponent where T2 : IComponent where T3 : IComponent
        {
            for (int i = handle.result.Count - 1; i >= 0; i--)
            {
                if (!handle.result[i].IsExistComponent<T1>() && !handle.result[i].IsExistComponent<T2>() && !handle.result[i].IsExistComponent<T3>())
                    handle.result.RemoveAt(i);
            }
            return handle;
        }
        public static SystemHandleBase WithAny<T1, T2, T3, T4>(this SystemHandleBase handle) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
        {
            for (int i = handle.result.Count - 1; i >= 0; i--)
            {
                if (!handle.result[i].IsExistComponent<T1>() && !handle.result[i].IsExistComponent<T2>() && !handle.result[i].IsExistComponent<T3>() && !handle.result[i].IsExistComponent<T4>())
                    handle.result.RemoveAt(i);
            }
            return handle;
        }
        #endregion
        /// <summary>
        /// 返回查询到的实体
        /// </summary>
        public static SystemHandleBase ReturnQueryResult(this SystemHandleBase handle, out List<Entity> result)
        {
            result = handle.result;
            return handle;
        }
        /// <summary>
        /// 查询结果中的 entity 将每隔 secondTime 秒执行
        /// </summary>
        /// <param name="secondTime">延迟执行的时间，单位为秒</param>
        /// <param name="selectId">指定的 Select 的唯一id，不能与其他 SelectId 重复，且不能为 int.MinValue</param>
        public static void ExcuteDelayTime(this SystemHandleBase handle, float secondTime, int selectId)
        {
            if (handle.system.TryGetExcuteInfo(selectId, out ExcuteInfo _excuteInfo))
            {
                handle.excuteInfo = _excuteInfo;
            }
            else
            {
                handle.excuteInfo.excuteTime = TimerManager.Instance.CurrentTime + secondTime;
                handle.excuteInfo.delayTime = secondTime;
                handle.excuteInfo.selectId = selectId;
            }

            handle.Excute();
        }
        /// <summary>
        /// 查询结果中的 entity 将每隔 frame 帧执行
        /// </summary>
        /// <param name="frame">延迟执行帧数</param>
        /// <param name="selectId">指定的 Select 的唯一id，不能与其他 SelectId 重复，且不能为 int.MinValue</param>
        public static void ExcuteDelayFrame(this SystemHandleBase handle, ushort frame, int selectId)
        {
            if (handle.system.TryGetExcuteInfo(selectId, out ExcuteInfo _excuteInfo))
            {
                handle.excuteInfo = _excuteInfo;
            }
            else
            {
                handle.excuteInfo.excuteFrame = TimerManager.Instance.CurrentFrame + frame;
                handle.excuteInfo.delayFrame = frame;
                handle.excuteInfo.selectId = selectId;
            }

            handle.Excute();
        }
        /// <summary>
        /// 直到通过 EventCenter 注册的事件被触发，才会调用Excute方法
        /// </summary>
        /// <param name="eventName">事件名称</param>///
        /// <param name="selectId">指定的 Select 的唯一id，不能与其他 SelectId 重复，且不能为 int.MinValue</param>/// 
        public static void ExcuteWithEvent(this SystemHandleBase handle, string eventName, int selectId)
        {
            if (handle.system.TryGetExcuteInfo(selectId, out ExcuteInfo _excuteInfo))
            {
                handle.excuteInfo = _excuteInfo;
            }
            else
            {
                handle.excuteInfo.selectId = selectId;
                handle.excuteInfo.eventName = eventName;
            }

            handle.Excute();
        }
        /// <summary>
        /// 是否允许执行
        /// </summary>
        public static SystemHandleBase CanBeEexcuted(this SystemHandleBase handle, bool canBeExcuted)
        {
            handle.excuteInfo.canBeExcuted = canBeExcuted;
            return handle;
        }
    }
}
