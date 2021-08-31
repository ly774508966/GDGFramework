using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using OfficeOpenXml;
using System.Reflection;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using GDGLogger = GDG.Utils.GDGLogger;

namespace GDG.ModuleManager
{
#pragma warning disable 0168
    public class ExcelManager
    {
        private static string Path { get => Application.persistentDataPath; }
        private static object TryToInt(object obj)
        {
            double num;
            if (double.TryParse(obj.ToString(), out num))
            {
                if ((int)num > (double)num)
                {
                    return (double)num;
                }
                return (int)num;
            }
            else if (obj is string str)
            {
                try
                {
                    var jsonList = JsonConvert.DeserializeObject<object[]>(obj.ToString());
                    return jsonList;
                }
                catch (Exception ex)
                {
                    try
                    {
                        var jsonObj = JsonConvert.DeserializeObject<object>(obj.ToString());
                        return jsonObj;
                    }
                    catch (Exception ex2) { }
                }
            }
            return obj;
        }
        static void GetData<T>(T obj, ExcelWorksheet sheet, int startline, int rowIndex, bool isForeachPropert = true)
        {
            //反射获取类
            Type classtype = obj.GetType();

            //反射获取所有字段和属性
            FieldInfo[] fields = classtype.GetFields();
            PropertyInfo[] properties = classtype.GetProperties();

            string jsonStr = String.Empty;

            if (obj is Dictionary<string, object> dic)
            {
                int column = 1;
                foreach (var item in dic)
                {
                    jsonStr = String.Empty;
                    sheet.Cells[startline, column].Value = item.Key;
                    if (item.Value.GetType() != typeof(int) &&
                        item.Value.GetType() != typeof(float) &&
                        item.Value.GetType() != typeof(double) &&
                        item.Value.GetType() != typeof(string))
                    {
                        jsonStr = JsonConvert.SerializeObject(item.Value);
                    }
                    sheet.Cells[startline + rowIndex, column].Value = jsonStr == string.Empty ? item.Value : jsonStr;
                    column++;
                }
                return;
            }

            //遍历字段
            for (int i = 0; i < fields.Length; i++)
            {

                jsonStr = String.Empty;

                sheet.Cells[startline, i + 1].Value = fields[i].Name;

                if (fields[i].FieldType != typeof(int) &&
                    fields[i].FieldType != typeof(float) &&
                    fields[i].FieldType != typeof(double) &&
                    fields[i].FieldType != typeof(string))
                {
                    jsonStr = JsonConvert.SerializeObject(fields[i].GetValue(obj));
                }
                sheet.Cells[startline + rowIndex, i + 1].Value = jsonStr == string.Empty ? fields[i].GetValue(obj) : jsonStr;
            }

            //遍历属性
            if (isForeachPropert)
                for (int i = 0; i < properties.Length; i++)
                {
                    jsonStr = String.Empty;

                    sheet.Cells[startline, fields.Length + i + 1].Value = properties[i].Name;

                    if (properties[i].PropertyType != typeof(int) &&
                        properties[i].PropertyType != typeof(float) &&
                        properties[i].PropertyType != typeof(double) &&
                        properties[i].PropertyType != typeof(string))
                    {
                        jsonStr = JsonConvert.SerializeObject(properties[i].GetValue(obj));
                    }
                    sheet.Cells[startline + rowIndex, fields.Length + i + 1].Value = jsonStr == string.Empty ? properties[i].GetValue(obj) : jsonStr;
                }
        }
        static void SetData<T>(ref T obj, ExcelWorksheet sheet, int startline, int index)
        {
            foreach (var field in obj.GetType().GetFields())
            {
                for (int i = 1; i <= sheet.Dimension.End.Column; i++)
                {
                    if (field.Name == sheet.Cells[startline, i].Text)
                    {
                        if (field.FieldType == typeof(int))
                            field.SetValue(obj, Convert.ToInt32(sheet.Cells[startline + index, i].Value));
                        else if (field.FieldType == typeof(float))
                            field.SetValue(obj, Convert.ToSingle(sheet.Cells[startline + index, i].Value));
                        else if (field.FieldType == typeof(string) || field.FieldType == typeof(double))
                            field.SetValue(obj, sheet.Cells[startline + index, i].Value);
                        else
                            field.SetValue(obj, JsonConvert.DeserializeObject(sheet.Cells[startline + index, i].Text, field.FieldType));
                        break;
                    }
                }
            }
            foreach (var property in obj.GetType().GetProperties())
            {
                for (int i = 1; i <= sheet.Dimension.End.Column; i++)
                {
                    if (property.Name == sheet.Cells[startline, i].Text)
                    {
                        if (property.PropertyType == typeof(int))
                            property.SetValue(obj, Convert.ToInt32(sheet.Cells[startline + index, i].Value));
                        else if (property.PropertyType == typeof(float))
                            property.SetValue(obj, Convert.ToSingle(sheet.Cells[startline + index, i].Value));
                        else if (property.PropertyType == typeof(string) || property.PropertyType == typeof(double))
                            property.SetValue(obj, sheet.Cells[startline + index, i].Value);
                        else
                            property.SetValue(obj, JsonConvert.DeserializeObject(sheet.Cells[startline + index, i].Text, property.PropertyType));
                        break;
                    }

                }
            }
        }
        /// <summary>
        /// 从Excel中序列化,仅支持xlsx
        /// </summary>
        /// <param name="data">存储的数据实例</param>
        /// <param name="filepath">filename可以是Application.persistentDataPath下的一个文件（可以不带后缀），或是一个自定义的完整的xlsx路径（必须带后缀）</param>
        /// <param name="startline">开始行（包括表头）</param>
        /// <param name="sheetIndex">Sheet序号</param>
        /// <typeparam name="T"></typeparam>
        public static void SaveData<T>(T data, string filepath, int startline = 1, int sheetIndex = 0)
        {
            var reg = Regex.Replace(filepath, @".xlsx", "");

            //如果不是一个完整的路径
            if (!UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.xlsx";
            }

            int rowIndex = 1;

            //打开Excel
            FileInfo info = new FileInfo(filepath);
            using (ExcelPackage excelPackage = new ExcelPackage(info))
            {
                ExcelWorksheet sheet;
                if (excelPackage.Workbook.Worksheets.Count == 0)
                    sheet = excelPackage.Workbook.Worksheets.Add("sheet1");
                else
                    sheet = excelPackage.Workbook.Worksheets[sheetIndex];

                //清空sheet
                sheet.Cells.Clear();
                //重命名表名
                sheet.Name = data.GetType().Name;

                if (data is ICollection datas)
                {
                    foreach (var obj in datas)
                    {
                        GetData(obj, sheet, startline, rowIndex);
                        rowIndex++;
                    }
                }
                else
                {
                    GetData<T>(data, sheet, startline, 1);
                }

                excelPackage.Save();
            }
        }
        /// <summary>
        /// 从Excel中反序列化，仅支持xlsx，数据集合类型仅支持IList、Stack、Queue、System.Array，不支持数组[]
        /// </summary>
        /// <param name="filepath">filename可以是Application.persistentDataPath下的一个文件（可以不带后缀），或是一个自定义的完整的xlsx路径（必须带后缀）/param>
        /// <param name="startline">开始行（包括表头）</param>
        /// <param name="sheetIndex">Sheet序号</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadData<T>(string filepath, int startline = 1, int sheetIndex = 0) where T : new()
        {
            var reg = Regex.Replace(filepath, @".xlsx", "");

            //如果不是一个完整的路径
            if (!UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.xlsx";
            }

            T data = new T();

            if (!File.Exists(filepath))
                return data;

            FileInfo info = new FileInfo(filepath);
            using (ExcelPackage excelPackage = new ExcelPackage(info))
            {
                var sheet = excelPackage.Workbook.Worksheets[sheetIndex];

                var count = sheet.Dimension.End.Row - startline;//几条数据
                int rows = startline + sheet.Dimension.End.Row;
                int columns = sheet.Dimension.End.Column;

                if (data is ICollection)
                {
                    if (data is IList list)
                    {
                        for (int index = 1; index <= count; index++)
                        {
                            var obj = Activator.CreateInstance(data.GetType().GetGenericArguments()[0]);
                            SetData(ref obj, sheet, startline, index);
                            list.Add(obj);
                        }
                    }
                    else if (data is Stack stack)
                    {
                        for (int index = 1; index <= count; index++)
                        {
                            var obj = Activator.CreateInstance(data.GetType().GetGenericArguments()[0]);
                            SetData(ref obj, sheet, startline, index);
                            stack.Push(obj);
                        }
                    }
                    else if (data is Queue queue)
                    {
                        for (int index = 1; index <= count; index++)
                        {
                            var obj = Activator.CreateInstance(data.GetType().GetGenericArguments()[0]);
                            SetData(ref obj, sheet, startline, index);
                            queue.Enqueue(obj);
                        }
                    }
                    else if (data is Array array)
                    {
                        for (int index = 1; index <= count; index++)
                        {
                            var obj = Activator.CreateInstance(data.GetType().GetGenericArguments()[0]);
                            SetData(ref obj, sheet, startline, index);
                            array.SetValue(obj, index - 1);
                        }
                    }
                }
                else
                {
                    SetData<T>(ref data, sheet, startline, 1);
                }
            }

            return data;
        }

