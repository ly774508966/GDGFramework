using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using System.Threading.Tasks;
using System;
using GDG.Utils;

namespace GDG.Async
{
    public class AsyncRunner
    {
        public delegate void SyncToMainThreadEventHandle();
        public static int TaskCount;
        static object locker = new object();
        static event SyncToMainThreadEventHandle mainThreadEvent;
        static event SyncToMainThreadEventHandle current_MainThreadEvent;
        /// <summary>
        /// 开启一个异步执行
        /// </summary>
        /// <param name="action">执行回调</param>
        /// <param name="errorAction">异常回调</param>
        /// <returns></returns>
        public static Task RunAsync(Action action, Action errorAction = null)
        {
            var task = Task.Run(() =>
            {
                Interlocked.Increment(ref TaskCount);
                try
                {
                    action();
                }
                finally
                {
                    Interlocked.Decrement(ref TaskCount);
                }
            });
            task.ContinueWith(t =>
            {
                SyncToMainThread(() =>
                {
                    foreach (var item in t.Exception.InnerExceptions)
                    {
                        Log.Error(item.GetType() + "：" + item.Message);
                    }
                    errorAction?.Invoke();
                });
            }, TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }
        /// <summary>
        /// 开启一个异步执行
        /// </summary>
        /// <param name="action">执行回调</param>
        /// <param name="cancellationTokenSource">取消令牌</param>
        /// <param name="cancelledAction">取消回调</param>
        /// <param name="errorAction">异常回调</param>
        /// <returns></returns>
        public static Task RunAsync(Action action, CancellationTokenSource cancellationTokenSource, Action cancelledAction, Action errorAction = null)
        {
            var task = Task.Run(() =>
            {
                Interlocked.Increment(ref TaskCount);
                try
                {
                    action();
                }
                finally
                {
                    Interlocked.Decrement(ref TaskCount);
                }
            }, cancellationTokenSource.Token);

            task.ContinueWith(t =>
            {
                SyncToMainThread(() =>
                {
                    cancelledAction?.Invoke();
                });
            }, TaskContinuationOptions.OnlyOnCanceled);


            task.ContinueWith(t =>
            {
                SyncToMainThread(() =>
                {
                    foreach (var item in t.Exception.InnerExceptions)
                    {
                        Log.Error(item.GetType() + "：" + item.Message);
                    }
                    errorAction?.Invoke();
                });
                cancellationTokenSource.Dispose();
            }, TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }
        /// <summary>
        /// 将回调函数与Unity主线程通信
        /// </summary>
        public static void SyncToMainThread(SyncToMainThreadEventHandle action)
        {
            lock (locker)
            {
                mainThreadEvent += action;
            }
        }
        internal static void OnUpdate()
        {
            if (mainThreadEvent != null)
            {
                lock (locker)
                {
                    current_MainThreadEvent = (SyncToMainThreadEventHandle)mainThreadEvent.Clone();
                    mainThreadEvent = null;
                }
                current_MainThreadEvent.Invoke();
            }
        }
    }
}