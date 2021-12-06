using System;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.Base
{
    public class GlobalMethod
    {
        public static bool IsBlittable(Type type)
        {
            if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) ||
               type == typeof(ushort) || type == typeof(int) || type == typeof(uint) ||
               type == typeof(long) || type == typeof(ulong) || type == typeof(Single) || type == typeof(double))
                return true;
            return false;
        }
        public static bool IsBaseType(Type type)
        {
            if (IsBlittable(type))
            {
                return true;
            }
            else
            {
                if (type == typeof(string) || type == typeof(char) || type == typeof(bool) || type.IsEnum || type == typeof(Vector3))
                {
                    return true;
                }
                return false;
            }
        }
        public static bool IsBlittable<T>()
        {
            return IsBlittable(typeof(T));
        }
        public static bool IsBaseType<T>()
        {
            return IsBaseType(typeof(T));
        }

    }
}