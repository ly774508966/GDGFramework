/*
 * @Author: 关东关 
 * @Date: 2021-03-11 22:04:40 
 * @Last Modified by: 关东关
 * @Last Modified time: 2021-03-16 14:48:02
 */
using System.Reflection;
using System;
using System.Collections;
using UnityEngine;

namespace GDG.ModuleManager
{
    /// <summary>
    /// 采用PlayerPrefs的数据持久化管理工具
    /// </summary>
    public class PlayerPrefsManager
    {
        private static void SetValue(string tag, object data, bool autocheck = false)
        {
            //常规数据
            switch (data)
            {
                case int d:
                    PlayerPrefs.SetInt(tag, d); return;
                case bool d:
                    PlayerPrefs.SetInt(tag, d ? 1 : 0); return;
                case float d:
                    PlayerPrefs.SetFloat(tag, d); return;
                case double d:
                    PlayerPrefs.SetFloat(tag, (float)d); return;
                case string d:
                    PlayerPrefs.SetString(tag, d); return;
                default: break;
            }
            //集合数据
            if (data is ICollection collection)
            {
                //集合数量
                PlayerPrefs.SetInt(tag, collection.Count);

                int index = 0;
                if (data is IDictionary)
                    foreach (var obj in collection)
                    {
                        SetValue($"{tag}_key_{index}", obj);
                        SetValue($"{tag}_value_{index}", obj);
                        index++;
                    }
                else
                    foreach (var obj in collection)
                    {
                        SetValue($"{tag}{index}", obj);
                        index++;
                    }
                return;
            }
            //自定义类型成员
            if (autocheck)
                SetValue(tag, data);
        }
        private static object GetValue(string tag, Type type, bool autocheck = false)
        {
            object data = Activator.CreateInstance(type);
            //常规数据
            if (typeof(int) == type)
                return PlayerPrefs.GetInt(tag);
            if (typeof(bool) == type)
                return PlayerPrefs.GetInt(tag) == 1 ? true : false;
            if (typeof(float) == type | typeof(double) == type)
                return PlayerPrefs.GetFloat(tag);
            if (typeof(string) == type)
                return PlayerPrefs.GetString(tag, "");
            //集合数据
            if (data is ICollection collection)
            {
                //集合数量
                int count = PlayerPrefs.GetInt(tag);
                Type[] types = type.GetGenericArguments();
                switch (data)
                {
                    case IList list:
                        for (int i = 0; i < count; i++)
                        {
                            list.Add(GetValue($"{tag}{i}", types[0], autocheck));
                        }
                        return list;
                    case IDictionary dic:
                        for (int i = 0; i < count; i++)
                        {
                            dic.Add(GetValue($"{tag}_key_{i}", types[0], autocheck), GetValue($"{tag}_value_{i}", types[1], autocheck));
                        }
                        return dic;
                    case Stack stack:
                        for (int i = 0; i < count; i++)
                        {
                            stack.Push(GetValue($"{tag}{i}", types[0], autocheck));
                        }
                        return stack;
                    case Queue queue:
                        for (int i = 0; i < count; i++)
                        {
                            queue.Enqueue(GetValue($"{tag}{i}", types[0], autocheck));
                        }
                        return queue;
                }
            }
            //自定义类型成员
            if (autocheck)
                return GetValue(tag, type, autocheck);
            return null;
        }
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
        /// <param name="data">是否开启自动检测自定义类型，默认关闭</param>
        /// <typeparam name="T">实例的类型</typeparam>
        public static void SaveData<T>(string tag, T data, bool autocheck = false) where T : class
        {
            //反射获取类
            Type classtype = data.GetType();

            //反射获取所有字段
            FieldInfo[] fieldtypes = classtype.GetFields();
            //遍历存储字段
            foreach (var info in fieldtypes)
            {
                SetValue($"{tag}_{data.GetType().Name}_{info.GetType().Name}_{info.Name}", info.GetValue(data), autocheck);
            }

            //反射获取所有属性
            PropertyInfo[] properties = classtype.GetProperties();
            //遍历存储属性
            foreach (var info in properties)
            {
                SetValue($"{tag}_{data.GetType().Name}_{info.GetType().Name}_{info.Name}", info.GetValue(data), autocheck);
            }

            //保存到磁盘
            PlayerPrefs.Save();
        }
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
        /// <param name="data">是否开启自动检测自定义类型，默认关闭</param>
        /// <typeparam name="T">实例的类型</typeparam>
        public static T LoadData<T>(string tag, bool autocheck = false) where T : class, new()
        {
            T obj = new T();

            //反射获取类
            Type classtype = obj.GetType();

            //反射获取所有字段
            FieldInfo[] fieldtypes = classtype.GetFields();
            //遍历存储字段
            foreach (var info in fieldtypes)
            {
                info.SetValue(obj, GetValue($"{tag}_{obj.GetType().Name}_{info.GetType().Name}_{info.Name}", info.GetType(), autocheck));
            }

            //反射获取所有属性
            PropertyInfo[] properties = classtype.GetProperties();
            //遍历存储属性
            foreach (var info in properties)
            {
                info.SetValue(obj, GetValue($"{tag}_{obj.GetType().Name}_{info.GetType().Name}_{info.Name}", info.GetType(), autocheck));
            }
            return obj;
        }
    }
}