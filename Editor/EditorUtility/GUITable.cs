using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using GDG.Utils;
using UnityEditor;
using UnityEngine;
namespace GDG.Editor
{
    public enum GUIElementType
    {
        Button,
        Toggle,
        TextField,
        Label,
        LabelButton,
    }
    public class GUITable
    {
        private (string, GUIElementType, int)[] elements;

        /// <summary>
        /// 将每一列的属性以元组的方式传入传入
        /// </summary>
        /// <param name="elements">元组参数分别为：该列名称，该列GUI组件类型，该列最大宽度</param>
        /// <returns></returns>
        public GUITable(params (string, GUIElementType, int)[] elements)
        {
            this.elements = elements;
        }
        private void GetElementReturn<T>(GUIElementType elementType, ref T value, int maxWidth)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));

            switch (elementType)
            {
                case GUIElementType.Toggle:
                    BooleanConverter booleanConverter = new BooleanConverter();
                    bool result1 = true;
                    if(value is bool temp1)
                        result1 = EditorGUILayout.Toggle(temp1, WindowStyles.MinWidth30, GUILayout.MaxWidth(maxWidth));

                    if(result1 is T result)
                    {
                        value = result;
                    }
                    break;
                case GUIElementType.TextField:
                    var temp2 = (string)converter.ConvertTo(value, typeof(string));
                    var result2 = EditorGUILayout.TextField(temp2, EditorStyles.label, WindowStyles.MinWidth30, GUILayout.MaxWidth(maxWidth));
                    value = (T)converter.ConvertTo(result2, typeof(T));
                    break;
                case GUIElementType.Button:
                    if (value is Tuple<Action,string> callback)
                    {
                        if (GUILayout.Button(callback.Item2))
                        {
                            callback.Item1();
                        }
                    }
                    break;
                case GUIElementType.LabelButton:
                    if (value is Tuple<Action,string> callback2)
                    {
                        if (GUILayout.Button(callback2.Item2,EditorStyles.boldLabel))
                        {
                            callback2.Item1();
                        }
                    }
                    break;
                case GUIElementType.Label:
                    var temp4 = (string)converter.ConvertTo(value, typeof(string));
                    EditorGUILayout.LabelField(temp4, EditorStyles.boldLabel, WindowStyles.MinWidth30, GUILayout.MaxWidth(maxWidth));
                    break;
            }
        }
        public void DrawTitle()
        {
            using (new EditorGUILayout.HorizontalScope(WindowStyles.SectionGray))
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    if (i == elements.Length - 1)
                        GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(elements[i].Item1, EditorStyles.boldLabel, WindowStyles.MinWidth30, GUILayout.MaxWidth(elements[i].Item3));
                }
            }
        }
  
        #region DrawRow
        /// <summary>
        /// 画一行数据<para/>
        /// 必须按照构造函数的参数顺序传参<para/>
        /// 参数仅支持：<para/>
        /// string                ———— TextField的返回值，或者Label的text<para/>
        /// bool                  ———— Toogle的返回值<para/>
        /// Tuple<Action,string>  ———— Button的监听事件，以及按钮名<para/>
        /// </summary>
        public void DrawRow<T1, T2>(ref T1 value1, ref T2 value2)
        {
            using (new EditorGUILayout.HorizontalScope(WindowStyles.SectionLight))
            {
                GetElementReturn<T1>(elements[0].Item2, ref value1, elements[0].Item3);
                
                GUILayout.FlexibleSpace();
                GetElementReturn<T2>(elements[1].Item2, ref value2, elements[1].Item3);
                GUILayout.Space(elements[1].Item3 / 2);
            }
        }
        /// <summary>
        /// 画一行数据<para/>
        /// 必须按照构造函数的参数顺序传参<para/> 
        /// 参数仅支持：<para/>
        /// string                ———— TextField的返回值，或者Label的text<para/>
        /// bool                  ———— Toogle的返回值<para/>
        /// Tuple<Action,string>  ———— Button的监听事件，以及按钮名<para/>
        /// </summary>
        public void DrawRow<T1, T2, T3>(ref T1 value1, ref T2 value2, ref T3 value3)
        {
            using (new EditorGUILayout.HorizontalScope(WindowStyles.SectionLight))
            {
                GetElementReturn<T1>(elements[0].Item2, ref value1, elements[0].Item3);
                GetElementReturn<T2>(elements[1].Item2, ref value2, elements[1].Item3);

                GUILayout.FlexibleSpace();
                GetElementReturn<T3>(elements[2].Item2, ref value3, elements[2].Item3);
                GUILayout.Space(elements[2].Item3 / 2);
            }
        }
        /// <summary>
        /// 画一行数据<para/>
        /// 必须按照构造函数的参数顺序传参<para/>
        /// 参数仅支持：<para/>
        /// string                ———— TextField的返回值，或者Label的text<para/>
        /// bool                  ———— Toogle的返回值<para/>
        /// Tuple<Action,string>  ———— Button的监听事件，以及按钮名<para/>
        /// </summary>
        public void DrawRow<T1, T2, T3, T4>(ref T1 value1, ref T2 value2, ref T3 value3, ref T4 value4)
        {
            using (new EditorGUILayout.HorizontalScope(WindowStyles.SectionLight))
            {
                GetElementReturn<T1>(elements[0].Item2, ref value1, elements[0].Item3);
                GetElementReturn<T2>(elements[1].Item2, ref value2, elements[1].Item3);
                GetElementReturn<T3>(elements[2].Item2, ref value3, elements[2].Item3);
                
                GUILayout.FlexibleSpace();
                GetElementReturn<T4>(elements[3].Item2, ref value4, elements[3].Item3);
                GUILayout.Space(elements[3].Item3 / 2);
            }
        }
        /// <summary>
        /// 画一行数据<para/> 
        /// 必须按照构造函数的参数顺序传参<para/>
        /// 参数仅支持：<para/>
        /// string                ———— TextField的返回值，或者Label的text<para/>
        /// bool                  ———— Toogle的返回值<para/>
        /// Tuple<Action,string>  ———— Button的监听事件，以及按钮名<para/>
        /// </summary>
        public void DrawRow<T1, T2, T3, T4, T5>(ref T1 value1, ref T2 value2, ref T3 value3, ref T4 value4, ref T5 value5)
        {
            using (new EditorGUILayout.HorizontalScope(WindowStyles.SectionLight))
            {
                GetElementReturn<T1>(elements[0].Item2, ref value1, elements[0].Item3);
                GetElementReturn<T2>(elements[1].Item2, ref value2, elements[1].Item3);
                GetElementReturn<T3>(elements[2].Item2, ref value3, elements[2].Item3);
                GetElementReturn<T4>(elements[3].Item2, ref value4, elements[3].Item3);
                
                GUILayout.FlexibleSpace();                
                GetElementReturn<T5>(elements[4].Item2, ref value5, elements[4].Item3);
                GUILayout.Space(elements[4].Item3 / 2);
            }
        }
        #endregion
    }
    public class WindowStyles
    {
        public static readonly GUIStyle SectionDark = CreateSolidColorStyle(new Color(0.1f, 0.1f, 0.1f));
        public static readonly GUIStyle SectionGray = CreateSolidColorStyle(new Color(0.15f, 0.15f, 0.15f));
        public static readonly GUIStyle SectionLight = CreateSolidColorStyle(new Color(0.2f, 0.2f, 0.2f));
        public static readonly GUIStyle None = CreateSolidColorStyle(new Color(0f, 0f, 0f, 0f));
        public static GUIStyle ErrorMessageStyle = new GUIStyle(GUI.skin.GetStyle("CN StatusError"))
        {
            alignment = TextAnchor.MiddleRight
        };

        public static GUIStyle CreateSolidColorStyle(Color color)
        {
            Texture2D texture = new Texture2D(2, 2);
            for (int y = 0; y < texture.height; ++y)
            {
                for (int x = 0; x < texture.width; ++x)
                {
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();

            return new GUIStyle { normal = { background = texture } };
        }

        public static readonly GUILayoutOption MaxWidth25 = GUILayout.MaxWidth(25.0f);
        public static readonly GUILayoutOption MaxWidth30 = GUILayout.MaxWidth(30.0f);
        public static readonly GUILayoutOption MaxWidth100 = GUILayout.MaxWidth(100.0f);
        public static readonly GUILayoutOption MaxWidth400 = GUILayout.MaxWidth(400.0f);
        public static readonly GUILayoutOption MaxWidth300 = GUILayout.MaxWidth(300.0f);
        public static readonly GUILayoutOption MaxWidth200 = GUILayout.MaxWidth(200.0f);

        public static readonly GUILayoutOption MinWidth30 = GUILayout.MinWidth(30.0f);
        public static readonly GUILayoutOption MinWidth100 = GUILayout.MinWidth(100.0f);
        public static readonly GUILayoutOption MinWidth150 = GUILayout.MinWidth(150.0f);
    }
}