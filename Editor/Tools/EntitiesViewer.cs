using System.Reflection;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GDG.ECS;
using GDG.Editor;
using IComponent = GDG.ECS.IComponent;
using System;
using System.Runtime.InteropServices;
using GDG.Utils;

public class EntitiesViewer : EditorWindow
{
    [MenuItem("GDGFramework/EntitiesViewer", false, 0)]
    static void CreateWindow()
    {
        EditorWindow.GetWindow<EntitiesViewer>(false, "EntitiesViewer", true);
    }
    private Vector2 m_CurrentEntitesScrollPosition = Vector2.zero;
    private Vector2 m_CurrentSystemScrollPosition = Vector2.zero;
    private Vector2 m_CurrentComponentScrollPosition = Vector2.zero;
    private Vector2 m_CurrentInspectorScrollPosition = Vector2.zero;
    private GUIStyle buttonStyle;
    private string searchText;
    private List<Entity> entityList = new List<Entity>();
    private int m_CurrentIndex = 0;
    private List<IComponent> m_CurrentComponents;
    private IComponent m_CurrentComponent;
    private Color beginColor;
    BaseWorld world;
    void  OnEnable()
    {
        beginColor = GUI.color;
    }
    void OnGUI()
    {   
        beginColor = GUI.color;

        buttonStyle = new GUIStyle("ProgressBarBar");
        buttonStyle.fontSize = 12;
        buttonStyle.normal.textColor = Color.white;
        
        if (world == null)
        {
            if (Application.isPlaying)
                world = BaseWorld.Instance;
        }

        using (new GUILayout.VerticalScope(WindowStyles.DarkBackground))
        {
            using (new GUILayout.HorizontalScope(WindowStyles.DarkBackground, GUILayout.Height(position.height / 1.5f)))
            {
                DrawSystem();
                DrawEntities();
                DrawComponent();
            }
            DrawInspector();
        }
    }
    void DrawSystem()
    {
        using (new GUILayout.VerticalScope(WindowStyles.GrayBackground, GUILayout.MinWidth(100), GUILayout.MaxWidth(250)))
        {
            using (new GUILayout.HorizontalScope(WindowStyles.GrayBackground))
            {
                GUILayout.Label("System", EditorStyles.boldLabel);
            }
            using (new GUILayout.VerticalScope())
            {
                using (var scope = new EditorGUILayout.ScrollViewScope(m_CurrentSystemScrollPosition))
                {
                    m_CurrentSystemScrollPosition = scope.scrollPosition;
                    if (world != null)
                        foreach (var system in World.Systems)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                system.SetActive(EditorGUILayout.Toggle(system.IsActived(), GUILayout.Width(20)));
                                if (system.IsActived())
                                {
                                    GDGEditorGUI.ActivedLabel(system.GetType().ToString());
                                }
                                else
                                {
                                    GDGEditorGUI.DisactivedLabel(system.GetType().ToString());
                                }

                            }
                        }
                }
            }
        }
    }
    void DrawEntities()
    {
        using (new GUILayout.VerticalScope(WindowStyles.LightBackground,GUILayout.MaxWidth(310)))//, GUILayout.MinWidth(120), GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true)))
        {
            using (new GUILayout.HorizontalScope(WindowStyles.LightBackground))
            {
                GUILayout.Label("Entities", EditorStyles.boldLabel,GUILayout.Width(60));
                searchText = GUILayout.TextField(searchText,new GUIStyle("SearchTextField"));
                GUILayout.Space(20);
            }
            using (new GUILayout.VerticalScope())
            {
                using (var scope = new EditorGUILayout.ScrollViewScope(m_CurrentEntitesScrollPosition))
                {
                    m_CurrentEntitesScrollPosition = scope.scrollPosition;
                    if (world != null)
                    {
                        if (m_CurrentComponents == null)
                        {
                            var temp_entities = World.EntityManager.GetAllEntity();
                            if(temp_entities.Count!=0)
                            {
                                var entity = World.EntityManager.GetAllEntity().First();
                                if (entity != null)
                                    m_CurrentComponents = World.EntityManager.GetComponents(entity);                                
                            }

                        }
                        var entities = World.EntityManager.GetAllEntity();
                        if(!string.IsNullOrEmpty(searchText))
                        {
                            entityList.Clear();
                            foreach(var item in entities)
                            {
                                if(item.Name.ToLower().Contains(searchText.ToLower()))
                                {
                                    entityList.Add(item);
                                }
                            }
                            entities = entityList;
                        }
                        for (int i = 0; i < entities.Count; i++)
                        {
                            if (m_CurrentIndex == i)
                            {
                                using (new GUILayout.HorizontalScope(WindowStyles.GrayBackground))
                                {
                                    if (entities[i].IsActived)
                                    {
                                        if (GUILayout.Button("- " + entities[i].Name, EditorStyles.label))
                                        {
                                            m_CurrentComponents = World.EntityManager.GetComponents(entities[i]);
                                            m_CurrentIndex = i;
                                            m_CurrentComponent = null;
                                        }
                                    }
                                    else
                                    {
                                        if (GUILayout.Button("- " + entities[i].Name, GDGEditorGUI.DisabledLabelStyle))
                                        {
                                            m_CurrentComponents = World.EntityManager.GetComponents(entities[i]);
                                            m_CurrentIndex = i;
                                            m_CurrentComponent = null;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (new GUILayout.HorizontalScope(WindowStyles.LightBackground))
                                {
                                    if (entities[i].IsActived)
                                    {
                                        if (GUILayout.Button("- " + entities[i].Name, EditorStyles.label))
                                        {
                                            m_CurrentComponents = World.EntityManager.GetComponents(entities[i]);
                                            m_CurrentIndex = i;
                                            m_CurrentComponent = null;
                                        }
                                    }
                                    else
                                    {
                                        if (GUILayout.Button("- " + entities[i].Name, GDGEditorGUI.DisabledLabelStyle))
                                        {
                                            m_CurrentComponents = World.EntityManager.GetComponents(entities[i]);
                                            m_CurrentIndex = i;
                                            m_CurrentComponent = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    void DrawComponent()
    {
        using (new GUILayout.VerticalScope(WindowStyles.GrayBackground))//,GUILayout.MinWidth(200), GUILayout.MaxWidth(250)))
        {
            using (new GUILayout.HorizontalScope(WindowStyles.GrayBackground))
            {
                GUILayout.Label("Component", EditorStyles.boldLabel);
            }
            //GUILayout.Space(10);
            using (new GUILayout.VerticalScope())
            {
                using (var scope = new EditorGUILayout.ScrollViewScope(m_CurrentComponentScrollPosition))
                {
                    m_CurrentComponentScrollPosition = scope.scrollPosition;
                    if (m_CurrentComponents != null)
                    {
                        if (m_CurrentComponent == null)
                            m_CurrentComponent = m_CurrentComponents.First();
                        foreach (var component in m_CurrentComponents)
                        {
                            if (m_CurrentComponent == component)
                            {
                                GUI.color = new Color(0.5f, 0.5f, 0.5f);
                                if (GUILayout.Button("  " + component.GetType().ToString() + "  ", buttonStyle, GUILayout.ExpandWidth(false)))
                                {
                                    m_CurrentComponent = component;
                                }
                                GUI.color = beginColor;
                            }
                            else
                            {
                                GUI.color = new Color(0.8f, 0.8f, 0.8f);
                                if (GUILayout.Button("  " + component.GetType().ToString() + "  ", buttonStyle, GUILayout.ExpandWidth(false)))
                                {
                                    m_CurrentComponent = component;
                                }
                                GUI.color = beginColor;
                            }

                        }
                    }
                }
            }
        }
        GUI.color = beginColor;
    }
    void DrawInspector()
    {
        using (new GUILayout.VerticalScope(WindowStyles.DarkBackground))
        {
            using (new GUILayout.VerticalScope(GUILayout.ExpandHeight(true)))
            {
                using (new GUILayout.HorizontalScope(WindowStyles.DarkBackground, GUILayout.ExpandWidth(true)))
                {
                    GUILayout.Label("ComponentData Inspector", EditorStyles.boldLabel);
                }
                GUILayout.Space(5);
                using (new GUILayout.VerticalScope())
                {
                    using (var scope = new EditorGUILayout.ScrollViewScope(m_CurrentInspectorScrollPosition))
                    {
                        m_CurrentInspectorScrollPosition = scope.scrollPosition;

                        var currentComponentFieldInfos = m_CurrentComponent?.GetType().GetFields();
                        var currentComponentGetPropertyInfos = m_CurrentComponent?.GetType().GetProperties();

                        if (currentComponentFieldInfos != null)
                        {
                            foreach (var info in currentComponentFieldInfos)
                            {
                                using (new GUILayout.HorizontalScope())
                                {
                                    MemberInfoViewer<FieldInfo>(info);
                                    GUILayout.Space(position.width / 1.5f);
                                }
                                GUILayout.Space(2);
                            }
                        }
                        if (currentComponentGetPropertyInfos != null)
                        {
                            foreach (var info in currentComponentGetPropertyInfos)
                            {
                                using (new GUILayout.HorizontalScope())
                                {
                                    MemberInfoViewer<PropertyInfo>(info);
                                    GUILayout.Space(position.width / 1.5f);
                                }
                                GUILayout.Space(2);
                            }
                        }
                    }


                }
            }
        }
    }
    private void MemberInfoViewer<T>(T info) where T : MemberInfo
    {
        Color TypeColor = new Color(0.4f, 0.6f, 1f,0.8f);
        Color childColor = beginColor;
        childColor.a = 0.5f;

        object value = null;
        Type type = default(Type);

        if (info is FieldInfo fieldInfo)
        {
            value = fieldInfo.GetValue(m_CurrentComponent);
            type = fieldInfo.FieldType;
        }
        else if (info is PropertyInfo propertyInfo)
        {
            value = propertyInfo.GetValue(m_CurrentComponent);
            type = propertyInfo.PropertyType;
        }

        using (new GUILayout.VerticalScope())
        {
            if (GDG.Utils.GDGTools.IsBlittable(value) || type == typeof(string) || type == typeof(char) || type == typeof(bool) || type.IsEnum)
            {
                using (new GUILayout.HorizontalScope())
                {

                    GUI.color = TypeColor;
                    GUILayout.Label($"[{type.Name}]  " , GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Label($"{info.Name}: ", GDGEditorGUI.LargeLabelStyle);
                    
                    GUILayout.FlexibleSpace();
                    GUI.color = childColor;
                    GUILayout.Label(value == null?"Null":value.ToString(), GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Space(10);
                }
            }
            else if (value is UnityEngine.Object obj)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUI.color = TypeColor;
                    GUILayout.Label($"[{type.Name}]  " , GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Label($"{info.Name}: ", GDGEditorGUI.LargeLabelStyle);
                    
                    
                    GUILayout.FlexibleSpace();
                    GUI.color = childColor;
                    var name = obj?.name;
                    GUILayout.Label(name == null?"Null":name, GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Space(10);
                }                
            }
            else if (value is IEnumerable enumerable)
            {

                using (new GUILayout.HorizontalScope())
                {
                    GUI.color = TypeColor;
                    GUILayout.Label($"[{type.Name}]  ", GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Label($"{info.Name}: ", GDGEditorGUI.LargeLabelStyle);
                    GUILayout.FlexibleSpace();
                }
                
                GUI.color = childColor;
                foreach (var item in enumerable)
                {
                    if (GDG.Utils.GDGTools.IsBlittable(item) || type == typeof(string) || type == typeof(char) || type == typeof(bool) || type.IsEnum)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();

                            GUILayout.Label($"► [{item.GetType().Name}]  ");

                            GUILayout.Label($"{nameof(item)}: ");
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(item == null?"Null":item.ToString());

                            GUILayout.Space(10);
                        }

                    }
                    else
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();

                            GUILayout.Label($"► [{item.GetType().Name}]  ");

                            GUILayout.Label($"{nameof(item)}: ");
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(item == null?"Null":item.ToString());

                            GUILayout.Space(10);
                        }
                    }
                }
                GUI.color = beginColor;
            }
            else if (value is Vector3 vector)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUI.color = TypeColor;
                    GUILayout.Label($"[{type.Name}]  " , GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Label($"{info.Name}: ", GDGEditorGUI.LargeLabelStyle);
                    
                    
                    GUILayout.FlexibleSpace();
                    GUI.color = childColor;
                    GUILayout.Label(vector.ToString(), GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Space(10);
                }                
            }
            else
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUI.color = TypeColor;
                    GUILayout.Label($"[{type.Name}]  " , GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Label($"{info.Name}: ", GDGEditorGUI.LargeLabelStyle);
                    
                    GUILayout.FlexibleSpace();
                    GUI.color = childColor;
                    GUILayout.Label(info == null?"Null":info.ToString(), GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Space(10);
                }
            }
        }
    }

}