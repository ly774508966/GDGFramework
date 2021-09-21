using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace GDG.Editor
{
    public class GUINavigationBar
    {
        private GUIStyle itemStyle;
        private GUIStyle selectionStyle;
        private GUIStyle backgroundStyle;
        private string[] ContentList;
        private bool[] selectionList;
        private int currentSelect = 0;
        private void StyleInit()
        {
            itemStyle = new GUIStyle();
            itemStyle.fontSize = 12;
            itemStyle.fontStyle = FontStyle.Bold;
            itemStyle.alignment = TextAnchor.MiddleCenter;
            itemStyle.normal.textColor = new Color(144 / 256f, 144 / 256f, 144 / 256f);
            itemStyle.hover.textColor = Color.white;
            itemStyle.active.textColor = Color.white;

            selectionStyle = new GUIStyle();
            selectionStyle.fontSize = 12;
            selectionStyle.fontStyle = FontStyle.Bold;
            selectionStyle.alignment = TextAnchor.MiddleCenter;
            selectionStyle.normal.textColor = Color.white;
            selectionStyle.hover.textColor = Color.white;
            selectionStyle.active.textColor = Color.white;
            selectionStyle.normal.background = GUI.skin.customStyles[280].normal.background;

            backgroundStyle = new GUIStyle();
            backgroundStyle.normal.background = GUI.skin.customStyles[248].normal.background;
        }
        public GUINavigationBar(string[] ContentList) => this.ContentList = ContentList;
        public GUINavigationBar(string[] ContentList, GUIStyle itemStyle, GUIStyle backgroundStyle ,GUIStyle selectionStyle)
        {
            this.ContentList = ContentList;
            this.itemStyle = itemStyle;
            this.backgroundStyle = backgroundStyle;
            this.selectionStyle = selectionStyle;
        }
        public int GetSelectionIndexOnGUI(float width, float height, float itemHeight = 40f)
        {
            if (itemStyle == null || backgroundStyle == null || selectionStyle == null)
                StyleInit();

            using (new GUILayout.VerticalScope(backgroundStyle, GUILayout.Width(width), GUILayout.Height(height)))
            {
                for (int i = 0; i < ContentList.Length; i++)
                {
                    var currentStyle = itemStyle;
                    if (currentSelect == i)
                    {
                        currentStyle = selectionStyle;
                    }
                    if (GUILayout.Button(ContentList[i], currentStyle, GUILayout.Width(width), GUILayout.Height(itemHeight)))
                    {
                        currentSelect = i;
                    }
                }
            }
            return currentSelect;
        }
    }
}