using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GDG.Utils
{
    public static class CollectionExtension
    {
        public static string ToString<T>(this T[] array)
        {
            if (array.Length == 0)
                return "null";
            if (array.Length == 1)
                return $"[{array[0]}]";

            return "[" + string.Join(", ", array) + "]";
        }
    }
}