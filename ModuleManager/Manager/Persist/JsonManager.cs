using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;
using LitJson;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Converters;
using GDG.Utils;
using GDGLogger = GDG.Utils.GDGLogger;

namespace GDG.ModuleManager
{
    public enum JsonType
    {
        JsonUtlity,
        LitJson,
        JsonNet
    }
    /// <summary>
    /// 采用json的数据持久化管理工具<para/>
    /// 默认存储路径路径:Application.persistentDataPath<para/>
    /// </summary>
    public class JsonManager
    {
        private static string Path { get => Application.persistentDataPath; }

        public static void SaveData<T>(T data, string filepath, JsonType jsonType = JsonType.JsonNet) where T : class
        {
            var reg = Regex.Replace(filepath, @".json", "");

            //如果不是一个完整的路径
            if (!UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.json";
            }

            string jsonStr = string.Empty;

            switch (jsonType)
            {
                case JsonType.JsonUtlity: jsonStr = JsonUtility.ToJson(data); break;
                case JsonType.LitJson: jsonStr = JsonMapper.ToJson(data); break;
                case JsonType.JsonNet: jsonStr = JsonConvert.SerializeObject(data); break;
            }

            using (StreamWriter writer = new StreamWriter(filepath))
            {
                writer.Write(jsonStr);
            }
        }
        public static T LoadData<T>(string filepath, JsonType jsonType = JsonType.JsonNet) where T : class
        {
            var reg = Regex.Replace(filepath, @".json", "");

            //如果不是一个完整的路径
            if (!UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.json";
            }

            if (!File.Exists(filepath))
            {
                return Activator.CreateInstance<T>();
            }

            string jsonStr = string.Empty;

            using (StreamReader reader = new StreamReader(filepath))
            {
                jsonStr = reader.ReadToEnd();
            }

            switch (jsonType)
            {
                case JsonType.JsonUtlity: return JsonUtility.FromJson<T>(jsonStr);
                case JsonType.LitJson: return JsonMapper.ToObject<T>(jsonStr);
                case JsonType.JsonNet: return JsonConvert.DeserializeObject<T>(jsonStr);
            }
            return Activator.CreateInstance<T>();
        }

        public static List<Dictionary<string, object>> JsonReader(string filepath)
        {
            var reg = Regex.Replace(filepath, @".json", "");

            //如果不是一个完整的路径
            if (!UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.json";
            }

            if (!File.Exists(filepath))
            {
                return Activator.CreateInstance<List<Dictionary<string, object>>>();
            }

            string jsonStr = string.Empty;

            using (StreamReader reader = new StreamReader(filepath))
            {
                jsonStr = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonStr);
        }

        public static void JsonWriter(List<Dictionary<string, object>> data, string filepath)
        {
            var reg = Regex.Replace(filepath, @".json", "");

            //如果不是一个完整的路径
            if (!UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.json";
            }

            string jsonStr = string.Empty;

            jsonStr = JsonConvert.SerializeObject(data);

            using (StreamWriter writer = new StreamWriter(filepath))
            {
                writer.Write(jsonStr);
            }

            GDGLogger.LogSuccess($"Json file is completed, path: {filepath}");
        }

    }
    /// <summary>
    /// 通用时间特性，用法：[JsonConverter(typeof(UniversalDateTimeConverter))]
    /// </summary>
    public class UniversalDateTimeConverter : DateTimeConverterBase
    {
        private static IsoDateTimeConverter isoConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return isoConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, JsonSerializer serializer)
        {
            isoConverter.WriteJson(writer, value, serializer);
        }
    }
}