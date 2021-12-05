using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

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
    public static XmlDictionary<TKey, TValue> ConvertFromDictionary(Dictionary<TKey,TValue> dictionary)
    {
        var result = new XmlDictionary<TKey, TValue>();
        foreach(var kv in dictionary)
        {
            result.Add(kv.Key, kv.Value);
        }
        return result;
    }
    public static Dictionary<TKey, TValue> ConvertToDictionary(XmlDictionary<TKey,TValue> xmlDictionary)
    {
        var result = new Dictionary<TKey, TValue>();
        foreach(var kv in xmlDictionary)
        {
            result.Add(kv.Key, kv.Value);
        }
        return result;
    }
    public Dictionary<TKey, TValue> ConvertToDictionary()
    {
        var result = new Dictionary<TKey, TValue>();
        foreach(var kv in this)
        {
            result.Add(kv.Key, kv.Value);
        }
        return result;  
    }
}