using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.Base;
using GDG.Utils;
using System;
using GDG.UI;
using GDG.ModuleManager;

namespace GDG.UI
{
    public class PanelManager : LazySingleton<PanelManager>
    {
        private Dictionary<Type, List<IPanel>> panelDic = new Dictionary<Type, List<IPanel>>();
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
        public BasePanel LoadAndRegisterPanel(string panelPath)
        {
            var obj = AssetPool.Instance.Pop<GameObject>(panelPath);
            obj.transform.SetParent(Canvas.transform);
            if(obj.TryGetComponent<BasePanel>(out BasePanel panel))
            {
                RegisterPanel(panel);
                return panel;
            }
            else
            {
                Log.Error("This GameObject does't exist any BasePanel component");
                return null;
            }
        }
        public BasePanel LoadAndRegisterPanel(string bundlename, string assetname, string path = null, string mainABName = null) 
        {
            var obj = AssetPool.Instance.Pop<GameObject>(bundlename,assetname,path,mainABName);
            obj.transform.SetParent(Canvas.transform);
            if(obj.TryGetComponent<BasePanel>(out BasePanel panel))
            {
                RegisterPanel(panel);
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
            if(panelDic.TryGetValue(panel.GetType(),out List<IPanel> panels))
            {
                panels.Add(panel);
            }
            else
                panelDic.Add(panel.GetType(),new List<IPanel>(){panel});
        }
        /// <summary>
        /// 注销Panel
        /// </summary>
        public bool LogoutPanels<T>() where T:BasePanel
        {
            if (panelDic.ContainsKey(typeof(T)))
            {
                return panelDic.Remove(typeof(T));
            }
            else
            {
                Log.Error($"LogoutPanel Failed ! This Panel has never been registered ! Panel :{typeof(T)}");
            }
            return false;
        }
        public bool LogoutPanel(BasePanel panel)
        {
            if (panelDic.TryGetValue(panel.GetType(),out List<IPanel> panels) )
            {
                bool isSucess = panels.Remove(panel);
                if(panels.Count == 0)
                {
                    panelDic.Remove(panel.GetType());
                }
                return isSucess;
            }
            else
            {
                Log.Error($"LogoutPanel Failed ! This Panel has never been registered ! Panel :{panel.GetType()}");
            }
            return false;
        }
        /// <summary>
        /// 移除Panel
        /// </summary>
        /// <param name="panel"></param>
        public void DestoryPanel(BasePanel panel)
        {
            if (panelDic.TryGetValue(panel.GetType(),out List<IPanel> panels) )
            {
                panels.Remove(panel);
            }
            panel.OnDestory();
        }
        public void DestoryPanels<T>()where T:BasePanel
        {
            if (panelDic.TryGetValue(typeof(T),out List<IPanel> panels) )
            {
                foreach (var item in panels)
                {
                    item.OnDestory();
                }
            }
            panelDic.Remove(typeof(T));
        }
        /// <summary>
        /// 显示Panel
        /// </summary>
        public void ShowPanels<T>() where T:BasePanel
        {
            if (panelDic.TryGetValue(typeof(T),out List<IPanel> panels))
            {
                foreach(var panel in panels)
                    panel.OnShow();
            }
            else
            {
                Log.Error($"ShowPanels Failed ! This Panel has never been registered ! Panel :{typeof(T)}");
            }
        }
        /// <summary>
        /// 隐藏Panel
        /// </summary>
        public void HidePanels<T>() where T:BasePanel
        {
            if (panelDic.TryGetValue(typeof(T),out List<IPanel> panels))
            {
                foreach(var panel in panels)
                    panel.OnHide();
            }
            else
            {
                Log.Error($"HidePanels Failed ! This Panel has never been registered ! Panel :{typeof(T)}");
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
                foreach(var panel in item)
                    panel.OnDestory();
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
