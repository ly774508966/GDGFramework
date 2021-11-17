using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.Utils
{
    public interface IMessageSource
    {
        /// <summary>
        /// 广播时消息源的回调
        /// </summary>
        void Call();
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
}