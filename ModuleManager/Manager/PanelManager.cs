using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using GDG.Utils;
using System;
using GDG.UI;

namespace GDG.ModuleManager
{
    public class PanelManager : LazySingleton<PanelManager>
    {
        private Dictionary<string, IPanel> panelDic = new Dictionary<string, IPanel>();
        private Stack<IPanel> panelStack = new Stack<IPanel>();
        public IPanel TopPanel{ get ; private set; }
        public Canvas Canvas;
        public PanelManager()
        {
            var canvasComponent = GameObject.FindObjectOfType<Canvas>();
            if(canvasComponent !=null)
                Canvas = canvasComponent;
            else
            {
                Log.Warning("Can't find Canvas in the scene !");
            }
        }
        public BasePanel LoadAndRegisterPanelFromResources(string panelPath)
        {
            var obj = GDGTools.ResourceLoder.LoadResource<GameObject>(panelPath);
            obj.transform.SetParent(Canvas.transform);
            if(obj.TryGetComponent<BasePanel>(out BasePanel panel))
            {
                RegisterPanel(obj.name,panel);
                return panel;
            }
            else
            {
                Log.Error("This GameObject does't exist any BasePanel component");
                return null;
            }
        }
        public BasePanel LoadAndRegisterPanelFromAssetBundle(string bundlename, string assetname, string path = null, string mainABName = null) 
        {
            var obj = GDGTools.AssetLoder.LoadAsset<GameObject>(bundlename,assetname,path,mainABName);
            obj.transform.SetParent(Canvas.transform);
            if(obj.TryGetComponent<BasePanel>(out BasePanel panel))
            {
                RegisterPanel(obj.name,panel);
                return panel;
            }
            else
            {
                Log.Error("This GameObject does't exist any BasePanel component");
                return null;
            }
        }
        /// <summary>
        /// 注册Panel
        /// </summary>
        public void RegisterPanel(BasePanel panel)
        {
            panel.gameObject.ClearNameWithClone();
            if(Canvas!=null)
                panel.transform.SetParent(Canvas.transform);
            panelDic.Add(panel.gameObject.name,panel);
        }
        public void RegisterPanel(string panelName, BasePanel panel)
        {
            panelDic.Add(panelName,panel);
        }
        /// <summary>
        /// 注销Panel
        /// </summary>
        public bool LogoutPanel(string panelName)
        {
            if (panelDic.ContainsKey(panelName))
            {
                return panelDic.Remove(panelName);
            }
            else
            {
                Log.Error($"LogoutPanel Failed ! This Panel has never been registered ! PanelName :{panelName}");
            }
            return false;
        }
        /// <summary>
        /// 移除Panel
        /// </summary>
        /// <param name="panel"></param>
        public void DestoryPanel(BasePanel panel)
        {
            panelDic.Remove(panel.gameObject.name);
            panel.OnDestory();
        }
        public void DestoryPanel(string panelName)
        {
            panelDic[panelName].OnDestory();
            panelDic.Remove(panelName);
        }
        /// <summary>
        /// 显示Panel
        /// </summary>
        public void ShowPanel(string panelName)
        {
            if (panelDic.TryGetValue(panelName,out IPanel panel))
            {
                panel.OnShow();
            }
            else
            {
                Log.Error($"ShowPanel Failed ! This Panel has never been registered ! PanelName :{panelName}");
            }
        }
        /// <summary>
        /// 隐藏Panel
        /// </summary>
        public void HidePanel(string panelName)
        {
            if (panelDic.TryGetValue(panelName,out IPanel panel))
            {
                panel.OnHide();
            }
            else
            {
                Log.Error($"HidePanel Failed ! This Panel has never been registered ! PanelName :{panelName}");
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
        public T PopPanel<T>()where T:BasePanel
        {
            var panel =  panelStack.Pop() as T;
            panel?.OnResume();
            TopPanel = panel;
            return panel;
        }
        /// <summary>
        /// 使panel，并入栈
        /// </summary>
        public void PushPanel(BasePanel panel)
        {
            if(panel==null)
                return;
            panelStack.Push(panel);
            panel.OnPause();
        }
    }
}
