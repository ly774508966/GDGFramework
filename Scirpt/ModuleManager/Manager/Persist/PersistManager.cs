using System.Collections;
using System.Collections.Generic;
using GDG.Base;
using GDG.ModuleManager;
using UnityEngine;

public class PersistManager : LazySingleton<PersistManager>
{
        /// <summary>
        /// 
        /// </summary>
        ///<param name="filepath">filename可以是Application.persistentDataPath下的一个文件（可以不带后缀），或是一个自定义的完整的json路径（必须带后缀）</param>
        ///<param name="jsonType">使用的json库</param>
        public T LoadData_Json<T>(string filepath, JsonType jsonType = JsonType.JsonNet) where T : class
        => JsonManager.LoadData<T>(filepath, jsonType);
        /// <summary>
        /// 
        /// </summary>
        ///<param name="data">储存的实例对象</param>
        ///<param name="filepath">filename可以是Application.persistentDataPath下的一个文件（可以不带后缀），或是一个自定义的完整的json路径（必须带后缀）</param>
        ///<param name="jsonType">使用的json库</param>
        public void SaveData_Json<T>(T data, string filepath, JsonType jsonType = JsonType.JsonNet) where T : class
        => JsonManager.SaveData(data, filepath, jsonType);
        /// <summary>
        /// 
        /// </summary>
        ///<param name="filepath">filename可以是Application.persistentDataPath下的一个文件（可以不带后缀），或是一个自定义的完整的xml路径（必须带后缀）</param>
        public T LoadData_Xml<T>(string filepath) where T : class, new()
        => XmlManager.LoadData<T>(filepath);
        /// <summary>
        /// 
        /// </summary>
        ///<param name="data">储存的实例对象</param>
        ///<param name="filepath">filename可以是Application.persistentDataPath下的一个文件（可以不带后缀），或是一个自定义的完整的xml路径（必须带后缀）</param>
        public void SaveData_Xml<T>(T data, string filepath) where T : class, new()
        => XmlManager.SaveData<T>(data, filepath);
        /// <summary>
        /// 从磁盘或内存中读取实例数据<para />
        /// 暂时支持的类型：<para />
        /// int<para />
        /// flaot<para />
        /// double(可能会丢失精度)<para />
        /// bool<para />
        /// IList<para />
        /// IDictionary<para />
        /// Stack<para />
        /// Queue<para />
        /// <para />
        /// 若需要检测自定义类型的成员字段和属性，请开启自动检测<para />
        /// 注意：<para />
        /// 开启自动检测时，自定义的类型中字段和属性必须是以上的类型<para />
        /// 若传入非以上类型，可能将导致无限递归
        /// </summary>
        /// <param name="tag">自定义的键名前缀</param>
        /// <param name="autocheck">是否开启自动检测自定义类型，默认关闭</param>
        public T LoadData_PlayerPrefs<T>(string tag, bool autocheck = false) where T : class, new()
        => PlayerPrefsManager.LoadData<T>(tag, autocheck);
        /// <summary>
        /// 存储data中所有字段和属性数据到磁盘<para />
        /// 存储后的键名形式: tag_data类型_字段类型_字段名<para />
        /// 暂时支持的存储类型：<para />
        /// int<para />
        /// flaot<para />
        /// double(可能会丢失精度)<para />
        /// bool<para />
        /// IList<para />
        /// IDictionary<para />
        /// Stack<para />
        /// Queue<para />
        /// <para />
        /// 若需要检测自定义类型的成员字段和属性，请开启自动检测<para />
        /// 注意：<para />
        /// 开启自动检测时，自定义的类型中字段和属性必须是以上的类型<para />
        /// 若传入非以上类型，可能将导致无限递归
        /// </summary>
        /// <param name="data">储存的实例对象</param>
        /// <param name="tag">自定义的键名前缀</param>
        /// <param name="autocheck">是否开启自动检测自定义类型，默认关闭</param>
        public void SaveData_PlayerPrefs<T>(string tag, T data, bool autocheck = false) where T : class
        => PlayerPrefsManager.SaveData<T>(tag, data, autocheck);

        /// <summary>
        /// 从Excel中反序列化，仅支持xlsx，数据集合类型仅支持IList、Stack、Queue、System.Array
        /// </summary>
        /// <param name="filepath">filename可以是Application.persistentDataPath下的一个文件（可以不带后缀），或是一个自定义的完整的xlsx路径（必须带后缀）/param>
        /// <param name="startline">开始行（包括表头）</param>
        /// <param name="sheetIndex">Sheet序号</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadData_Excel<T>(string filepath, int startline = 1, int sheetIndex = 0) where T : new()
        => ExcelManager.LoadData<T>(filepath, startline, sheetIndex);

        /// <summary>
        /// 从Excel中序列化,仅支持xlsx
        /// </summary>
        /// <param name="data">储存的实例对象</param>
        /// <param name="filepath">filename可以是Application.persistentDataPath下的一个文件（可以不带后缀），或是一个自定义的完整的xlsx路径（必须带后缀）</param>
        /// <param name="startline">开始行（包括表头）</param>
        /// <param name="sheetIndex">Sheet序号</param>
        /// <typeparam name="T"></typeparam>
        public void SaveData_Excel<T>(T data, string filepath, int startline = 1, int sheetIndex = 0)where T:new()
        => ExcelManager.SaveData<T>(data, filepath, startline, sheetIndex);

        /// <summary>
        /// 支持txt和无后缀文件的二进制文件反序列化
        /// </summary>
        /// <param name="filepath">filename可以是Application.persistentDataPath下的一个文件（可以不带后缀），或是一个自定义的完整的二进制路径（.txt或无后缀）</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
       
        public T LoadData_Binary<T>(string filepath) where T : new()
        => BinaryManager.LoadData<T>(filepath);

        /// <summary>
        /// 支持txt和无后缀文件的二进制文件序列化，对于自定义的类型（包括自定义的成员字段类型），需要加上[System.Serializable]特性
        /// </summary>
        /// <param name="data">储存的实例对象</param>
        /// <param name="filepath">filename可以是Application.persistentDataPath下的一个文件（可以不带后缀），或是一个自定义的完整的二进制路径（.txt或无后缀）</param>
        /// <typeparam name="T"></typeparam>
        public void SaveData_Binary<T>(T data, string filepath)
        => BinaryManager.SaveData<T>(data, filepath);
}
