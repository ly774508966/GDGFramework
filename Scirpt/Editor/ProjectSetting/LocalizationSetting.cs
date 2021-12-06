using System;
using System.Collections.Generic;
using System.Linq;
using GDG.Base;
using GDG.Editor;
using GDG.Localization;
using GDG.ModuleManager;
using GDG.Utils;
using UnityEditor;
using UnityEngine;

public partial class ProjectSetting
{
    LocalizationConfig m_languageSetting;
    private string m_LocalizationPath = Configurations.ConfigPath + "\\LocalizationConfig.json";
    private Language m_CurrentLanguage = Language.Chinese_Simplified;
    private List<string> languages = new List<string>();
    private int m_CurrentLanguageIndex = 0;
    private string m_NewKey;
    private bool m_isCopy;
    private Language m_CopyLanguage;
    private GUITable m_LocaleTable;
    private void SaveLocalizationConfig()
    {
        JsonManager.SaveData<LocalizationConfig>(m_languageSetting, m_LocalizationPath);
        AssetDatabase.Refresh();
        Log.Sucess("Save Succesfully !");
    }
    private void LoadLocalizationConfig()
    {
        m_LocaleTable = new GUITable(
            ("Key", GUIElementType.Label, 200),
            ("Value", GUIElementType.Custom, 200),
            ("Remove", GUIElementType.Button, 100)
        );
        languages = Enum.GetNames(typeof(Language)).ToList();
        var languageConfig = JsonManager.LoadData<LocalizationConfig>(m_LocalizationPath);
        if (languageConfig != null)
        {
            m_languageSetting = languageConfig;
            m_CurrentLanguageIndex = (int)Enum.Parse(typeof(Language), m_languageSetting.CurrentLanguage.ToString());
        }

    }
    private void DrawLocalization()
    {
        if (m_languageSetting == null)
            return;

        using (new EditorGUILayout.VerticalScope())
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Language: ", EditorStyles.boldLabel, GUILayout.Width(80));
                m_CurrentLanguageIndex = EditorGUILayout.Popup(m_CurrentLanguageIndex, languages.ToArray(), GUILayout.Width(130));
                m_CurrentLanguage = (Language)Enum.Parse(typeof(Language), languages[m_CurrentLanguageIndex]);
                m_languageSetting.CurrentLanguage = m_CurrentLanguage;

                if (m_isCopy)
                {
                    GUILayout.FlexibleSpace();

                    EditorGUILayout.LabelField("Copy: ", EditorStyles.boldLabel, GUILayout.Width(45));
                    using (new EditorGUILayout.HorizontalScope(WindowStyles.CreateSolidColorStyle(new Color(0.15f, 0.15f, 0.15f)), GUILayout.Width(130)))
                    {
                        EditorGUILayout.LabelField(m_CopyLanguage.ToString(), EditorStyles.boldLabel, GUILayout.Width(130));
                    }
                }


            }
            GUILayout.Space(20);
            m_LocaleTable.DrawTitle();

            if (m_languageSetting.LanguageMapping.TryGetValue(m_CurrentLanguage, out Dictionary<string, string> dic))
            {
                using (var scope = new EditorGUILayout.ScrollViewScope(m_CurrentScrollPosition))
                {
                    m_CurrentScrollPosition = scope.scrollPosition;
                    using (new EditorGUILayout.VerticalScope())
                    {
                        for (int i = 0; i < dic.Count; ++i)
                        {
                            var kv = dic.ElementAt(i);
                            bool isRemove = false;
                            Action callback = () =>
                            {
                                dic.Remove(kv.Key);
                                isRemove = true;
                            };

                            var k = kv.Key;
                            Action action = () =>
                            {
                                dic[kv.Key] = EditorGUILayout.TextField(kv.Value, EditorStyles.label, GUILayout.MinWidth(30), GUILayout.MaxWidth(200));
                            };

                            var item = new Tuple<Action, string>(callback, "Remove");
                            m_LocaleTable.DrawRow(ref k, ref action, ref item);
                            if (isRemove)
                                break;
                        }
                    }
                }
            }

            GUILayout.FlexibleSpace();
            using (new EditorGUILayout.HorizontalScope())
            {
                m_NewKey = EditorGUILayout.TextField(m_NewKey);

                if (GUILayout.Button("Add Key"))
                {
                    if (dic == null)
                    {
                        dic = new Dictionary<string, string>();
                        m_languageSetting.LanguageMapping.Add(m_CurrentLanguage, dic);
                    }
                    if (!string.IsNullOrEmpty(m_NewKey))
                        dic.Add(m_NewKey, "");
                }

                GUILayout.Space(20);
                if (GUILayout.Button("Copy"))
                {
                    m_isCopy = true;
                    m_CopyLanguage = m_CurrentLanguage;
                }
                if (GUILayout.Button("Paste"))
                {
                    m_languageSetting.LanguageMapping[m_CurrentLanguage] = m_languageSetting.LanguageMapping[m_CopyLanguage];
                }


                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Apply", GUILayout.Width(100)))
                {
                    SaveLocalizationConfig();
                }
            }
        }
    }
}
