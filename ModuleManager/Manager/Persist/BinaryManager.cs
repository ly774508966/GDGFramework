using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System;
using GDG.Utils;
using Log = GDG.Utils.Log;

namespace GDG.ModuleManager
{
    public class BinaryManager
    {
        private static string Path { get => Application.persistentDataPath; }
        /// <summary>
        /// 支持txt和无后缀文件的二进制文件序列化，对于自定义的类型（包括自定义的成员字段类型），需要加上[System.Serializable]特性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filepath"></param>
        /// <typeparam name="T"></typeparam>
        public static void SaveData<T>(T data, string filepath)
        {
            bool isTxt = Regex.IsMatch(filepath, @".txt");
            var reg = Regex.Replace(filepath, @".txt", "");

            //如果不是一个完整的路径
            if (!filepath.IsFormatPath())
            {
                if (isTxt)
                    filepath = $"{Path}/{reg}.txt";
                else
                    filepath = $"{Path}/{reg}";
            }

            BinaryFormatter bf = new BinaryFormatter();

            using (FileStream stream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                bf.Serialize(stream, data);
            }
        }
        /// <summary>
        /// 支持txt和无后缀文件的二进制文件反序列化
        /// </summary>
        /// <param name="filepath">filename可以是Application.persistentDataPath下的一个文件（可以不带后缀），或是一个自定义的完整的二进制路径（.txt或无后缀）</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadData<T>(string filepath) where T : new()
        {
            T data = new T();
            bool isTxt = Regex.IsMatch(filepath, @".txt");
            var reg = Regex.Replace(filepath, @".txt", "");

            //如果不是一个完整的路径
            if (!filepath.IsFormatPath())
            {
                if (isTxt)
                    filepath = $"{Path}/{reg}.txt";
                else
                    filepath = $"{Path}/{reg}";
            }

            if (!File.Exists(filepath))
            {
                throw new Exception($"Error file path!:{filepath}");
            }

            BinaryFormatter bf = new BinaryFormatter();

            using (FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                data = (T)bf.Deserialize(stream);
            }
            return data;
        }
    }
}