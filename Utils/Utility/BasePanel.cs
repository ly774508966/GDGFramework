using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Text.RegularExpressions;
using GDG.Utils;

namespace GDG.Utils
{
    public interface IPanel
    {
        void OnShow();
        void OnHide();
        void OnDestory();
        void OnPause();
        void OnResume();
        List<T> GetControl<T>() where T : UIBehaviour;
        T GetControl<T>(string controlname) where T : UIBehaviour;
        List<T> GetControls<T>(string controlname, int count) where T : UIBehaviour;
        void LogAllControlName();
    }

    /// <summary>
    /// 面板基类，将会生成一个装有该面板下所有子控件UIBehaviour的字典,可以通过GetUI的方法获取当前面板下某个子控件；<para />
    /// 可以通过覆写 OnButtonDown、OnDropDownValueChanged等方法实现，并使用switch(controlname)对某个子控件的事件进行监听；<para />
    /// 提供了面板加载、销毁、显示、隐藏时的回调函数；<para />
    /// </summary>
    public abstract class BasePanel : MonoBehaviour, IPanel
    {
        private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();
        private CanvasGroup canvasGroup;
        protected virtual void Awake()
        {
            canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
            AddControl<GridLayoutGroup>();
            AddControl<VerticalLayoutGroup>();
            AddControl<HorizontalLayoutGroup>();
            AddControl<TMP_Text>();
            AddControl<TMP_Dropdown>();
            AddControl<TMP_InputField>();
            AddControl<Button>();//onclick
            AddControl<Image>();
            AddControl<RawImage>();
            AddControl<Dropdown>();//onValueChanged
            AddControl<Text>();
            AddControl<Toggle>();//onValueChanged
            AddControl<Slider>();//onValueChanged
            AddControl<ScrollRect>();//onValueChanged 
            AddControl<Scrollbar>();//onValueChanged
            AddControl<InputField>();//onSubmit,onValueChanged,onValueChange,onEndEdit
        }
        //遍历面板下的控件，并加入到字典中
        private void AddControl<T>() where T : UIBehaviour
        {
            //获得 所有该面板下该类型的UIBehaviour
            T[] controls = this.GetComponentsInChildren<T>();

            //全部加入到字典中
            foreach (var control in controls)
            {
                string name = control.gameObject.name;
                //若 控件没有匹配 则 生成键值
                if (!controlDic.ContainsKey(name))
                {
                    controlDic.Add(name, new List<UIBehaviour>());
                }
                //加入到字典中
                controlDic[name].Add(control);

                //添加监听事件
                if (control is Button button)
                    button.onClick.AddListener(() => { OnButtonDown(name); });
                else if (control is Dropdown dropdown)
                    dropdown.onValueChanged.AddListener((value) => { OnDropDownValueChanged(name, value); });
                else if (control is Toggle toggle)
                    toggle.onValueChanged.AddListener((ischanged) => { OnToggleValueChanged(name, ischanged); });
                else if (control is Slider slider)
                    slider.onValueChanged.AddListener((value) => { OnSliderValueChanged(name, value); });
                else if (control is ScrollRect scrollRect)
                    scrollRect.onValueChanged.AddListener((value) => { OnScrollRectValueChanged(name, value); });
                else if (control is Scrollbar scrollbar)
                    scrollbar.onValueChanged.AddListener((value) => { OnScrollbarValueChanged(name, value); });
                else if (control is InputField inputField)
                {
                    //inputField.onSumbit;.AddListener((value) => { OnInputFieldSumbit(name,value); });
                    inputField.onEndEdit.AddListener((value) => { OnInputFieldEndEdit(name, value); });
                    inputField.onValueChanged.AddListener((value) => { OnInputFieldValueChanged(name, value); });
                }
            }
        }
        /// <summary>
        /// 从子控件（装有UI的GameObject）中获取UIBehaviour
        /// </summary>
        /// <param name="controlname">子控件名字</param>
        /// <typeparam name="T">UI的类型</typeparam>
        public T GetControl<T>(string controlname) where T : UIBehaviour
        {
            if (controlDic.ContainsKey(controlname))
            {
                foreach (var ui in controlDic[controlname])
                {
                    if (ui is T)
                        return ui as T;
                }
            }
            Log.Warning($"Can't find control：'{controlname}'！");
            return null;
        }
        public List<T> GetControl<T>() where T : UIBehaviour
        {
            var list = new List<T>();
            foreach (var uilist in controlDic.Values)
            {
                foreach (var ui in uilist)
                {
                    if (ui is T)
                    {
                        list.Add(ui as T);
                    }
                }
            }
            return list;
        }
        public List<T> GetControls<T>(string controlname, int count) where T : UIBehaviour
        {
            var list = new List<T>();
            int i = 0;
            foreach (var uilist in controlDic.Values)
            {
                foreach (var ui in uilist)
                {
                    if (ui is T && Regex.Replace(ui.gameObject.name, @" \(.*\)", "") == controlname)
                    {
                        list.Add(ui as T);
                        ++i;
                        if (i == count)
                            return list;
                    }
                }
            }
            return list;
        }
        public virtual void OnShow() {this.gameObject.SetActive(true); }
        public virtual void OnHide() {this.gameObject.SetActive(false); }
        public virtual void OnPause() {canvasGroup.blocksRaycasts = false; }
        public virtual void OnResume() {canvasGroup.blocksRaycasts = true; }
        public virtual void OnDestory() {controlDic.Clear(); Destroy(this.gameObject); }
        

        /// <summary>
        /// Button按下后时的监听,通过switch来匹配控件名
        /// </summary>
        protected virtual void OnButtonDown(string controlname) { }
        /// <summary>
        /// DropDown改变后的监听,通过switch来匹配控件名
        /// </summary>
        protected virtual void OnDropDownValueChanged(string controlname, int value) { }
        /// <summary>
        /// Toggle改变后的监听,通过switch来匹配控件名
        /// </summary>
        protected virtual void OnToggleValueChanged(string controlname, bool ischanged) { }
        /// <summary>
        /// Slider改变后的监听,通过switch来匹配控件名
        /// </summary>
        protected virtual void OnSliderValueChanged(string controlname, float value) { }
        /// <summary>
        /// ScrollRect改变后的监听,通过switch来匹配控件名
        /// </summary>
        protected virtual void OnScrollRectValueChanged(string controlname, Vector2 value) { }
        /// <summary>
        /// Scrollbar改变后的监听,通过switch来匹配控件名
        /// </summary>
        protected virtual void OnScrollbarValueChanged(string controlname, float value) { }
        // /// <summary>
        // /// InputField提交后的监听,通过switch来匹配控件名
        // /// </summary>
        // protected virtual void OnInputFieldSumbit(string controlname,string value) { }
        /// <summary>
        /// InputField结束编辑后的监听,通过switch来匹配控件名
        /// </summary>
        protected virtual void OnInputFieldEndEdit(string controlname, string value) { }
        /// <summary>
        /// InputField值改变后的监听,通过switch来匹配控件名
        /// </summary>
        protected virtual void OnInputFieldValueChanged(string controlname, string value) { }

        public void LogAllControlName()
        {
            foreach (var item in controlDic)
            {
                Log.Info(item.Key);
            }
        }
    }
}