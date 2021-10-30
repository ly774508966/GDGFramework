using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GDG.Utils;
using UnityEditor;
using UnityEngine;
using GDG.ModuleManager;
using GDG.Editor;
using GUIElementType = GDG.Editor.GUIElementType;
using System;

public class Macro
{
    public bool enabled = true;
    public string name;
    public string description;
}
public partial class ProjectSetting
{
    private Dictionary<int, string> m_Index2CategoriesMapping = new Dictionary<int, string>();
    private Dictionary<string, List<Macro>> m_Categories2MarcoesMapping = new Dictionary<string, List<Macro>>()
    { { "Default", new List<Macro>()
        {
            new Macro(){name = "EDITOR_DEBUG" , description = "Used in the project testing phase"}
        }} 
    };
    private int m_CurrentCategoriesIndex = 0;
    private string m_NewCategoriesName;
    private string m_NewMarcoName;
    private string m_CurrentCategoriesName = "Default";
    private Vector2 m_CurrentScrollPosition = Vector2.zero;
    private GUITable m_MacroTable;
    private string m_MacroConfigPath = "/GDGFramework/Config/MacroConfig.json";

    private void SaveMacroConfig()
    {
        JsonManager.SaveData<Dictionary<string, List<Macro>>>(m_Categories2MarcoesMapping,m_MacroConfigPath);

        List<string> defineList = new List<string>();

        foreach(var macrolist in m_Categories2MarcoesMapping.Values)
        {
            foreach (var macro in macrolist)
            {
                if(!defineList.Contains(macro.name))
                {
                    if(macro.enabled)
                    defineList.Add(macro.name);
                }
            }
        }
        var macroStr = string.Join(";", defineList.ToArray());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, macroStr);
        AssetDatabase.Refresh();
        Log.Editor("Save Succesfully !");
    }
    private void LoadMacroConfig()
    {
        m_MacroTable = new GUITable
        (
            ("MacroName",GUIElementType.TextField,100),
            ("Enable",GUIElementType.Toggle,100),
            ("Description",GUIElementType.TextField,300),
            ("Remove",GUIElementType.Button,100)
        );
        
        var macroConfig = JsonManager.LoadData<Dictionary<string, List<Macro>>>(m_MacroConfigPath);

        if(macroConfig!=null)
            m_Categories2MarcoesMapping = macroConfig;
    }
    private void DrawMacro()
    {
        m_Index2CategoriesMapping.Clear();
        for (int i = 0; i < m_Categories2MarcoesMapping.Keys.Count;i++)
        {
            m_Index2CategoriesMapping.Add(i, m_Categories2MarcoesMapping.Keys.ElementAt(i));
        }
        
        using (new EditorGUILayout.VerticalScope())
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Categories: ",EditorStyles.boldLabel ,GUILayout.Width(80));
                m_CurrentCategoriesIndex = EditorGUILayout.Popup(m_CurrentCategoriesIndex, m_Categories2MarcoesMapping.Keys.ToArray(),GUILayout.Width(100));
                
                GUILayout.FlexibleSpace();

                if(GUILayout.Button("Create Page"))
                {
                    if(!string.IsNullOrEmpty(m_NewCategoriesName))
                    {
                        m_Categories2MarcoesMapping.Add(m_NewCategoriesName, new List<Macro>());
                        m_CurrentCategoriesIndex = m_Categories2MarcoesMapping.Count - 1;
                    }
                }                
                m_NewCategoriesName = EditorGUILayout.TextField(m_NewCategoriesName);
            }
            GUILayout.Space(20);

            m_MacroTable.DrawTitle();

            if (m_Index2CategoriesMapping.ContainsKey(m_CurrentCategoriesIndex))
            {
                m_CurrentCategoriesName = m_Index2CategoriesMapping[m_CurrentCategoriesIndex];

                using (var scope = new EditorGUILayout.ScrollViewScope(m_CurrentScrollPosition))
                {
                    m_CurrentScrollPosition = scope.scrollPosition;
                    using (new EditorGUILayout.VerticalScope())
                    {
                        foreach (var macro in m_Categories2MarcoesMapping[m_CurrentCategoriesName])
                        {
                            bool isRemove = false;
                            Action callback = () =>
                            {
                                m_Categories2MarcoesMapping[m_CurrentCategoriesName].Remove(macro);
                                isRemove = true;
                            };
                            var item = new Tuple<Action,string>(callback,"Remove");
                            m_MacroTable.DrawRow(ref macro.name, ref macro.enabled, ref macro.description, ref item);
                            
                            if(isRemove)
                                break;
                        }
                    }
                }
            }
            GUILayout.FlexibleSpace();
            using (new EditorGUILayout.HorizontalScope())
            {
                m_NewMarcoName = EditorGUILayout.TextField(m_NewMarcoName);

                if(GUILayout.Button("Add Macro"))
                {
                    if(!string.IsNullOrEmpty(m_NewMarcoName))
                        m_Categories2MarcoesMapping[m_CurrentCategoriesName].Add(new Macro() { name = m_NewMarcoName });
                }

                GUILayout.FlexibleSpace();
                if(GUILayout.Button("Remove Page"))
                {
                    if(m_Categories2MarcoesMapping.ContainsKey(m_CurrentCategoriesName))
                        m_Categories2MarcoesMapping.Remove(m_CurrentCategoriesName);
                    m_CurrentCategoriesIndex = 0;
                }
                if(GUILayout.Button("Apply"))
                {
                    SaveMacroConfig();
                }
            }
        }
    }
}
