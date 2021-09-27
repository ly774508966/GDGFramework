using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using UnityEditor;

namespace GDG.Editor
{
    public class GDGEditorGUI
    {
        public static GUIStyle DisabledLabelStyle{ get => new GUIStyle("PR DisabledLabel"); }
        public static GUIStyle LargeLabelStyle{ get => new GUIStyle("AM ChannelStripHeaderStyle"); }

        public static void DisactivedLabel(string text)
        {
            GUILayout.Label(text,DisabledLabelStyle);
        }
        public static void ActivedLabel(string text)
        {
            GUILayout.Label(text);
        }
        

    }
}