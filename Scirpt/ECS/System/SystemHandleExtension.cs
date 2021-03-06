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
        /// ????????????????????????
        /// </summary>
        public static SystemHandleBase ReturnQueryResult(this SystemHandleBase handle, out List<Entity> result)
        {
            result = handle.result;
            return handle;
        }
        /// <summary>
        /// ?????????????????? entity ????????? secondTime ?????????
        /// </summary>
        /// <param name="secondTime">????????????????????????????????????</param>
        /// <param name="selectId">????????? Select ?????????id?????????????????? SelectId ????????????????????? int.MinValue</param>
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
        /// ?????????????????? entity ????????? frame ?????????
        /// </summary>
        /// <param name="frame">??????????????????</param>
        /// <param name="selectId">????????? Select ?????????id?????????????????? SelectId ????????????????????? int.MinValue</param>
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
        /// ???????????? EventCenter ???????????????????????????????????????Excute??????
        /// </summary>
        /// <param name="eventName">????????????</param>///
        /// <param name="selectId">????????? Select ?????????id?????????????????? SelectId ????????????????????? int.MinValue</param>/// 
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
        /// ??????????????????
        /// </summary>
        public static SystemHandleBase CanBeEexcuted(this SystemHandleBase handle, bool canBeExcuted)
        {
            handle.excuteInfo.canBeExcuted = canBeExcuted;
            return handle;
        }
    }
}
