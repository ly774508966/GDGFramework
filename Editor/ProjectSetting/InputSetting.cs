using System;
using System.Collections;
using System.Collections.Generic;
using GDG.Editor;
using GDG.ModuleManager;
using GDG.Utils;
using UnityEditor;
using UnityEngine;

public partial class ProjectSetting
{
    public class InputDialog : EditorWindow
    {
        private List<KeyCode> keyCodes = new List<KeyCode>();
        private int keydownCount = 0;

        public static void CreateWizard()
        {
            Rect rect = new Rect(500, 500, 120, 30);
            EditorWindow.GetWindow<InputDialog>(false, "Key Input Listener", true);
        }
        void OnGUI()
        {
            using (new GUILayout.VerticalScope(WindowStyles.DarkBackground))
            {
                var keyEvent = Event.current;

                if (keyEvent.type == EventType.KeyDown)
                {
                    foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                    {
                        if (keyEvent.keyCode == keyCode && keyEvent.keyCode != KeyCode.None)
                        {
                            keydownCount++;

                            if (keydownCount > 3)
                            {
                                keydownCount = 1;
                                keyCodes.Clear();
                            }
                            keyCodes.Add(keyCode);
                            break;
                        }
                    }
                }

                var keyCombine = string.Join(" + ", keyCodes.ToArray());
                GUILayout.FlexibleSpace();
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(keyCombine, EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                }

                GUILayout.FlexibleSpace();
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Reset", GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        keyCodes.Clear();
                        keydownCount = 0;
                    }
                    GUILayout.Space(20);
                    if (GUILayout.Button("Apply", GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        currentKey.keyCodes = keyCodes;
                        this.Close();
                    }
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Space(10);
            }
            this.Repaint();
        }
    }
    private string m_InputConfigPath = "/GDGFramework/Config/InputConfig.json";
    public List<Key> m_keyList = new List<Key>();
    private GUITable m_InputTable;
    private string m_NewKeyName;
    private static Key currentKey;
    private void SaveInputConfig()
    {
        JsonManager.SaveData<List<Key>>(m_keyList, m_InputConfigPath);
        AssetDatabase.Refresh();
        Log.Editor("Save Succesfully !");
    }
    private void LoadInputConfig()
    {
        m_InputTable = new GUITable(
            ("KeyName", GUIElementType.TextField, 200),
            ("Key", GUIElementType.LabelButton, 200),
            ("Remove", GUIElementType.Button, 100)
        );

        var inputConfig = JsonManager.LoadData<List<Key>>(m_InputConfigPath);
        if(inputConfig!=null)
            m_keyList = inputConfig;
    }
    private void DrawInput()
    {
        using (new EditorGUILayout.VerticalScope())
        {
            using (var scope = new EditorGUILayout.ScrollViewScope(m_CurrentScrollPosition))
            {
                m_CurrentScrollPosition = scope.scrollPosition;
                m_InputTable.DrawTitle();
                bool isRemove = false;

                for (int i = 0; i < m_keyList.Count; i++)
                {
                    Action callback = () =>
                    {
                        currentKey = m_keyList[i];
                        InputDialog.CreateWizard();
                    };
                    Action remove = () =>
                    {
                        m_keyList.RemoveAt(i);
                        isRemove = true;
                    };
                    if (isRemove)
                        break;

                    Tuple<Action, string> Remove = new Tuple<Action, string>(remove, "Remove");
                    Tuple<Action, string> keyword = new Tuple<Action, string>(callback, m_keyList[i].ToString());
                    m_InputTable.DrawRow(ref m_keyList[i].keyName, ref keyword, ref Remove);
                }
            }

            GUILayout.FlexibleSpace();
            using (new EditorGUILayout.HorizontalScope())
            {
                m_NewKeyName = EditorGUILayout.TextField(m_NewKeyName);

                if (GUILayout.Button("Add Key"))
                {
                    if (!string.IsNullOrEmpty(m_NewKeyName))
                        m_keyList.Add(new Key() { keyName = m_NewKeyName, keyCodes = new List<KeyCode>() });
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Apply",GUILayout.Width(100)))
                {
                    SaveInputConfig();
                }
            }

        }
    }
}
