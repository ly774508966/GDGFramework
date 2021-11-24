using System.Diagnostics;
using System.Text.RegularExpressions;
using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Schema;
using System.Xml;
using System.Collections;
using Newtonsoft.Json;
using GDG.Utils;
using Log = GDG.Utils.Log;
/*
* @Author: 关东关 
* @Date: 2021-03-16 12:24:25 
* @Last Modified by: 关东关
* @Last Modified time: 2021-05-22 21:59:57
*/

namespace GDG.ModuleManager
{
    /// <summary>
    /// 采用xml的数据持久化管理工具<para/>
    /// 不支持Dictionary的数据持久化,若需要使用字典，应使用：XmlDictionary< TKey, TValue ><para/>
    /// 默认路径：Application.persistentDataPath<para/>
    /// </summary>
    public class XmlManager
    {
        public static object UserFileMgr { get; private set; }
        private static string Path { get => Application.persistentDataPath; }

        /// <summary>
        /// filename可以是Application.persistentDataPath下的一个文件（可以不带后缀），或是一个自定义的完整的xml路径（必须带后缀）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T LoadData<T>(string filepath) where T : class, new()
        {
            //如果文件不存在则返回一个新实例
            var reg = Regex.Replace(filepath, @".xml", "");

            //如果是一个完整的路径
            if (UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.xml";
            }
            if (!File.Exists(filepath))
            {
                throw new Exception("Error file ath!");
            }
            using (StreamReader reader = new StreamReader(filepath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(reader) as T;
            }
        }

        /// <summary>
        /// filename可以是Application.persistentDataPath下的一个文件（可以不带后缀），或是一个自定义的完整的xml路径（必须带后缀）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void SaveData<T>(T data, string filepath) where T : class, new()
        {
            var reg = Regex.Replace(filepath, @".xml", "");

            if (!UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.xml";
            }

            using (StreamWriter writer = new StreamWriter(filepath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, data);
            }
        }
        public static string JsonToXml(string jsonStr,string filepath,string ArrayName = "ArrayData")
        {
            string root = "\"root\":";
            bool isList = JsonManager.IsList(jsonStr);
            //如果文件不存在则返回一个新实例
            var reg = Regex.Replace(filepath, @".xml", "");
            //如果是一个完整的路径
            if (UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.xml";
            }

            if(isList)
            {
                root = "\"ArrayOfUnitData\":{\"@xmlns:xsd\":\"http://www.w3.org/2001/XMLSchema\",\"@xmlns:xsi\":\"http://www.w3.org/2001/XMLSchema-instance\",\""+ ArrayName +"\":" ;
                jsonStr = "{\"?xml\":{\"@version\":\"1.0\",\"@standalone\":\"no\"}," + root + jsonStr + "}}";
            }
            else
                jsonStr = "{\"?xml\":{\"@version\":\"1.0\",\"@standalone\":\"no\"}," + root + jsonStr + "}";
            
            using (StreamWriter writer = new StreamWriter(filepath))
            {
                var doc = JsonConvert.DeserializeXmlNode(jsonStr);
                writer.Write(doc.OuterXml);
            }
            return filepath;
        }
        public static string XmlToJson(string filepath)
        {
            string jsonStr = "";

            //如果文件不存在则返回一个新实例
            var reg = Regex.Replace(filepath, @".xml", "");

            //如果是一个完整的路径
            if (UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.xml";
            }
            if (!File.Exists(filepath))
            {
                throw new Exception("Error file ath!");
            }

            using (StreamReader reader = new StreamReader(filepath))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(reader.ReadToEnd());
                
                jsonStr = JsonConvert.SerializeXmlNode(doc);

                bool isArray = Regex.IsMatch(jsonStr, "\"ArrayOfUnitData\"");
                if(isArray)
                {
                    jsonStr = Regex.Replace(jsonStr, "\"ArrayOfUnitData\".*?:\\[" , "[");
                    jsonStr = jsonStr.Substring(0, jsonStr.Length - 2);
                }
                else
                    jsonStr = jsonStr.Substring(0, jsonStr.Length - 1);
                
                //去除root和xml信息
                jsonStr = Regex.Replace(jsonStr, "{?\"\\?xml\":.*?}," , "");
                jsonStr = Regex.Replace(jsonStr, "\"root\":", "");

                //格式化，把是数字和布尔的部分去除双引号
                string pattern = ":\"((\\d+(.\\d+)?)|(true)|(false))\"";
                string replacement = ":$1";
                jsonStr = Regex.Replace(jsonStr, pattern,replacement);
            }
            return jsonStr;
        }
    }

    /// <summary>
    /// 支持xml序列化的字典
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class XmlDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        //反序列化时
        public void ReadXml(XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            //跳过父节点
            reader.Read();

            //如果不是</>节点则持续反序列化
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                keySerializer.Deserialize(reader);
                valueSerializer.Deserialize(reader);
            }
        }
        //序列化时
        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            //序列化时遍历本类字典
            foreach (KeyValuePair<TKey, TValue> kv in this)
            {
                //分别对键值序列化
                keySerializer.Serialize(writer, kv.Key);
                valueSerializer.Serialize(writer, kv.Value);
            }
        }
    }
}