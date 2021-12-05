using System;
using System.Collections.Generic;
using System.Reflection;
using GDG.ModuleManager;
using GDG.Utils;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
namespace GDG.UI
{
    #region Style
    [Serializable]
    public struct TextStyle
    {
        [SerializeField]
        public Font font;
        [SerializeField]
        public FontStyle fontStyle;
        [SerializeField]
        public int fontSize;
        [SerializeField]
        public float lineSpacing;
    }
    [Serializable]
    public struct TMPTextStyle
    {
        [SerializeField]
        public TMP_FontAsset fontAsset;
        [SerializeField]
        public FontStyles fontStyle;
        [SerializeField]
        public int fontSize;
        [SerializeField]
        public float characterSpacing;
		[SerializeField]
		public float wordSpacing;
        [SerializeField]
        public float lineSpacing;
        [SerializeField]
        public float paragraphSpacing;


    }
    [Serializable]
    public struct ImageStyle
    {
        [SerializeField]
        public Sprite sprite;
    }
    #endregion
    public abstract class LanguageDictionary<TValue> : SerializableDictionary<Language, TValue>{}
	[Serializable]
    public class LanguageTextDictionary : LanguageDictionary<TextStyle> { }
	[Serializable]
    public class LanguageTMPTextDictionary : LanguageDictionary<TMPTextStyle> { }
	[Serializable]
    public class LanguageImageTextDictionary : LanguageDictionary<ImageStyle> { }


}
