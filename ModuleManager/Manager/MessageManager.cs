using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.ModuleManager
{
    public abstract class MessageObserver
    {
        internal bool isAutoRemoveAfterCallback = false;
        internal string messageName;
        internal IMessageSource messageSource;
        public abstract void Callback();
    }

    public interface IMessageSource
    {
        void Call();
    }
    public class Observer : MessageObserver
    {
        private Action action;
        private Observer() { }
        public Observer(Action action)
        {
            this.action = action;
        }
        public void CancelListening(MessageSource messageSource)
        {
            MessageManager.Instance.RemoveMessageObserver(messageSource, this);
        }
        public void CancelListening(string messageName)
        {
            MessageManager.Instance.RemoveMessageObserver(messageName, this);
        }
        public override void Callback()
        {
            action();
        }
    }

    public class MessageSource : IMessageSource
    {
        private Action action;
        private MessageSource(Action action)
        {
            this.action = action;
        }
        public void Call()
        {
            action?.Invoke();
        }
    }
    public class MessageManager : AbsLazySingleton<MessageManager>
    {
        private  Dictionary<string, List<MessageObserver>> m_MessageName2observerMapping = new Dictionary<string, List<MessageObserver>>();
        private readonly Dictionary<IMessageSource, List<MessageObserver>> m_MessageSource2observerMapping = new Dictionary<IMessageSource, List<MessageObserver>>();
        public void AddMessageObserver(string messageName, MessageObserver observer,bool isAutoRemoveAfterCallback = false)
        {
            if (m_MessageName2observerMapping.TryGetValue(messageName, out List<MessageObserver> observerList))
            {
                observerList.Add(observer);
                observer.messageName = messageName;
                observer.isAutoRemoveAfterCallback = true;
            }
            else
            {
                m_MessageName2observerMapping.Add(messageName, new List<MessageObserver>() { observer });
                observer.messageName = messageName;
                observer.isAutoRemoveAfterCallback = true;
            }
        }
        public void AddMessageObserver(IMessageSource source, MessageObserver observer,bool isAutoRemoveAfterCallback = false)
        {
            if (m_MessageSource2observerMapping.TryGetValue(source, out List<MessageObserver> observerList))
            {
                observerList.Add(observer);
                observer.messageSource = source;
                observer.isAutoRemoveAfterCallback = true;
            }
            else
            {
                m_MessageSource2observerMapping.Add(source, new List<MessageObserver>() { observer });
                observer.messageSource = source;
                observer.isAutoRemoveAfterCallback = true;
            }
        }
        public void RemoveMessageObserver(string messageName, MessageObserver observer)
        {
            if (m_MessageName2observerMapping.TryGetValue(messageName, out List<MessageObserver> observerList))
            {
                observerList.Remove(observer);
            }
            else
            {
                Utils.Log.Error($"There is no message named {messageName} here");
            }
        }
        public void RemoveMessageObserver(IMessageSource source, MessageObserver observer)
        {
            if (m_MessageSource2observerMapping.TryGetValue(source, out List<MessageObserver> observerList))
            {
                observerList.Remove(observer);
            }
            else
            {
                Utils.Log.Error($"There is no MessageSource here");
            }
        }
        public void Broadcast(string messageName)
        {
            if (m_MessageName2observerMapping.TryGetValue(messageName, out List<MessageObserver> observerList))
            {
                for (int i = observerList.Count - 1; i >= 0;i--)
                {
                    observerList[i].Callback();
                    if(observerList[i].isAutoRemoveAfterCallback)
                    {
                        observerList.RemoveAt(i);
                    }
                }
            }
        }
        public void Broadcast(IMessageSource messageSource)
        {
            if (m_MessageSource2observerMapping.TryGetValue(messageSource, out List<MessageObserver> observerList))
            {
                messageSource.Call();

                for (int i = observerList.Count - 1; i >= 0;i--)
                {
                    observerList[i].Callback();
                    if(observerList[i].isAutoRemoveAfterCallback)
                    {
                        observerList.RemoveAt(i);
                    }
                }
            }
        }
    }
}