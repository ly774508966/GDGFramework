using System;
using System.Collections.Generic;
using GDG.Utils;
using UnityEngine;
using System.IO;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Build.Reporting;
using GDG.Editor;
#endif

namespace GDG.Config
{
    public class Configurations
    {
        public static string ConfigPath
        {
            get
            {
#if UNITY_EDITOR
                return $"{EditorPath.AssetsPath}\\GDGFramework\\Config";
#else
				return $"{System.Environment.CurrentDirectory}\\Config";
#endif
            }
        }
#if UNITY_EDITOR
        //打包后自动拷贝配置文件
        [PostProcessBuildAttribute]
        private static void OnPostprocessBuild(BuildTarget BuildTarget, string path)
        {
            try
            {
                FileBuilder.CopyFolder($"{Application.dataPath}/GDGFramework/Config", path.GetFolderPath() + "/Config");
                Log.Sucess($"成功拷贝配置文件，路径为：{path.GetFolderPath()}/Config");
            }
            catch (Exception ex)
            {
                throw new CustomErrorException(ex.Message);
            }
        }
#endif
    }

}
