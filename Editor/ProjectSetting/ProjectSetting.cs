using System;
using System.Collections;
using System.Collections.Generic;
using GDG.Editor;
using GDG.Utils;
using UnityEditor;
using UnityEngine;

public partial class ProjectSetting : EditorWindow
{
    [MenuItem("Project/GlobalSetting", false)]
    static void CreateWindow()
    {
        Rect rect = new Rect(500, 500, 500, 200);
        EditorWindow.GetWindow<ProjectSetting>(false, "ProjectSetting", true);
    }
    private GUINavigationBar SelectionBar;
    public static string[] s_SelectionItemList = new string[]{"Logger","Input","Audio","Macro"};
    private static int s_CurrentSelect = 0 ;
    private void Awake() 
    {
        string path = "";
#if UNITY_ANDROID
        path = Application.persistentDataPath;
#else
        path = Application.dataPath;
#endif
        m_MacroConfigPath = path + m_MacroConfigPath;
        m_LoggerConfigPath = path + m_LoggerConfigPath;
        m_InputConfigPath = path + m_InputConfigPath;
        m_AudioConfigPath = path + m_AudioConfigPath;
    }
    private void OnEnable()
    {
        SelectionBar = new GUINavigationBar(s_SelectionItemList);
        LoadMacroConfig();
        LoadLoggerConfig();
        LoadInputConfig();
        LoadAudioConfig();
    }
    private void OnGUI()
    {
        using(new EditorGUILayout.HorizontalScope())
		{
            s_CurrentSelect = SelectionBar.GetSelectionIndexOnGUI(70,position.height);

            GUILayout.Space(30);

			switch(s_CurrentSelect)
			{
				case 0:
                    using (new EditorGUILayout.VerticalScope())
                    {
                        DrawSplitLine("LOGGER");
                        FixGroup(DrawLogger);
                    }
                    break;
				case 1:
                    using (new EditorGUILayout.VerticalScope())
                    {
                        DrawSplitLine("INPUT");
                        FixGroup(DrawInput);
                    }
                    break;
				case 2:
                    using (new EditorGUILayout.VerticalScope())
                    {
                        DrawSplitLine("AUDIO");
                        FixGroup(DrawAudio);
                    }
                    break;
				case 3:
                    using (new EditorGUILayout.VerticalScope())
                    {
                        DrawSplitLine("MACRO");
                        FixGroup(DrawMacro);
                    }
                    break;
            }
		}
    }
	private void FixGroup(Action callback)
	{
        GUILayout.Space(15);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(position.width/40);
            callback();
            GUILayout.Space(position.width/30);
        }
        GUILayout.Space(30);
    }
    public enum SelectionType
    {
        Logger,
        Audio,
        Input,
        Marco
    }
    private void DrawSplitLine(string title)
    {
        GUILayout.Space(20);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label($"—————————————————————————————{title}—————————————————————————————",EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
        }
    }
}
