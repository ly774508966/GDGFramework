using System;
using System.Collections;
using System.Collections.Generic;
using GDG.ModuleManager;
using GDG.Utils;
using UnityEngine;
namespace GDG.ModuleManager
{

    public class MessageManager : LazySingleton<MessageManager>
    {
        private  Dictionary<string, List<MessageObserver>> m_MessageName2observerMapping = new Dictionary<string, List<MessageObserver>>();
        private readonly Dictionary<IMessageSource, List<MessageObserver>> m_MessageSource2observerMapping = new Dictionary<IMessageSource, List<MessageObserver>>();
        public void AddMessageObserver(IMessageSource source, MessageObserver observer,bool isAutoRemoveAfterCallback = false)
        {
            if (m_MessageSource2observerMapping.TryGetValue(source, out List<MessageObserver> observerList))
            {
                observerList.Add(observer);
                observer.messageSource = source;
                observer.isAutoRemoveAfterCallback = isAutoRemoveAfterCallback;
            }
            else
            {
                m_MessageSource2observerMapping.Add(source, new List<MessageObserver>() { observer });
                observer.messageSource = source;
                observer.isAutoRemoveAfterCallback = isAutoRemoveAfterCallback;
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