using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEditor;

public class CSScriptCreator : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        var obj = GenerateScriptFromTemplate(pathName, resourceFile);
        ProjectWindowUtil.ShowCreatedAsset(obj);
    }
    public static Object GenerateScriptFromTemplate(string pathName,string resourceFile)
    {
        string content = "";
        using(StreamReader reader = new StreamReader(resourceFile))
        {
            content = reader.ReadToEnd();
        }

        var fullPath = Path.GetFullPath(pathName);
        var fileName = Path.GetFileNameWithoutExtension(pathName);

        //替换名称
        content = Regex.Replace(content, "#SCRIPTNAME#", fileName);
        UTF8Encoding encoding = new UTF8Encoding(true, false);

        using(StreamWriter writer = new StreamWriter(fullPath, false, encoding))
        {
            writer.Write(content);
        }

        //刷新AssetDataBase
        AssetDatabase.ImportAsset(pathName);
        AssetDatabase.Refresh();
        return AssetDatabase.LoadAssetAtPath<Object>(pathName);
    }
}
