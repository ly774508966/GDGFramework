using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using GDG.Base;
using GDG.Utils;
using UnityEngine;

namespace GDG.ModuleManager
{
    internal class UserFileManager : LazySingleton<UserFileManager>
    {
        internal static string AssetPath{ get => Application.dataPath; }
        public static string Path
        {
            get
            {
#if UNITY_ANDROID
                return $"{Application.persistentDataPath}/User";
#else
                return $"{Application.streamingAssetsPath}/User";
#endif
            }
        }
        /// <summary>
        /// 在UserFileMgr.Path下创建文件夹
        /// </summary>
        /// <param name="foldername">文件夹名</param>
        public static void BuildFolder_Async(string foldername)
        {
            MonoWorld.Instance.StartCoroutine(BuildFoldAsync(foldername));
        }
        /// <summary>
        /// 在自定义路径下创建文件夹
        /// </summary>
        /// <param name="foldername">文件夹名</param>
        /// <param name="path">路径</param>
        public static void BuildFolder_Async(string foldername, string path)
        {
            MonoWorld.Instance.StartCoroutine(BuildFoldAsync(foldername, path));
        }
        public static void BuildFolder(string foldername)
        {
            if (!Directory.Exists($"{Path}/{foldername}"))
            {
                DirectoryInfo info = Directory.CreateDirectory($"{Path}/{foldername}");
                Log.Warning($"创建了文件夹{Path}/{foldername}");
            }
        }
        public static void BuildFolder(string foldername, string path)
        {
            if (!Directory.Exists($"{path}/{foldername}"))
            {
                DirectoryInfo info = Directory.CreateDirectory($"{path}/{foldername}");
                Log.Warning($"创建了文件夹{path}/{foldername}");
            }
        }
        /// <summary>
        /// 在文件夹路径下创建文件
        /// </summary>
        /// <param name="foldername">相对于UserFileMgr.Path下的文件夹路径</param>
        /// <param name="filename">folderPath下的文件名（包含后缀）</param>
        public static void BuildFile_Async(string foldername, string filename)
        {
            MonoWorld.Instance.StartCoroutine(BuildFileAsync(foldername, filename));
        }
        /// <summary>
        /// 在文件夹路径下创建文件
        /// </summary>
        /// <param name="foldername">自定义路径path下的文件夹名称</param>
        /// <param name="filename">文件名称</param>
        /// <param name="path"自定义的文件夹路径></param>
        public static void BuildFile_Async(string foldername, string filename, string path)
        {
            MonoWorld.Instance.StartCoroutine(BuildFileAsync(foldername, filename, path));
        }
        public static void BuildFile(string foldername, string filename)
        {
            DirectoryInfo info = Directory.CreateDirectory($"{Path}/{foldername}");
            if (!File.Exists($"{Path}/{foldername}/{filename}"))
            {
                using (FileStream fs = File.Create($"{Path}/{foldername}/{filename}"))
                {
                    Log.Warning($"创建了文件{Path}/{foldername}/{filename}");
                }
            }
        }
        public static void BuildFile(string foldername, string filename, string path)
        {
            BuildFolder(foldername, path);
            if (!File.Exists($"{Path}/{foldername}/{filename}"))
            {
                using (FileStream fs = File.Create($"{path}/{foldername}/{filename}"))
                {
                    Log.Warning($"创建了文件{path}/{foldername}/{filename}");
                }
            }
        }
        /// <summary>
        /// 向文件中写入信息
        /// </summary>
        /// <param name="filePath">相对于UserFileMgr.Path下的文件路径</param>
        /// <param name="infos"></param>
        public static void Append(string filePath, string infos)
        {
            if (!File.Exists($"{Path}/{filePath}"))
                return;
            try
            {
                using (FileStream fs = new FileStream($"{Path}/{filePath}", FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(infos);
                    }
                }
            }
            catch { }
        }
        public static void ClearFile(string filePath)
        {
            using (FileStream fs = new FileStream($"{Path}/{filePath}", FileMode.Create, FileAccess.ReadWrite)) { }
        }
        static IEnumerator BuildFoldAsync(string foldername)
        {
            if (!Directory.Exists($"{Path}/{foldername}"))
            {
                DirectoryInfo info = Directory.CreateDirectory($"{Path}/{foldername}");
                yield return info;
                Log.Warning($"创建了文件夹{Path}/{foldername}");
            }
        }
        static IEnumerator BuildFoldAsync(string foldername, string path)
        {
            if (!Directory.Exists($"{path}/{foldername}"))
            {
                DirectoryInfo info = Directory.CreateDirectory($"{path}/{foldername}");
                yield return info;
                Log.Warning($"创建了文件夹{path}/{foldername}");
            }
        }
        static IEnumerator BuildFileAsync(string foldername, string filename)
        {

            yield return MonoWorld.Instance.StartCoroutine(BuildFoldAsync(foldername));
            if (!File.Exists($"{Path}/{foldername}/{filename}"))
            {
                using (FileStream fs = File.Create($"{Path}/{foldername}/{filename}"))
                {
                    yield return fs;
                    Log.Warning($"创建了文件{Path}/{foldername}/{filename}");
                }
            }
        }
        static IEnumerator BuildFileAsync(string foldername, string filename, string path)
        {
            yield return MonoWorld.Instance.StartCoroutine(BuildFoldAsync(foldername, path));
            if (!File.Exists($"{Path}/{foldername}/{filename}"))
            {
                using (FileStream fs = File.Create($"{path}/{foldername}/{filename}"))
                {
                    yield return fs;
                    Log.Warning($"创建了文件{path}/{foldername}/{filename}");
                }
            }
        }
    }
}