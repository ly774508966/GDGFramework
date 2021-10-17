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
