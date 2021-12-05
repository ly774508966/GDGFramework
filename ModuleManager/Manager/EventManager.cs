using System;
using System.Collections.Generic;
using System.IO;
using GDG.Utils;
using UnityEngine.Events;

namespace GDG.ModuleManager
{
    public class EventManager : LazySingleton<EventManager>
    {
        #region EventHandle
        private interface IEventHandle { }
        private class VLArgEventHandle : IEventHandle
        {
            public UnityAction<object[]> actions;
            public Func<object[], object> funcs;
            public VLArgEventHandle(UnityAction<object[]> action)
            {
                this.actions = action;
            }
            public VLArgEventHandle(Func<object[], object> func)
            {
                this.funcs = func;
            }
        }
        private class EventHandle<T1, T2, T3, T4, T5> : IEventHandle
        {
            public Func<T1, T2, T3, T4, T5> funcs;
            public EventHandle(Func<T1, T2, T3, T4, T5> func)
            {
                this.funcs = func;
            }
        }
        private class EventHandle<T1, T2, T3, T4> : IEventHandle
        {
            public UnityAction<T1, T2, T3, T4> actions;
            public Func<T1, T2, T3, T4> funcs;
            public EventHandle(UnityAction<T1, T2, T3, T4> action)
            {
                this.actions = action;
            }
            public EventHandle(Func<T1, T2, T3, T4> func)
            {
                this.funcs = func;
            }
        }
        private class EventHandle<T1, T2, T3> : IEventHandle
        {
            public UnityAction<T1, T2, T3> actions;
            public Func<T1, T2, T3> funcs;
            public EventHandle(UnityAction<T1, T2, T3> action)
            {
                this.actions = action;
            }
            public EventHandle(Func<T1, T2, T3> func)
            {
                this.funcs = func;
            }
        }
        private class EventHandle<T1, T2> : IEventHandle
        {
            public UnityAction<T1, T2> actions;
            public Func<T1, T2> funcs;
            public EventHandle(UnityAction<T1, T2> action)
            {
                this.actions = action;
            }
            public EventHandle(Func<T1, T2> func)
            {
                this.funcs = func;
            }
        }
        private class EventHandle<T> : IEventHandle
        {
            public UnityAction<T> actions;
            public Func<T> funcs;
            public EventHandle(UnityAction<T> action)
            {
                this.actions = action;
            }
            public EventHandle(Func<T> func)
            {
                this.funcs = func;
            }
        }
        private class EventHandle : IEventHandle
        {
            public UnityAction actions;
            public EventHandle(UnityAction action)
            {
                this.actions = action;
            }
        }
        # endregion

