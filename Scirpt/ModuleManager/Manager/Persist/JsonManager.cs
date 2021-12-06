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
using System.Linq;
using Log = GDG.Utils.Log;
using JsonToken = Newtonsoft.Json.JsonToken;

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
            if (!filepath.IsFormatPath())
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
            if (!filepath.IsFormatPath())
            {
                filepath = $"{Path}/{reg}.json";
            }
            if (!File.Exists(filepath))
            {
                throw new Exception($"Error file path!:{filepath}");
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
            return default(T);
        }
        public static string WriteJson(string filepath, string jsonStr)
        {
            var reg = Regex.Replace(filepath, @".json", "");

            //如果不是一个完整的路径
            if (!filepath.IsFormatPath())
            {
                filepath = $"{Path}/{reg}.json";
            }

            using (StreamWriter writer = new StreamWriter(filepath))
            {
                writer.Write(jsonStr);
            }
            return filepath;
        }
        public static string ReadJson(string filepath)
        {
            var reg = Regex.Replace(filepath, @".json", "");

            //如果不是一个完整的路径
            if (!filepath.IsFormatPath())
            {
                filepath = $"{Path}/{reg}.json";
            }

            if (!File.Exists(filepath))
            {
                Log.Error($"There is no JSON file in this path. Path :{filepath}");
                return "";
            }

            string jsonStr = string.Empty;

            using (StreamReader reader = new StreamReader(filepath))
            {
                jsonStr = reader.ReadToEnd();
            }
            return jsonStr;
        }
        /// <summary>
        /// 用于解析class类型数组类型的Json，返回一个字典数组，键值对名分别为对象名称、值
        /// </summary>
        public static List<Dictionary<string,string>> JsonStringToObjectArray(string jsonStr)
        {
            var objectArray = new List<Dictionary<string, string>>();
            JsonTextReader reader = new JsonTextReader(new StringReader(jsonStr));

            List<string> result = new List<string>();
            StringBuilder sb = new StringBuilder();
            int dicIndex = -1;
            string currentPropertyName = null;

            while (reader.Read())
            {
                if (reader.Value != null) // 有内容
                {
                    if(reader.TokenType == JsonToken.PropertyName)
                    {
                        var objDic = objectArray[dicIndex];
                        currentPropertyName = reader.Value.ToString();
                        objDic.Add(currentPropertyName, "");
                        continue;
                    }
                    if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Boolean
                        || reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Null)
                    {
                        if(currentPropertyName!=null)
                        {                            
                            objectArray[dicIndex][currentPropertyName] = reader.Value.ToString();
                        }
                    }
                }
                else
                {
                    //对象开始
                    if(reader.TokenType==JsonToken.StartObject)
                    {
                        var objDic = new Dictionary<string, string>();
                        objectArray.Add(objDic);
                        dicIndex++;
                        continue;
                    }
                }
            }
            return objectArray;
        }
        /// <summary>
        /// 用于解析值类型数组类型的Json，返回一个string数组
        /// </summary>
        public static List<string> JsonStringToValueArray(string jsonStr)
        {
            List<string> result = new List<string>();
            JsonTextReader reader = new JsonTextReader(new StringReader(jsonStr));

            while (reader.Read())
            {
                if(reader.Value!=null)
                {
                    if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Boolean
                        || reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Null)
                    {
                        result.Add(reader.Value.ToString());
                    }    
                }
            }
            return result;
        }
        /// <summary>
        /// 用于解析值为class类型的字典的Json，返回一个字典，键为字典的key，值为对象字典，对象字典的键值对名分别为对象名称、值
        /// </summary>
        public static Dictionary<string,Dictionary<string,string>> JsonStringToObjectDictionary(string jsonStr)
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            var reader = new JsonTextReader(new StringReader(jsonStr));

            string currentKey = null;
            string currentPropertyName = null;
            bool isBegin = true;

            while (reader.Read())
            {
                if(reader.Value!=null)
                {
                    if(reader.TokenType == JsonToken.PropertyName)
                    {
                        var property = reader.Value.ToString();
                        if(currentKey == null)
                        {
                            currentKey = property;
                            continue;                            
                        }
                        currentPropertyName = property;
                        result[currentKey].Add(property, "");
                    }
                    else
                    {
                        result[currentKey][currentPropertyName] = reader.Value.ToString();
                    }
                }
                else
                {
                    if(isBegin)
                    {
                        isBegin = false;
                        continue;
                    }
                    if(reader.TokenType == JsonToken.StartObject)
                    {
                        result.Add(currentKey,new Dictionary<string, string>());
                    }
                    else if (reader.TokenType == JsonToken.EndObject)
                    {
                        currentKey = null;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 用于解析值为非class类型类型的字典的Json，返回一个字典，键值对名分别为对象名称、值
        /// </summary>
        public static Dictionary<string,string> JsonStringToValueDictionary(string jsonStr)
        {
            var result = new Dictionary<string, string>();
            var reader = new JsonTextReader(new StringReader(jsonStr));
            string currentKey = "";
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        currentKey = reader.Value.ToString();
                        result.Add(currentKey, "");
                    }
                    else
                    {
                        result[currentKey] = reader.Value.ToString();
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 用于解析class类型的Json（不支持成员变量为 ICollection 的解析），返回一个字典，键值对名分别为变量名称、值
        /// </summary>
        public static Dictionary<string, string> JsonStringToObject(string jsonStr)
        {
            var result = new Dictionary<string, string>();
            var reader = new JsonTextReader(new StringReader(jsonStr));
            string currentVarName = null;
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        currentVarName = reader.Value.ToString();
                        result.Add(currentVarName, null);
                        continue;
                    }
                    else
                    {
                        result[currentVarName] = reader.Value.ToString();
                    }
                }
            }
            return result;
        }
        public static bool IsDictionary(string jsonStr)
        {
            bool isList = false;
            if (jsonStr.Contains('['))
            {
                isList = true;
            }
            if (!isList)
            {
                if(Regex.IsMatch(jsonStr, "\":\\s.*{"))
                    return  true;
            }
            return false;
        }
        public static bool IsList(string jsonStr)
        {
            if (jsonStr.Contains('['))
            {
                return true;
            }
            return false;
        }
    }
    /// <summary>
    /// 通用时间特性，用法：[JsonConverter(typeof(UDTConverter))]
    /// </summary>
    public class UDTConverter : DateTimeConverterBase
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