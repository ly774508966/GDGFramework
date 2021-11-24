using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GDG.ModuleManager;
using Newtonsoft.Json;
using GDG.Utils;

public class DataTableConversion : EditorWindow
{
    private Rect InputRect = new Rect(10,10,200,50);
    private int firstIndex;
    private int secondIndex;
    private string[] firstOption = new string[] { "Json", "Xml", "Excel" };
    private string[] secondOption = new string[] { "Json", "Xml", "Excel"};
    private string firstPath;
    private string secondPath;
    private int startline=1;
    private int sheetIndex=0;
    private float width=40f;
    private bool isArray = false;
    private string ArrayName;

    [MenuItem("GDGFramework/Tools/DataTableConversion", false,11)]
	static void AddWindow()
	{
		Rect rect=new Rect(500,500,500,200);
	    EditorWindow.GetWindowWithRect<DataTableConversion>(rect);
	}
    private void OnGUI() 
    {
        EditorGUILayout.Space(15f);
        
        using(new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.Space();

            using(new EditorGUILayout.HorizontalScope())
            {
                firstIndex = EditorGUILayout.Popup(firstIndex, firstOption);

                GUILayout.Label("To");

                secondIndex = EditorGUILayout.Popup(secondIndex, firstOption);
            }
            EditorGUILayout.Space();
        }

        EditorGUILayout.Space(30f);

        using(new EditorGUILayout.HorizontalScope())
        {
            using(new EditorGUILayout.VerticalScope())
            {
                if (firstOption[firstIndex] == "Excel" || secondOption[secondIndex] == "Excel")
                {
                    int a = 1;
                    int b = 0;
                    using(new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.Space(5f);

                        GUILayout.Label("start line:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(60));
                        startline = int.TryParse(GUILayout.TextField(startline.ToString(), GUILayout.ExpandWidth(false), GUILayout.MaxWidth(100)),out a)?a:1;

                        EditorGUILayout.Space(10f);

                        GUILayout.Label("sheet index:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(70));
                        sheetIndex = int.TryParse(GUILayout.TextField(sheetIndex.ToString(), GUILayout.ExpandWidth(false), GUILayout.MaxWidth(100)),out b)?b:0;

                        EditorGUILayout.Space(5f);

                        width = 35f;
                    }
                }
                else if(secondOption[secondIndex] == "Xml")
                {
                    using(new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.Space(5f);

                        isArray = GUILayout.Toggle(isArray,"IsArray");
                        if(isArray)
                        {
                            GUILayout.Label("ArrayName:", GUILayout.ExpandWidth(false), GUILayout.MaxWidth(70));
                            ArrayName = GUILayout.TextField(ArrayName, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(100));
                            EditorGUILayout.Space(5f);
                        }

                        width = 35f;
                    }
                }
                else
                    width = 53f;

                EditorGUILayout.Space(10f);

                using(new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.Space(10f,false);
                    GUILayout.Label($"{firstOption[firstIndex]} Path:", GUILayout.ExpandWidth(false),GUILayout.MinWidth(80));
                    firstPath = GUILayout.TextField(firstPath);
                    EditorGUILayout.Space(10f,false);
                }

                EditorGUILayout.Space(5f,false);

                using(new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.Space(10f,false);
                    GUILayout.Label($"{secondOption[secondIndex]} Path:", GUILayout.ExpandWidth(false),GUILayout.MinWidth(80));
                    secondPath = GUILayout.TextField(secondPath);
                    EditorGUILayout.Space(10f,false);
                }
            }
        }
        EditorGUILayout.Space(width);
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.Space(50f, false);
            if (GUILayout.Button("Generate"))
            {
                string jsonStr = "";
                string targetPath = secondPath;
                switch (firstOption[firstIndex])
                {
                    case "Json": jsonStr = JsonManager.ReadJson(firstPath); break;
                    case "Xml": jsonStr = XmlManager.XmlToJson(firstPath);break;
                    case "Excel": jsonStr = ExcelManager.ExcelToJson(firstPath, startline, sheetIndex); break;
                }
                switch (secondOption[secondIndex])
                {
                    case "Json": targetPath = JsonManager.WriteJson(secondPath,jsonStr);break;
                    case "Xml": targetPath = XmlManager.JsonToXml(jsonStr, secondPath,ArrayName); break;
                    case "Excel": targetPath = ExcelManager.JsonToExcel(jsonStr, secondPath, startline, sheetIndex); break;
                }
                if(string.IsNullOrEmpty(jsonStr))
                {
                    throw new CustomErrorException("Convert failed ! Please check format.");
                }
                else
                {
                    Log.Editor($"Convert successfully ! TargetPath : {targetPath}");
                }
            }
            EditorGUILayout.Space(50f, false);
        }
    }
}