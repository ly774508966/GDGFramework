using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GDG.Utils;
using UnityEditor;
using UnityEngine;

namespace GDG.Editor
{
    public class AutoNamespace : EditorWindow
    {
        [MenuItem("GDGFramework/Tools/Auto Namespace", false, 13)]
        static void CreateWindow()
        {
            Rect rect=new Rect(500,500,350,100);
            EditorWindow.GetWindowWithRect<AutoNamespace>(rect ,false, "AutoNamespace", true);
        }
        private string CurrentNamespace;
        private bool enable = true;
        private string tempNamespace;
        void Awake() 
        {
            enable = EditorPrefs.GetBool("EnableAutoNamespace",false);
            CurrentNamespace = EditorPrefs.GetString("CurrentNamespace");
            if(!string.IsNullOrEmpty(CurrentNamespace))
            {
                tempNamespace = CurrentNamespace;
            }
        }
        void OnGUI()
        {
            using(new EditorGUILayout.VerticalScope())
            {
                GUILayout.FlexibleSpace();
                using(new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Namespace:",GUILayout.ExpandWidth(false));
                    tempNamespace = GUILayout.TextField(tempNamespace);
                    enable = GUILayout.Toggle(enable,"Enabled",GUILayout.ExpandWidth(false));
                }
                GUILayout.Space(20);
                using(new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button("Apply",GUILayout.MinWidth(100f)))
                    {
                        if (!string.IsNullOrEmpty(tempNamespace))
                        {
                            CurrentNamespace = "";
                        }
                        CurrentNamespace = tempNamespace;
                        EditorPrefs.SetString("CurrentNamespace", CurrentNamespace);
                        EditorPrefs.SetBool("EnableAutoNamespace", enable);
                        Log.Editor("Save Succesfully !");
                    }
                    GUILayout.FlexibleSpace();
                }
                GUILayout.FlexibleSpace();
            }
        }
    }
    public class AddNamespace : UnityEditor.AssetModificationProcessor
    {
        private static void OnWillCreateAsset(string assetName)
        {
            if(EditorPrefs.GetBool("EnableAutoNamespace",false))
            {
                string filePath = assetName.Replace(".meta", "");
                filePath.GetFileName(out string extensionName);
                if(extensionName == "cs")
                {
                    string namespaceStr = EditorPrefs.GetString("CurrentNamespace");
                    if(string.IsNullOrEmpty(namespaceStr))
                        return;
                    
                    StringBuilder stringBuilder = new StringBuilder();
                    using (StreamReader streamReader = new StreamReader(filePath))
                    {
                        bool isAddNamespace = false;

                        while (streamReader.Peek() >= 0)
                        {
                            var str = streamReader.ReadLine();
                            if (!str.Contains("using") && !isAddNamespace)
                            {
                                isAddNamespace = true;
                                stringBuilder.Append("\nnamespace " + namespaceStr + "\n{\n");
                            }
                            if (isAddNamespace)
                            {
                                stringBuilder.Append($"\t{str}" + "\n");
                            }
                            else
                            {
                                stringBuilder.Append(str + "\n");
                            }
                        }
                        stringBuilder.Append("}");
                    }
                    using(StreamWriter streamWriter = new StreamWriter(filePath))
                    {
                        streamWriter.Write(stringBuilder.ToString());
                        streamWriter.Flush();
                    }
                }
            }
        }
    }
}