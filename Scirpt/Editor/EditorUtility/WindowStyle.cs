using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDG.Editor
{
    public class WindowStyles
    {
        public static readonly GUIStyle DarkBackground = CreateSolidColorStyle(new Color(0.1f, 0.1f, 0.1f));
        public static readonly GUIStyle GrayBackground = CreateSolidColorStyle(new Color(0.15f, 0.15f, 0.15f));
        public static readonly GUIStyle LightBackground = CreateSolidColorStyle(new Color(0.2f, 0.2f, 0.2f));
        public static readonly GUIStyle None = CreateSolidColorStyle(new Color(0f, 0f, 0f, 0f));
        public static GUIStyle CreateSolidColorStyle(Color color)
        {
            Texture2D texture = new Texture2D(2, 2);
            for (int y = 0; y < texture.height; ++y)
            {
                for (int x = 0; x < texture.width; ++x)
                {
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();

            return new GUIStyle { normal = { background = texture } };
        }
    }

}