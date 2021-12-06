using System.Net.Mime;
using System.IO;
using System;
using System.Collections.Generic;
using GDG.Utils;
using UnityEngine;

namespace GDG.Base
{
    public class EditorPath
    {
        public static string AssetsPath{ get => $"{System.Environment.CurrentDirectory}\\Assets"; }
    }
}