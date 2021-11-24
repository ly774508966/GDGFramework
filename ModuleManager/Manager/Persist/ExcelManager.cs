using System.Linq;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using GDG.ModuleManager;
using GDG.Utils;
using Newtonsoft.Json;
using OfficeOpenXml;
using UnityEngine;

namespace GDG.ModuleManager
{
    public class ExcelManager
    {
        private static string Path { get => Application.persistentDataPath; }
        static void SetDataToSheet<T>(T data, ExcelWorksheet sheet, int startline)
        {

            string jsonStr = String.Empty;
            if (data is ICollection collection)
            {
                #region Dictionary
                if (data is IDictionary dic)
                {
                    sheet.Cells[startline, 1].Value = "Key\\Value";
                    sheet.Cells[startline, 1].AutoFitColumns();

                    var arguments = dic.GetType().GetGenericArguments();
                    var keyType = arguments[0];
                    var valueType = arguments[1];
                    FieldInfo[] valueFields = valueType.GetFields();
                    PropertyInfo[] valueProperties = valueType.GetProperties();

                    int column = 2;

                    //初始化第一行
                    foreach (var field in valueFields)
                    {
                        sheet.Cells[startline, column++].Value = field.Name;
                        sheet.Cells[startline, column - 1].AutoFitColumns();
                    }
                    foreach (var field in valueProperties)
                    {
                        sheet.Cells[startline, column++].Value = field.Name;
                        sheet.Cells[startline, column - 1].AutoFitColumns();
                    }
                    //遍历每一行
                    int line = startline + 1;
                    foreach (var key in dic.Keys)
                    {
                        column = 2;
                        jsonStr = String.Empty;
                        sheet.Cells[line, 1].Value = key.ToString();
                        sheet.Cells[line, 1].AutoFitColumns();

                        var value = dic[key];
                        var type = value.GetType();
                        FieldInfo[] vf = type.GetFields();
                        PropertyInfo[] vp = type.GetProperties();

                        //遍历字段和属性
                        foreach (var item in vf)
                        {
                            if (!GDGTools.IsBaseType(item.GetValue(value).GetType()))
                            {
                                jsonStr = JsonConvert.SerializeObject(value);
                            }
                            sheet.Cells[line, column++].Value = jsonStr == string.Empty ? item.GetValue(value) : jsonStr;
                            sheet.Cells[line, column - 1].AutoFitColumns();
                        }
                        foreach (var item in vp)
                        {
                            if (!GDGTools.IsBaseType(item.GetValue(value).GetType()))
                            {
                                jsonStr = JsonConvert.SerializeObject(value);
                            }
                            sheet.Cells[line, column++].Value = jsonStr == string.Empty ? item.GetValue(value) : jsonStr;
                            sheet.Cells[line, column - 1].AutoFitColumns();
                        }
                        line++;
                    }
                    return;
                }
                #endregion
                #region 其他
                else
                {
                    var arguments = collection.GetType().GetGenericArguments();
                    var type = arguments[0];
                    int column = 1;

                    if (!GDGTools.IsBaseType(type))
                    {
                        FieldInfo[] valueFields = type.GetFields();
                        PropertyInfo[] valueProperties = type.GetProperties();

                        //初始化第一行
                        foreach (var field in valueFields)
                        {
                            sheet.Cells[startline, column++].Value = field.Name;
                            sheet.Cells[startline, column - 1].AutoFitColumns();
                        }
                        foreach (var field in valueProperties)
                        {
                            sheet.Cells[startline, column++].Value = field.Name;
                            sheet.Cells[startline, column - 1].AutoFitColumns();
                        }
                        int line = startline + 1;
                        foreach (var obj in collection)
                        {
                            column = 1;
                            var valueType = obj.GetType();
                            FieldInfo[] vf = valueType.GetFields();
                            PropertyInfo[] vp = valueType.GetProperties();

                            //遍历字段和属性
                            foreach (var item in vf)
                            {
                                if (!GDGTools.IsBaseType(item.GetValue(obj).GetType()))
                                {
                                    jsonStr = JsonConvert.SerializeObject(obj);
                                }
                                sheet.Cells[line, column++].Value = jsonStr == string.Empty ? item.GetValue(obj) : jsonStr;
                                sheet.Cells[line, column - 1].AutoFitColumns();
                            }
                            foreach (var item in vp)
                            {
                                if (!GDGTools.IsBaseType(item.GetValue(obj).GetType()))
                                {
                                    jsonStr = JsonConvert.SerializeObject(obj);
                                }
                                sheet.Cells[line, column++].Value = jsonStr == string.Empty ? item.GetValue(obj) : jsonStr;
                                sheet.Cells[line, column - 1].AutoFitColumns();
                            }
                            line++;
                        }
                    }
                    else
                    {
                        int line = startline;
                        foreach (var obj in collection)
                        {
                            sheet.Cells[line, 1].Value = obj;
                            sheet.Cells[line, 1].AutoFitColumns();
                            line++;
                        }
                    }
                }
                #endregion
            }
            else
            {
                //反射获取类
                Type classtype = data.GetType();

                //反射获取所有字段和属性
                FieldInfo[] fields = classtype.GetFields();
                PropertyInfo[] properties = classtype.GetProperties();
                int column = 1;
                //初始化第一行
                foreach (var field in fields)
                {
                    sheet.Cells[startline, column].Value = field.Name;
                    sheet.Cells[startline, column].AutoFitColumns();

                    if (!GDGTools.IsBaseType(field.GetValue(data).GetType()))
                    {
                        jsonStr = JsonConvert.SerializeObject(field.GetValue(data));
                    }
                    sheet.Cells[startline + 1, column].Value = jsonStr == string.Empty ? field.GetValue(data) : jsonStr;
                    column++;
                }
                foreach (var property in properties)
                {
                    sheet.Cells[startline, column].Value = property.Name;
                    sheet.Cells[startline, column].AutoFitColumns();

                    if (!GDGTools.IsBaseType(property.GetValue(data).GetType()))
                    {
                        jsonStr = JsonConvert.SerializeObject(property.GetValue(data));
                    }
                    sheet.Cells[startline + 1, column].Value = jsonStr == string.Empty ? property.GetValue(data) : jsonStr;
                    column++;
                }
            }
        }
        static T GetDataFromSheet<T>(ExcelWorksheet sheet, int startline, int rowCount) where T : new()
        {
            var data = new T();
            if (data is ICollection)
            {
                if (data is IDictionary dic)
                {
                    var arguments = dic.GetType().GetGenericArguments();
                    var valueType = arguments[1];

                    for (int i = startline + 1; i <= rowCount; i++)
                    {
                        var value = Activator.CreateInstance(valueType);
                        value = LoadColumn(value, sheet, startline, i, 2, false);
                        dic.Add(sheet.Cells[i, 1].Value, value);
                    }
                }
                else
                {
                    var valueType = data.GetType().GetGenericArguments()[0];
                    var line = startline;
                    bool isBaseType = true;
                    if (!GDGTools.IsBaseType(valueType))
                    {
                        isBaseType = false;
                        line += 1;
                    }
                    for (int i = line; i <= rowCount; i++)
                    {
                        var value = Activator.CreateInstance(valueType);
                        value = LoadColumn(value, sheet, startline, i, 1, isBaseType);
                        if (data is IList list)
                            list.Add(value);
                        else if (data is Stack stack)
                            stack.Push(value);
                        else if (data is Queue queue)
                            queue.Enqueue(value);
                        else if (data is Array array)
                            array.SetValue(value, i - 1);
                    }
                }
            }
            else
            {
                var type = typeof(T);
                if (!GDGTools.IsBaseType(type))
                {
                    data = (T)LoadColumn(data, sheet, startline, startline + 1, 1, false);
                }
                else
                {
                    try
                    {
                        data = (T)sheet.Cells[startline, 1].Value;
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error($"Load Excel failed ! Error value Type , Exception: { ex.Message }");
                    }
                }
            }
            return data;
        }
        static object LoadColumn(object obj, ExcelWorksheet sheet, int startline, int row, int startColumn, bool isBaseType)
        {
            foreach (var field in obj.GetType().GetFields())
            {
                if (!isBaseType)
                {
                    for (int i = startColumn; i <= sheet.Dimension.End.Column; i++)
                    {
                        if (field.Name == sheet.Cells[startline, i].Text)
                        {
                            if (field.FieldType == typeof(int))
                                field.SetValue(obj, Convert.ToInt32(sheet.Cells[row, i].Value));
                            else if (field.FieldType == typeof(float))
                                field.SetValue(obj, Convert.ToSingle(sheet.Cells[row, i].Value));
                            else if (field.FieldType == typeof(string))
                                field.SetValue(obj, Convert.ToString(sheet.Cells[row, i].Value));
                            else if (GDGTools.IsBaseType(field.FieldType))
                                field.SetValue(obj, sheet.Cells[row, i].Value);
                            else
                                field.SetValue(obj, JsonConvert.DeserializeObject(sheet.Cells[row, i].Text, field.FieldType));
                        }
                    }
                }

            }
            foreach (var property in obj.GetType().GetProperties())
            {
                if (!isBaseType)
                {
                    for (int i = startColumn; i <= sheet.Dimension.End.Column; i++)
                    {
                        if (property.Name == sheet.Cells[startline, i].Text)
                        {
                            if (property.PropertyType == typeof(int))
                                property.SetValue(obj, Convert.ToInt32(sheet.Cells[row, i].Value));
                            else if (property.PropertyType == typeof(float))
                                property.SetValue(obj, Convert.ToSingle(sheet.Cells[row, i].Value));
                            else if (property.PropertyType == typeof(string))
                                property.SetValue(obj, Convert.ToString(sheet.Cells[row, i].Value));
                            else if (GDGTools.IsBaseType(property.PropertyType))
                                property.SetValue(obj, sheet.Cells[row, i].Value);
                            else
                                property.SetValue(obj, JsonConvert.DeserializeObject(sheet.Cells[row, i].Text, property.PropertyType));
                        }
                    }
                }
            }
            return obj;
        }
        public static void SaveData<T>(T data, string filepath, int startline = 1, int sheetIndex = 0, string sheetRename = "Sheet") where T : new()
        {
            var reg = Regex.Replace(filepath, @".xlsx", "");

            //如果不是一个完整的路径
            if (!UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.xlsx";
            }
            //打开Excel
            FileInfo info = new FileInfo(filepath);
            using (ExcelPackage excelPackage = new ExcelPackage(info))
            {
                ExcelWorksheet sheet;
                if (excelPackage.Workbook.Worksheets.Count == 0)
                {
                    sheet = excelPackage.Workbook.Worksheets.Add(sheetRename);
                    //重命名表名
                    if (string.IsNullOrEmpty(sheetRename))
                        sheet.Name = data.GetType().ToString();
                    else
                        sheet.Name = sheetRename;
                }
                else
                    sheet = excelPackage.Workbook.Worksheets[sheetIndex];

                //清空sheet
                sheet.Cells.Clear();
                SetDataToSheet<T>(data, sheet, startline);
                excelPackage.Save();
            }
        }
        public static T LoadData<T>(string filepath, int startline = 1, int sheetIndex = 0) where T : new()
        {
            var reg = Regex.Replace(filepath, @".xlsx", "");

            //如果不是一个完整的路径
            if (!UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.xlsx";
            }


            if (!File.Exists(filepath))
            {
                throw new Exception("Error file path!");
                
            }
                

            FileInfo info = new FileInfo(filepath);
            T data;
            using (ExcelPackage excelPackage = new ExcelPackage(info))
            {
                var sheet = excelPackage.Workbook.Worksheets[sheetIndex];
                if (sheet.Dimension == null)
                {
                    return default(T);
                }
                int rowcount = sheet.Dimension.End.Row - startline + 1;//几行数据
                data = GetDataFromSheet<T>(sheet, startline, rowcount);
            }
            return data;
        }
        public static string ExcelToJson(string filepath, int startline = 1, int sheetIndex = 0)
        {
            var reg = Regex.Replace(filepath, @".xlsx", "");
            //如果不是一个完整的路径
            if (!UserFileManager.IsCompletePath(filepath))
            {
                filepath = $"{Path}/{reg}.xlsx";
            }
            if (!File.Exists(filepath))
            {
                throw new Exception("Error file ath!");
            }

            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(filepath)))
            {
                bool isDic = false;
                bool isList = false;
                StringBuilder sb = new StringBuilder();

                var sheet = excelPackage.Workbook.Worksheets[sheetIndex];
                int rowcount = sheet.Dimension.End.Row - startline + 1;//几行数据
                int columncount = sheet.Dimension.End.Column;
                if (sheet.Cells[startline, 1].Text.Equals("Key\\Value"))
                {
                    isDic = true;
                }
                if(sheet.Dimension.End.Row - startline > 1)
                {
                    isList = true;
                }
                if (isDic)
                {
                    sb.Append("{");
                    for (int row = startline + 1; row <= rowcount; row++)
                    {
                        //添加key
                        sb.Append($"\"{sheet.Cells[row, 1].Text}\":{{");
                        for (int column = 2; column <= columncount; column++)
                        {
                            //添加value的键名
                            sb.Append($"\"{sheet.Cells[startline, column].Text}\":");
                            //添加value
                            if (sheet.Cells[row, column].Text.IsNumber() || sheet.Cells[row, column].Text.Contains("{{"))
                                sb.Append(sheet.Cells[row, column].Text);
                            else
                                sb.Append($"\"{sheet.Cells[row, column].Text}\"");
                            if (column != columncount)
                                sb.Append(",");
                        }
                        sb.Append("}");
                        if (row != rowcount)
                            sb.Append(",");
                    }
                    sb.Append("}");
                }
                else if(isList)
                {
                    
                    
                    sb.Append("[");
                    for (int row = startline + 1; row <= rowcount; row++)
                    {
                        if (columncount != 1)
                        {
                            //添加key
                            sb.Append("{");
                            for (int column = 1; column <= columncount; column++)
                            {
                                //添加value的键名
                                sb.Append($"\"{sheet.Cells[startline, column].Text}\":");
                                //添加value
                                if (sheet.Cells[row, column].Text.IsNumber() || sheet.Cells[row, column].Text.Contains("{{"))
                                    sb.Append(sheet.Cells[row, column].Text);
                                else
                                    sb.Append($"\"{sheet.Cells[row, column].Text}\"");
                                if (column != columncount)
                                    sb.Append(",");
                            }
                            sb.Append("}");
                            if (row != rowcount)
                                sb.Append(",");
                        }
                        else
                        {
                            if (!sheet.Cells[row, 1].Text.IsNumber() && !sheet.Cells[row, 1].Text.Contains("{{"))
                                sb.Append(sheet.Cells[row, 1].Text);
                            else
                                sb.Append($"\"{sheet.Cells[row, 1].Text}\"");
                            if (row != rowcount)
                                sb.Append(",");
                        }

                    }
                    sb.Append("]");
                }
                else
                {
                    sb.Append("{");
                    for (int column = 1; column <= columncount; column++)
                    {
                        //添加键名
                        sb.Append($"\"{sheet.Cells[startline, column].Text}\":");

                        //添加value
                        if (sheet.Cells[startline + 1, column].Text.IsNumber() || sheet.Cells[startline + 1, column].Text.Contains("{{"))
                            sb.Append(sheet.Cells[startline + 1, column].Text);
                        else
                            sb.Append($"\"{sheet.Cells[startline + 1, column].Text}\"");
                        if (column != columncount)
                            sb.Append(",");
                    }
                    sb.Append("}");
                }
                return sb.ToString();
            }
        }
        public static string JsonToExcel(string jsonStr, string filePath, int startline = 1, int sheetIndex = 0)
        {
            bool isList = false;
            bool isDic = false;
            if (JsonManager.IsList(jsonStr))
            {
                isList = true;
            }
            if (JsonManager.IsDictionary(jsonStr))
            {
                isDic = true;
            }
            //如果不是一个完整的路径
            var reg = Regex.Replace(filePath, @".xlsx", "");
            if (!UserFileManager.IsCompletePath(filePath))
            {
                filePath = $"{Path}/{reg}.xlsx";
            }
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet sheet;
                //初始化sheet
                if (excelPackage.Workbook.Worksheets.Count == 0)
                {
                    sheet = excelPackage.Workbook.Worksheets.Add("Sheet");
                }
                else
                {
                    sheet = excelPackage.Workbook.Worksheets[sheetIndex];
                }
                sheet.Cells.Clear();

                if (isList)
                {
                    var list = JsonManager.JsonStringToObjectArray(jsonStr);
                    for (int i = 0; i < list.Count; i++)
                    {
                        for (int j = 0; j < list[i].Count; j++)
                        {
                            var kv = list[i].ElementAt(j);
                            sheet.Cells[startline, j + 1].Value = kv.Key;
                            sheet.Cells[startline + i + 1, j + 1].Value = kv.Value;
                        }
                    }
                }
                else if(isDic)
                {
                    var dic = JsonManager.JsonStringToObjectDictionary(jsonStr);
                    sheet.Cells[startline, 1].Value = "Key\\Value";
                    for (int i = 0; i < dic.Count; i++)
                    {
                        var pkv = dic.ElementAt(i);
                        sheet.Cells[startline + i + 1, 1].Value = pkv.Key;
                        for (int j = 0; j < pkv.Value.Count; j++)
                        {
                            var kv = pkv.Value.ElementAt(j);
                            sheet.Cells[startline, j + 2].Value = kv.Key;
                            sheet.Cells[startline + i + 1, j + 2].Value = kv.Value;
                        }
                    }
                }
                else
                {
                    var objDic = JsonManager.JsonStringToObject(jsonStr);
                    for (int i = 0; i < objDic.Count; i++)
                    {
                        var kv = objDic.ElementAt(i);
                        sheet.Cells[startline, i + 1].Value = kv.Key;
                        sheet.Cells[startline + 1, i + 1].Value = kv.Value;
                    }
                }
                excelPackage.Save();
            }
            return filePath;
        }
        
    }
}