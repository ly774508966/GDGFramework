using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ECS;
using System.Text.RegularExpressions;
using System;
using System.Linq.Expressions;
using System.Text;

public static class CsharpExtension
{
    #region ToFormatString()
    public static string ToFormatString<T>(this List<T> list)
    {
        return list.ToArray().ToFormatString();
    }
    public static string ToFormatString<T>(this T[] array)
    {
        if (array.Length == 0)
            return "null";
        if (array.Length == 1)
            return $"[{array[0]}]";

        return "[" + string.Join(", ", array) + "]";
    }
    public static string ToFormatString<K, V>(this IDictionary<K, V> dic)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in dic)
        {
            sb.Append("[" + item.Key + " ： " + item.Value + "]\n");
        }
        return sb.ToString();
    }

    #endregion
    #region 验证
    private static readonly Regex RegNumber = new Regex("^\\d+$");
    private static readonly Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$");
    private static readonly Regex RegIp = new Regex("((25[0-5]|2[0-4]\\d|1?\\d?\\d)\\.){3}(25[0-5]|2[0-4]\\d|1?\\d?\\d)");
    public static bool IsBoolean(this string str) => str.Equals("true") || str.Equals("false");
    public static bool IsNumber(this string str) => RegNumber.Match(str).Success;
    public static bool IsEmail(this string str) => RegEmail.Match(str).Success;
    public static bool IsIPAddress(this string str) => RegEmail.Match(str).Success;
    public static bool IsCompletePath(this string filepath)
    {
        filepath = Regex.Replace(filepath, @"\\", @"/");
        filepath = Regex.Replace(filepath, @"//", @"/");
        return (Regex.IsMatch(filepath, @"([A-Za-z]+:\/|\/\/)([^\/^\/:*?""<>|].*\/)*([^\^\/:*?""<>|]+)$")//Windows下
        || Regex.IsMatch(filepath, @"(.\/|\/){1}([^\/^\/:*?""<>|].*)*(\/[^\^\/:*?""<>|]+)$"));//Linux下
    }
    #endregion
    #region 类型转换
    public static int? ToInt(this object obj)
    {
        if (int.TryParse(obj.ToString(), out int value))
        {
            return value;
        }
        return null;
    }
    public static Int32? ToInt32(this object obj)
    {
        if (Int32.TryParse(obj.ToString(), out Int32 value))
        {
            return value;
        }
        return null;
    }
    public static Int64? ToInt64(this object obj)
    {
        if (Int64.TryParse(obj.ToString(), out Int64 value))
        {
            return value;
        }
        return null;
    }
    public static bool? ToBool(this object obj)
    {
        if (bool.TryParse(obj.ToString(), out bool value))
        {
            return value;
        }
        return null;
    }
    public static double? ToDouble(this object obj)
    {
        if (double.TryParse(obj.ToString(), out double value))
        {
            return value;
        }
        return null;
    }
    public static float? ToFloat(this object obj)
    {
        if (float.TryParse(obj.ToString(), out float value))
        {
            return value;
        }
        return null;
    }

    #endregion
    #region 文件
    public static string GetFileName(this string filepath, bool withExtension = true)
    {
        if (filepath.IsCompletePath())
        {
            filepath = Regex.Replace(filepath, @"\\", @"\");
            filepath = Regex.Replace(filepath, @"//", @"\");
            Match match = Regex.Match(filepath, @".+\\(.+)");
            if (withExtension)
                return match.Groups[1].Value;
            else
                return Regex.Match(match.Groups[1].Value, @"(.*)\.").Groups[1].Value;

        }
        return null;
    }
    public static string GetFileName(this string filepath, out string extension, bool withExtension = true)
    {
        extension = "";
        if (filepath.IsCompletePath())
        {
            filepath = Regex.Replace(filepath, @"\\", @"\");
            filepath = Regex.Replace(filepath, @"//", @"\");
            filepath = Regex.Replace(filepath, @"/", @"\");
            Match match = Regex.Match(filepath, @".+\\(.+)");

            var result = match.Groups[1].Value;
            extension = Regex.Match(result, @"(.*)\.(.*)").Groups[2].Value;

            if (withExtension)
                return result;
            else
                return Regex.Match(result, @"(.*)\.").Groups[1].Value;

        }
        return null;
    }
    public static string GetParentFolderPath(this string filepath)
    {
        if (filepath.IsCompletePath())
        {
            filepath = Regex.Replace(filepath, @"\\", @"\");
            filepath = Regex.Replace(filepath, @"//", @"\");
            Match match = Regex.Match(filepath, @"(.*)\\");
            return match.Groups[1].Value;
        }
        return null;
    }
    public static string GetParentFolderName(this string filepath)
    {
        if (filepath.IsCompletePath())
        {
            filepath = Regex.Replace(filepath, @"\\", @"\");
            filepath = Regex.Replace(filepath, @"//", @"\");
            Match match = Regex.Match(filepath, @"([^\\]*?)\\?[^\\]*$");

            return match.Groups[1].Value;
        }
        return null;
    }
    #endregion
}
