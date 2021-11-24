using UnityEngine;
using UnityEditor;
using GDG.Utils;

public class GUIStyleViewer : EditorWindow
{
    Vector2 scrollPosition = new Vector2(0, 0);
    private string search = "";
    GUIStyle textStyle;

    private static GUIStyleViewer window;
    [MenuItem("GDGFramework/Tools/GUIStyleViewer", false, 12)]
    private static void OpenStyleViewer()
    {
        window = GetWindow<GUIStyleViewer>(false, "GUIStyle");
    }

    void OnGUI()
    {
        if (textStyle == null)
        {
            textStyle = new GUIStyle("HeaderLabel");
            textStyle.fontSize = 25;
        }
         GUILayout.Label("Search:");
         search = EditorGUILayout.TextField(search);

         GUILayout.Label("Result", textStyle);
 
         GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
         GUILayout.Label("Simple", textStyle, GUILayout.Width(300));
         GUILayout.Label("Name", textStyle, GUILayout.Width(300));
         GUILayout.EndHorizontal();


        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        var styles = GUI.skin.customStyles;
        for (int i = 0; i < styles.Length;i++)
        {
            if (styles[i].name.ToLower().Contains(search.ToLower()))
            {
                GUILayout.Space(15);
                GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                if (GUILayout.Button(styles[i].name, styles[i], GUILayout.Width(300)))
                {
                    EditorGUIUtility.systemCopyBuffer = styles[i].name;
                    Log.Editor("StyleName: " + styles[i].name + $" |  Index: {i}");
                }
                EditorGUILayout.SelectableLabel(styles[i].name, GUILayout.Width(300));
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
    }
}
