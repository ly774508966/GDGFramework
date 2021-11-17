using System;
using System.Collections;
using System.Collections.Generic;
using GDG.ModuleManager;
using UnityEngine;

namespace GDG.Utils
{
    public abstract class MessageObserver
    {
        internal bool isAutoRemoveAfterCallback = false;
        internal IMessageSource messageSource;
        /// <summary>
        /// 观察者被广播后执行的回调
        /// </summary>
        public abstract void Callback();
    }
    public class Observer : MessageObserver
    {
        private Action action;
        private Observer() { }
        public Observer(Action action)
        {
            this.action = action;
        }
        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="messageSource">消息源</param>
        public void CancelListening(MessageSource messageSource)
        {
            MessageManager.Instance.RemoveMessageObserver(messageSource, this);
        }
        public override void Callback()
        {
            action();
        }
    }
}
