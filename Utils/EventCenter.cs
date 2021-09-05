/*
 * @Author: 关东关 
 * @Date: 2021-05-22 20:14:47 
 * @Last Modified by: 关东关
 * @Last Modified time: 2021-05-22 22:49:02
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GDG.ModuleManager;
namespace GDG.Utils
{
    public class EventCenter
    {
        #region Action
        public static void AddActionListener(string eventName, UnityAction Event)
        => EventManager.Instance.AddActionListener(eventName, Event);
        public static void AddActionListener<T>(string eventName, UnityAction<T> Event)
        => EventManager.Instance.AddActionListener<T>(eventName, Event);
        public static void AddActionListener<T1, T2>(string eventName, UnityAction<T1, T2> Event)
        => EventManager.Instance.AddActionListener<T1, T2>(eventName, Event);
        public static void AddActionListener<T1, T2, T3>(string eventName, UnityAction<T1, T2, T3> Event)
        => EventManager.Instance.AddActionListener<T1, T2, T3>(eventName, Event);
        public static void AddActionListener<T1, T2, T3, T4>(string eventName, UnityAction<T1, T2, T3, T4> Event)
        => EventManager.Instance.AddActionListener<T1, T2, T3, T4>(eventName, Event);
        public static void RemoveActionListener(string eventName, UnityAction Event)
        => EventManager.Instance.RemoveActionListener(eventName, Event);
        public static void RemoveActionListener<T>(string eventName, UnityAction<T> Event)
        => EventManager.Instance.RemoveActionListener<T>(eventName, Event);
        public static void RemoveActionListener<T1, T2>(string eventName, UnityAction<T1, T2> Event)
        => EventManager.Instance.RemoveActionListener<T1, T2>(eventName, Event);
        public static void RemoveActionListener<T1, T2, T3>(string eventName, UnityAction<T1, T2, T3> Event)
        => EventManager.Instance.RemoveActionListener<T1, T2, T3>(eventName, Event);
        public static void RemoveActionListener<T1, T2, T3, T4>(string eventName, UnityAction<T1, T2, T3, T4> Event)
        => EventManager.Instance.RemoveActionListener<T1, T2, T3, T4>(eventName, Event);
        public static void ActionTrigger(string eventName)
        => EventManager.Instance.ActionTrigger(eventName);
        public static void ActionTrigger<T>(string eventName,T info)
        => EventManager.Instance.ActionTrigger<T>(eventName,info);
        public static void ActionTrigger<T1,T2>(string eventName,T1 info1,T2 info2)
        => EventManager.Instance.ActionTrigger<T1,T2>(eventName,info1,info2);
        public static void ActionTrigger<T1,T2,T3>(string eventName,T1 info1,T2 info2,T3 info3)
        => EventManager.Instance.ActionTrigger<T1,T2,T3>(eventName,info1,info2,info3);
        public static void ActionTrigger<T1,T2,T3,T4>(string eventName,T1 info1,T2 info2,T3 info3,T4 info4)
        => EventManager.Instance.ActionTrigger<T1,T2,T3,T4>(eventName,info1,info2,info3,info4);
        #endregion
        #region Func
        public static void AddFuncListener<R>(string eventName, Func<R> Event)
        => EventManager.Instance.AddFuncListener<R>(eventName, Event);
        public static void AddFuncListener<T1,R>(string eventName, Func<T1,R> Event)
        => EventManager.Instance.AddFuncListener<T1,R>(eventName, Event);
        public static void AddFuncListener<T1,T2,R>(string eventName, Func<T1,T2,R> Event)
        => EventManager.Instance.AddFuncListener<T1,T2,R>(eventName, Event);
        public static void AddFuncListener<T1,T2,T3,R>(string eventName, Func<T1,T2,T3,R> Event)
        => EventManager.Instance.AddFuncListener<T1,T2,T3,R>(eventName, Event);
        public static void AddFuncListener<T1,T2,T3,T4,R>(string eventName, Func<T1,T2,T3,T4,R> Event)
        => EventManager.Instance.AddFuncListener<T1,T2,T3,T4,R>(eventName, Event);
        public static void RemoveFuncListener<R>(string eventName, Func<R> Event)
        => EventManager.Instance.RemoveFuncListener<R>(eventName, Event);
        public static void RemoveFuncListener<T1,R>(string eventName, Func<T1,R> Event)
        => EventManager.Instance.RemoveFuncListener<T1,R>(eventName, Event);
        public static void RemoveFuncListener<T1,T2,R>(string eventName, Func<T1,T2,R> Event)
        => EventManager.Instance.RemoveFuncListener<T1,T2,R>(eventName, Event);
        public static void RemoveFuncListener<T1,T2,T3,R>(string eventName, Func<T1,T2,T3,R> Event)
        => EventManager.Instance.RemoveFuncListener<T1,T2,T3,R>(eventName, Event);
        public static void RemoveFuncListener<T1,T2,T3,T4,R>(string eventName, Func<T1,T2,T3,T4,R> Event)
        => EventManager.Instance.RemoveFuncListener<T1,T2,T3,T4,R>(eventName, Event);
        public static R FuncTrigger<R>(string eventName)
        => EventManager.Instance.FuncTrigger<R>(eventName);
        public static R FuncTrigger<T,R>(string eventName,T info1)
        => EventManager.Instance.FuncTrigger<T,R>(eventName,info1);
        public static R FuncTrigger<T1,T2,R>(string eventName,T1 info1,T2 info2)
        => EventManager.Instance.FuncTrigger<T1,T2,R>(eventName,info1,info2);
        public static R FuncTrigger<T1,T2,T3,R>(string eventName,T1 info1,T2 info2,T3 info3)
        => EventManager.Instance.FuncTrigger<T1,T2,T3,R>(eventName,info1,info2,info3);
        public static R FuncTrigger<T1,T2,T3,T4,R>(string eventName,T1 info1,T2 info2,T3 info3,T4 info4)
        => EventManager.Instance.FuncTrigger<T1,T2,T3,T4,R>(eventName,info1,info2,info3,info4);
        #endregion
        #region params
        /// <summary>
        /// 可变长非泛型方案，有装拆箱，不建议使用
        /// </summary>
        public static void AddActionListener_VLArgs(string eventName, UnityAction<object[]> Event)
        => EventManager.Instance.AddActionListener_VLArgs(eventName, Event);
        /// <summary>
        /// 可变长非泛型方案，有装拆箱，不建议使用
        /// </summary>
        public static void AddFuncListener_VLArgs(string eventName, Func<object[],object> Event)
        => EventManager.Instance.AddFuncListener_VLArgs(eventName, Event);
        /// <summary>
        /// 可变长非泛型方案，有装拆箱，不建议使用
        /// </summary>
        public static void RemoveActionListener_VLArgs(string eventName, UnityAction<object[]> Event)
        => EventManager.Instance.RemoveActionListener_VLArgs(eventName, Event);
        /// <summary>
        /// 可变长非泛型方案，有装拆箱，不建议使用
        /// </summary>
        public static void RemoveFuncListener_VLArgs(string eventName, Func<object[],object> Event)
        => EventManager.Instance.RemoveFuncListener_VLArgs(eventName, Event);
        /// <summary>
        /// 可变长非泛型方案，有装拆箱，不建议使用
        /// </summary>
        public static void FuncTrigger_VLArgs(string eventName, params object[] args)
        => EventManager.Instance.FuncTrigger_VLArgs(eventName, args);
        /// <summary>
        /// 可变长非泛型方案，有装拆箱，不建议使用
        /// </summary>
        public static void ActionTrigger_VLArgs(string eventName, params object[] args)
        => EventManager.Instance.ActionTrigger_VLArgs(eventName, args);
        #endregion
    }
}