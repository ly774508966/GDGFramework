using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using GDG.Utils;
using System;

namespace GDG.ModuleManager
{
    public class PanelManager : LazySingleton<PanelManager>
    {
        private Dictionary<Type, IPanel> panelDic = new Dictionary<Type, IPanel>();
        private Stack<IPanel> panelStack = new Stack<IPanel>();
        public IPanel TopPanel{ get ; private set; }
        /// <summary>
        /// 注册Panel
        /// </summary>
        public void RegisterPanel(IPanel panel)
        {
            panelDic.Add(panel.GetType(), panel);
        }
        /// <summary>
        /// 注销Panel
        /// </summary>
        public bool LogoutPanel<T>()where T:IPanel
        {
            if (panelDic.ContainsKey(typeof(T)))
            {
                return panelDic.Remove(typeof(T));
            }
            else
            {
                Log.Error($"LogoutPanel Failed ! This type of Panel has never been registered ! Type :{typeof(T)}");
            }
            return false;
        }
        /// <summary>
        /// 移除Panel
        /// </summary>
        /// <param name="panel"></param>
        public void DestoryPanel(IPanel panel)
        {
            panelDic.Remove(panel.GetType());
            panel.OnDestory();
        }
        /// <summary>
        /// 显示Panel
        /// </summary>
        public void ShowPanel<T>()where T:IPanel
        {
            if (panelDic.TryGetValue(typeof(T),out IPanel panel))
            {
                panel.OnShow();
            }
            else
            {
                Log.Error($"ShowPanel Failed ! This type of Panel has never been registered ! Type :{typeof(T)}");
            }
        }
        /// <summary>
        /// 隐藏Panel
        /// </summary>
        public void HidePanel<T>()where T:IPanel
        {
            if (panelDic.TryGetValue(typeof(T),out IPanel panel))
            {
                panel.OnHide();
            }
            else
            {
                Log.Error($"HidePanel Failed ! This type of Panel has never been registered ! Type :{typeof(T)}");
            }
        }
        /// <summary>
        /// 清空所有Panel
        /// </summary>
        public void ClearPanel()
        {
            foreach(var item in panelStack)
            {
                if(item != null)
                    item.OnDestory();
            }
            panelStack.Clear();
            foreach(var item in panelDic.Values)
            {
                if(item != null)
                    item.OnDestory();
            }
            panelDic.Clear();
        }
        /// <summary>
        /// 启用Panel，并出栈
        /// </summary>
        public IPanel PopPanel()
        {
            var panel =  panelStack.Pop();
            panel?.OnResume();
            TopPanel = panel;
            return panel;
        }
        /// <summary>
        /// 使panel，并入栈
        /// </summary>
        public void PushPanel(IPanel panel)
        {
            if(panel==null)
                return;
            panelStack.Push(panel);
            panel.OnPause();
        }
    }
}