        //事件字典
        private readonly Dictionary<string, IEventHandle> EventDic = new Dictionary<string, IEventHandle>();
        public static bool EnableEventLog = false;
        #region 清空事件
        public void ClearEvent(string eventName)
        {
            if (EventDic.TryGetValue(eventName, out IEventHandle eventHandle))
            {
                if(eventHandle is EventHandle handle)
                {
                    handle.actions = null;
                    EventDic.Remove(eventName);
                }
            }
        }
        public void ClearEvent<T>(string eventName)
        {
            if (EventDic.TryGetValue(eventName, out IEventHandle eventHandle))
            {
                if(eventHandle is EventHandle<T> handle)
                {
                    handle.actions = null;
                    handle.funcs = null;
                    EventDic.Remove(eventName);
                }
            }
        }
        public void ClearEvent<T1,T2>(string eventName)
        {
            if (EventDic.TryGetValue(eventName, out IEventHandle eventHandle))
            {
                if(eventHandle is EventHandle<T1,T2> handle)
                {
                    handle.actions = null;
                    handle.funcs = null;
                    EventDic.Remove(eventName);
                }
            }
        }
        public void ClearEvent<T1,T2,T3>(string eventName)
        {
            if (EventDic.TryGetValue(eventName, out IEventHandle eventHandle))
            {
                if(eventHandle is EventHandle<T1,T2,T3> handle)
                {
                    handle.actions = null;
                    handle.funcs = null;
                    EventDic.Remove(eventName);
                }
            }
        }
        public void ClearEvent<T1,T2,T3,T4>(string eventName)
        {
            if (EventDic.TryGetValue(eventName, out IEventHandle eventHandle))
            {
                if(eventHandle is EventHandle<T1,T2,T3,T4> handle)
                {
                    handle.actions = null;
                    handle.funcs = null;
                    EventDic.Remove(eventName);
                }
            }
        }
        public void ClearEvent<T1,T2,T3,T4,T5>(string eventName)
        {
            if (EventDic.TryGetValue(eventName, out IEventHandle eventHandle))
            {
                if(eventHandle is EventHandle<T1,T2,T3,T4,T5> handle)
                {
                    handle.funcs = null;
                    EventDic.Remove(eventName);
                }
            }
        }
        public void ClearEvent_VLArg(string eventName)
        {
            if (EventDic.TryGetValue(eventName, out IEventHandle eventHandle))
            {
                if(eventHandle is VLArgEventHandle handle)
                {
                    handle.funcs = null;
                    EventDic.Remove(eventName);
                }
            }
        }
        #endregion
        #region 注册监听
        public void AddActionListener(string eventName, UnityAction Event,bool AutoRemoveAfterTrigger = false)
        {
            UnityAction callback = null;
            if(AutoRemoveAfterTrigger)
            {
                callback = () =>
                {
                    Event();
                    if(EventDic.TryGetValue(eventName,out IEventHandle handle))
                    {
                        if(handle is EventHandle eventHandle)
                        {
                            eventHandle.actions -= callback;
                        }
                    }
                };
            }

            if (!EventDic.ContainsKey(eventName))
            {
                EventDic.Add(eventName, new EventHandle(AutoRemoveAfterTrigger?callback:Event));
            }
            else
                (EventDic[eventName] as EventHandle).actions += AutoRemoveAfterTrigger?callback:Event;
        }
        public void AddActionListener<T>(string eventName, UnityAction<T> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                EventDic.Add(eventName, new EventHandle<T>(Event));
            }
            else
                (EventDic[eventName] as EventHandle<T>).actions += Event;
        }
        public void AddActionListener<T1, T2>(string eventName, UnityAction<T1, T2> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                EventDic.Add(eventName, new EventHandle<T1, T2>(Event));
            }
            else
                (EventDic[eventName] as EventHandle<T1, T2>).actions += Event;
        }
        public void AddActionListener<T1, T2, T3>(string eventName, UnityAction<T1, T2, T3> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                EventDic.Add(eventName, new EventHandle<T1, T2, T3>(Event));
            }
            else
                (EventDic[eventName] as EventHandle<T1, T2, T3>).actions += Event;
        }
        public void AddActionListener<T1, T2, T3, T4>(string eventName, UnityAction<T1, T2, T3, T4> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                EventDic.Add(eventName, new EventHandle<T1, T2, T3, T4>(Event));
            }
            else
                (EventDic[eventName] as EventHandle<T1, T2, T3, T4>).actions += Event;
        }
        public void AddFuncListener<R>(string eventName, Func<R> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                EventDic.Add(eventName, new EventHandle<R>(Event));
            }
            else
                (EventDic[eventName] as EventHandle<R>).funcs += Event;
        }
        public void AddFuncListener<T1, R>(string eventName, Func<T1, R> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                EventDic.Add(eventName, new EventHandle<T1, R>(Event));
            }
            else
                (EventDic[eventName] as EventHandle<T1, R>).funcs += Event;
        }
        public void AddFuncListener<T1, T2, R>(string eventName, Func<T1, T2, R> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                EventDic.Add(eventName, new EventHandle<T1, T2, R>(Event));
            }
            else
                (EventDic[eventName] as EventHandle<T1, T2, R>).funcs += Event;
        }
        public void AddFuncListener<T1, T2, T3, R>(string eventName, Func<T1, T2, T3, R> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                EventDic.Add(eventName, new EventHandle<T1, T2, T3, R>(Event));
            }
            else
                (EventDic[eventName] as EventHandle<T1, T2, T3, R>).funcs += Event;
        }
        public void AddFuncListener<T1, T2, T3, T4, R>(string eventName, Func<T1, T2, T3, T4, R> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                EventDic.Add(eventName, new EventHandle<T1, T2, T3, T4, R>(Event));
            }
            else
                (EventDic[eventName] as EventHandle<T1, T2, T3, T4, R>).funcs += Event;
        }

        #endregion
        #region 注销监听

        public void RemoveActionListener(string eventName, UnityAction Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
                return;
            }
            (EventDic[eventName] as EventHandle).actions -= Event;
        }
        public void RemoveActionListener<T>(string eventName, UnityAction<T> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
                return;
            }
            (EventDic[eventName] as EventHandle<T>).actions -= Event;
        }
        public void RemoveActionListener<T1, T2>(string eventName, UnityAction<T1, T2> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
                return;
            }
            (EventDic[eventName] as EventHandle<T1, T2>).actions -= Event;
        }
        public void RemoveActionListener<T1, T2, T3>(string eventName, UnityAction<T1, T2, T3> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
                return;
            }
            (EventDic[eventName] as EventHandle<T1, T2, T3>).actions -= Event;
        }
        public void RemoveActionListener<T1, T2, T3, T4>(string eventName, UnityAction<T1, T2, T3, T4> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
                return;
            }
            (EventDic[eventName] as EventHandle<T1, T2, T3, T4>).actions -= Event;
        }
        public void RemoveFuncListener<R>(string eventName, Func<R> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
                return;
            }
            (EventDic[eventName] as EventHandle<R>).funcs -= Event;
        }
        public void RemoveFuncListener<T1, R>(string eventName, Func<T1, R> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
                return;
            }
            (EventDic[eventName] as EventHandle<T1, R>).funcs -= Event;
        }
        public void RemoveFuncListener<T1, T2, R>(string eventName, Func<T1, T2, R> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
                return;
            }
            (EventDic[eventName] as EventHandle<T1, T2, R>).funcs -= Event;
        }
        public void RemoveFuncListener<T1, T2, T3, R>(string eventName, Func<T1, T2, T3, R> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
                return;
            }
            (EventDic[eventName] as EventHandle<T1, T2, T3, R>).funcs -= Event;
        }
        public void RemoveFuncListener<T1, T2, T3, T4, R>(string eventName, Func<T1, T2, T3, T4, R> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
                return;
            }
            (EventDic[eventName] as EventHandle<T1, T2, T3, T4, R>).funcs -= Event;
        }
        #endregion
        #region 事件多播
        private void ActionTriggerCallback(string eventName, Action<string> callback)
        {
            if (!EventDic.ContainsKey(eventName) || EventDic[eventName] == null)
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
            }
            else
            {
                callback(eventName);
                if (EnableEventLog)
                    Log.Warning($"Excute event :  {eventName}", "Event");
            }
        }
        public R FuncTriggerCallback<R>(string eventName, Func<R> callback)
        {
            if (!EventDic.ContainsKey(eventName) || EventDic[eventName] == null)
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
                return default(R);
            }
            if (EnableEventLog)
                Log.Warning($"Excute event :  {eventName}", "Event");

            return callback();
        }
        public void ActionTrigger(string eventName)
        => ActionTriggerCallback(eventName, (eventName) => { (EventDic[eventName] as EventHandle)?.actions?.Invoke(); });
        public void ActionTrigger<T>(string eventName, T info)
        => ActionTriggerCallback(eventName, (eventName) => { (EventDic[eventName] as EventHandle<T>)?.actions?.Invoke(info); });
        public void ActionTrigger<T1, T2>(string eventName, T1 info1, T2 info2)
        => ActionTriggerCallback(eventName, (eventName) => { (EventDic[eventName] as EventHandle<T1, T2>)?.actions?.Invoke(info1, info2); });
        public void ActionTrigger<T1, T2, T3>(string eventName, T1 info1, T2 info2, T3 info3)
        => ActionTriggerCallback(eventName, (eventName) => { (EventDic[eventName] as EventHandle<T1, T2, T3>)?.actions?.Invoke(info1, info2, info3); });
        public void ActionTrigger<T1, T2, T3, T4>(string eventName, T1 info1, T2 info2, T3 info3, T4 info4)
        => ActionTriggerCallback(eventName, (eventName) => { (EventDic[eventName] as EventHandle<T1, T2, T3, T4>)?.actions?.Invoke(info1, info2, info3, info4); });

        public R FuncTrigger<R>(string eventName)
        => FuncTriggerCallback<R>(eventName, () =>
        {
            var func = (EventDic[eventName] as EventHandle<R>).funcs;
            return func == null ? default(R) : (EventDic[eventName] as EventHandle<R>).funcs.Invoke();
        });
        public R FuncTrigger<T1, R>(string eventName, T1 info)
        => FuncTriggerCallback<R>(eventName, () =>
        {
            var func = (EventDic[eventName] as EventHandle<T1, R>).funcs;
            return func == null ? default(R) : (EventDic[eventName] as EventHandle<T1, R>).funcs.Invoke(info);
        });
        public R FuncTrigger<T1, T2, R>(string eventName, T1 info1, T2 info2)
        => FuncTriggerCallback<R>(eventName, () =>
        {
            var func = (EventDic[eventName] as EventHandle<T1, T2, R>).funcs;
            return func == null ? default(R) : (EventDic[eventName] as EventHandle<T1, T2, R>).funcs.Invoke(info1, info2);
        });
        public R FuncTrigger<T1, T2, T3, R>(string eventName, T1 info1, T2 info2, T3 info3)
        => FuncTriggerCallback<R>(eventName, () =>
        {
            var func = (EventDic[eventName] as EventHandle<T1, T2, T3, R>).funcs;
            return func == null ? default(R) : (EventDic[eventName] as EventHandle<T1, T2, T3, R>).funcs.Invoke(info1, info2, info3);
        });
        public R FuncTrigger<T1, T2, T3, T4, R>(string eventName, T1 info1, T2 info2, T3 info3, T4 info4)
        => FuncTriggerCallback<R>(eventName, () =>
        {
            var func = (EventDic[eventName] as EventHandle<T1, T2, T3, T4, R>);
            return func == null ? default(R) : (EventDic[eventName] as EventHandle<T1, T2, T3, T4, R>).funcs.Invoke(info1, info2, info3, info4);
        });

        #endregion
        #region 可变长方案
        public void AddActionListener_VLArgs(string eventName, UnityAction<object[]> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                EventDic.Add(eventName, new VLArgEventHandle(Event));
            }
            else
                (EventDic[eventName] as VLArgEventHandle).actions += Event;
        }
        public void AddFuncListener_VLArgs(string eventName, Func<object[], object> Event)
        {
            if (!EventDic.ContainsKey(eventName))
            {
                EventDic.Add(eventName, new VLArgEventHandle(Event));
            }
            else
                (EventDic[eventName] as VLArgEventHandle).funcs += Event;
        }
        public void RemoveActionListener_VLArgs(string eventName, UnityAction<object[]> Event)
        {
            if (!EventDic.ContainsKey(eventName) || EventDic[eventName] == null)
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
                return;
            }
            (EventDic[eventName] as VLArgEventHandle).actions -= Event;
        }
        public void RemoveFuncListener_VLArgs(string eventName, Func<object[], object> Event)
        {
            if (!EventDic.ContainsKey(eventName) || EventDic[eventName] == null)
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
                return;
            }
            (EventDic[eventName] as VLArgEventHandle).funcs -= Event;
        }
        public object FuncTrigger_VLArgs(string eventName, params object[] args)
        {
            if (!EventDic.ContainsKey(eventName) || EventDic[eventName] == null)
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
            }
            else
            {
                if (EnableEventLog)
                    Log.Warning($"Excute event :  {eventName}", "Event");

                return (EventDic[eventName] as VLArgEventHandle)?.funcs?.Invoke(args);
            }
            return default(object);
        }
        public void ActionTrigger_VLArgs(string eventName, params object[] args)
        {
            if (!EventDic.ContainsKey(eventName) || EventDic[eventName] == null)
            {
                if (EnableEventLog)
                    Log.Warning($"\"{eventName}\" Doesn't exist in EventDic");
            }
            else
            {
                if (EnableEventLog)
                    Log.Warning($"Excute event :  {eventName}", "Event");

                (EventDic[eventName] as VLArgEventHandle)?.actions?.Invoke(args);
            }
        }
        #endregion

        public void GenerateEventTableFile()
        {
            #if UNITY_EDITOR
            using (FileStream fs = new FileStream($"{UserFileManager.Path}/EventDictionary.txt", FileMode.Create, FileAccess.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (var dic in EventDic)
                    {
                        sw.WriteLine($"{dic.Key}");
                    }
                }
            }
            #endif
        }
    }
}