        public static List<Dictionary<string, object>> ExcelReader(string filepath, int startline = 1, int sheetIndex = 0)
        {
            var reg = Regex.Replace(filepath, @".xlsx", "");

            //如果不是一个完整的路径
            if (!UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.xlsx";
            }

            if (!File.Exists(filepath))
                throw new CustomErrorException($"Cant'Find File , Path: '{filepath}'");

            List<Dictionary<string, object>> collectList = new List<Dictionary<string, object>>();

            FileInfo info = new FileInfo(filepath);
            using (ExcelPackage excelPackage = new ExcelPackage(info))
            {
                var sheet = excelPackage.Workbook.Worksheets[sheetIndex];
                var rows = sheet.Dimension.End.Row;
                int columns = sheet.Dimension.End.Column;

                for (int i = startline + 1; i <= rows; i++)
                {
                    Dictionary<string, object> contentDic = new Dictionary<string, object>();
                    for (int j = 1; j <= columns; j++)
                    {
                        var id = sheet.Cells[startline, j].Text;
                        var value = TryToInt(sheet.Cells[i, j].Value);
                        contentDic.Add(id, value);
                    }
                    collectList.Add(contentDic);
                }
            }
            return collectList;
        }
        public static void ExcelWriter(List<Dictionary<string, object>> data, string filepath, int startline = 1, int sheetIndex = 0)
        {
            var reg = Regex.Replace(filepath, @".xlsx", "");

            //如果不是一个完整的路径
            if (!UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.xlsx";
            }

            int rowIndex = 1;

            //打开Excel
            FileInfo info = new FileInfo(filepath);
            using (ExcelPackage excelPackage = new ExcelPackage(info))
            {
                ExcelWorksheet sheet;
                if (excelPackage.Workbook.Worksheets.Count == 0)
                    sheet = excelPackage.Workbook.Worksheets.Add("sheet1");
                else
                    sheet = excelPackage.Workbook.Worksheets[sheetIndex];

                //清空sheet
                sheet.Cells.Clear();
                //重命名表名
                sheet.Name = data.GetType().Name;

                foreach (var obj in data)
                {
                    GetData<Dictionary<string, object>>(obj, sheet, startline, rowIndex, false);
                    rowIndex++;
                }

                excelPackage.Save();
            }
            GDGLogger.LogSuccess($"Excel file is completed, path: {filepath}");
        }

    }
    #pragma warning restore 0168
}