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

public class EntitiesViewer : EditorWindow
{
    [MenuItem("GDGFramework/EntitiesViewer", false, 1)]
    static void CreateWindow()
    {
        EditorWindow.GetWindow<EntitiesViewer>(false, "EntitiesViewer", true);
    }
    private Vector2 m_CurrentEntitesScrollPosition = Vector2.zero;
    private Vector2 m_CurrentSystemScrollPosition = Vector2.zero;
    private Vector2 m_CurrentComponentScrollPosition = Vector2.zero;
    private Vector2 m_CurrentInspectorScrollPosition = Vector2.zero;
    private GUIStyle buttonStyle;
    private int m_CurrentIndex = 0;
    private List<IComponent> m_CurrentComponents;
    private IComponent m_CurrentComponent;
    private Color beginColor;
    BaseWorld world;
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
            using (new GUILayout.HorizontalScope(WindowStyles.DarkBackground,GUILayout.Height(position.height/1.5f)))
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
        using (new GUILayout.VerticalScope(WindowStyles.LightBackground))//, GUILayout.MinWidth(120), GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true)))
        {
            using (new GUILayout.HorizontalScope(WindowStyles.LightBackground))
            {
                GUILayout.Label("Entities", EditorStyles.boldLabel);
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
                            var entity = World.EntityManager.GetAllEntity().First();
                            if (entity != null)
                                m_CurrentComponents = World.EntityManager.GetComponent(entity);
                        }
                        var entities = World.EntityManager.GetAllEntity();
                        for (int i = 0; i < entities.Count; i++)
                        {
                            if (m_CurrentIndex == i)
                            {
                                using (new GUILayout.HorizontalScope(WindowStyles.GrayBackground))
                                {
                                    if (entities[i].IsActived)
                                    {
                                        if (GUILayout.Button(entities[i].Name, EditorStyles.label))
                                        {
                                            m_CurrentComponents = World.EntityManager.GetComponent(entities[i]);
                                            m_CurrentIndex = i;
                                            m_CurrentComponent = null;
                                        }
                                    }
                                    else
                                    {
                                        if (GUILayout.Button(entities[i].Name, GDGEditorGUI.DisabledLabelStyle))
                                        {
                                            m_CurrentComponents = World.EntityManager.GetComponent(entities[i]);
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
                                        if (GUILayout.Button(entities[i].Name, EditorStyles.label))
                                        {
                                            m_CurrentComponents = World.EntityManager.GetComponent(entities[i]);
                                            m_CurrentIndex = i;
                                            m_CurrentComponent = null;
                                        }
                                    }
                                    else
                                    {
                                        if (GUILayout.Button(entities[i].Name, GDGEditorGUI.DisabledLabelStyle))
                                        {
                                            m_CurrentComponents = World.EntityManager.GetComponent(entities[i]);
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
                                    FieldViewer(info);
                                    //GUILayout.FlexibleSpace();
                                    GUILayout.Space(position.width/1.5f);
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
                                    PropertyViewer(info);
                                    GUILayout.Space(position.width/1.5f);
                                }
                                GUILayout.Space(2);
                            }
                        }
                    }


                }
            }
        }
    }
    private void FieldViewer(FieldInfo info)
    {
        Color valueColor = beginColor;
        Color childColor = beginColor;
        childColor.a = 0.5f;
        using (new GUILayout.VerticalScope())
        {
            if (GDG.Utils.GDGTools.IsBlittable(info.GetValue(m_CurrentComponent)))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUI.color = valueColor;
                    GUILayout.Label($"[{info.FieldType.Name}]  {info.Name}: ", GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.FlexibleSpace();
                    GUI.color = childColor;
                    GUILayout.Label(info.GetValue(m_CurrentComponent).ToString(), GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Space(10);
                }
            }
            else if (info.FieldType == typeof(string) || info.FieldType == typeof(char) || info.FieldType == typeof(bool) || info.FieldType.IsEnum)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUI.color = valueColor;
                    GUILayout.Label($"[{info.FieldType.Name}]  {info.Name}: ", GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.FlexibleSpace();
                    GUI.color = childColor;
                    GUILayout.Label(info.GetValue(m_CurrentComponent).ToString(), GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Space(10);
                }
            }
            else if (info.GetValue(m_CurrentComponent) is IEnumerable enumerable)
            {
                GUI.color = valueColor;
                GUILayout.Label($"[{info.FieldType.Name}]  {info.Name}: ", GDGEditorGUI.LargeLabelStyle);
                GUI.color = beginColor; ;
                GUI.color = childColor;
                foreach (var item in enumerable)
                {
                    if (GDG.Utils.GDGTools.IsBlittable(item))
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Label($"● [{item.GetType().Name}]  {nameof(item)}: ", GDGEditorGUI.LargeLabelStyle);
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(item.ToString(), GDGEditorGUI.LargeLabelStyle);

                            GUILayout.Space(10);
                        }

                    }
                    else
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Label($"● [{item.GetType().Name}]  {nameof(item)}: ", GDGEditorGUI.LargeLabelStyle);
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("[Invisible]", GDGEditorGUI.LargeLabelStyle);

                            GUILayout.Space(10);
                        }
                    }
                }
                GUI.color = beginColor;
            }
            else
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUI.color = valueColor;
                    GUILayout.Label($"[{info.FieldType.Name}]  {info.Name}: ", GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.FlexibleSpace();
                    GUI.color = childColor;
                    GUILayout.Label("[Invisible]", GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Space(10);
                }
            }
        }
    }
    private void PropertyViewer(PropertyInfo info)
    {
        Color valueColor = beginColor;
        Color childColor = beginColor;
        childColor.a = 0.5f;
        using (new GUILayout.VerticalScope())
        {
            if (GDG.Utils.GDGTools.IsBlittable(info.GetValue(m_CurrentComponent)))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUI.color = valueColor;
                    GUILayout.Label($"[{info.PropertyType.Name}]  {info.Name}: ", GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.FlexibleSpace();
                    GUI.color = childColor;
                    GUILayout.Label(info.GetValue(m_CurrentComponent).ToString(), GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Space(10);
                }
            }
            else if (info.PropertyType == typeof(string) || info.PropertyType == typeof(char) || info.PropertyType == typeof(bool) || info.PropertyType.IsEnum)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUI.color = valueColor;
                    GUILayout.Label($"[{info.PropertyType.Name}]  {info.Name}: ", GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.FlexibleSpace();
                    GUI.color = childColor;
                    GUILayout.Label(info.GetValue(m_CurrentComponent).ToString(), GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Space(10);
                }
            }
            else if (info.GetValue(m_CurrentComponent) is IEnumerable enumerable)
            {
                GUI.color = valueColor;
                GUILayout.Label($"[{info.PropertyType.Name}]  {info.Name}: ", GDGEditorGUI.LargeLabelStyle);
                GUI.color = beginColor; ;
                GUI.color = childColor;
                foreach (var item in enumerable)
                {
                    if (GDG.Utils.GDGTools.IsBlittable(item))
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Label($"● [{item.GetType().Name}]  {nameof(item)}: ", GDGEditorGUI.LargeLabelStyle);
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(item.ToString(), GDGEditorGUI.LargeLabelStyle);

                            GUILayout.Space(10);
                        }

                    }
                    else
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Label($"● [{item.GetType().Name}]  {nameof(item)}: ", GDGEditorGUI.LargeLabelStyle);
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("[Invisible]", GDGEditorGUI.LargeLabelStyle);

                            GUILayout.Space(10);
                        }
                    }
                }
                GUI.color = beginColor;
            }
            else
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUI.color = valueColor;
                    GUILayout.Label($"[{info.PropertyType.Name}]  {info.Name}: ", GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.FlexibleSpace();
                    GUI.color = childColor;
                    GUILayout.Label("[Invisible]", GDGEditorGUI.LargeLabelStyle);
                    GUI.color = beginColor;
                    GUILayout.Space(10);
                }
            }
        }
    }
}