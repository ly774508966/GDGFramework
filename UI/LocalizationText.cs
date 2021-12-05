using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GDG.Utils;
using GDG.ModuleManager;
using System;

namespace GDG.UI
{
    public class LocalizationText : MonoBehaviour
    {
        public string Key;
        private Text text;

        [SerializeField]
        public LanguageTextDictionary textStyle = new LanguageTextDictionary();

        void Awake()
        {
            this.text = GetComponent<Text>();
            GDGTools.EventCenter.AddActionListener<Language, string, string>("SetLocalizationText", SetLocalizationText);
        }
        void Start()
        {
            if (!GDGTools.LocalLanguage.isInit)
            {
                GDGTools.LocalLanguage.SetLanguage(GDGTools.LocalLanguage.CurrentLanguage);
                GDGTools.LocalLanguage.isInit = true;
            }
        }
        void OnDestroy()
        {
            GDGTools.EventCenter.RemoveActionListener<Language, string, string>("SetLocalizationText", SetLocalizationText);
        }
        void SetLocalizationText(Language language, string key, string text)
        {
            if (key != this.Key)
                return;
            if (this.text != null)
            {
                this.text.text = text;
            }
            if (textStyle.TryGetValue(language, out TextStyle style))
            {
                this.text.font = style.font;
                this.text.fontStyle = style.fontStyle;
                this.text.fontSize = style.fontSize;
                this.text.lineSpacing = style.lineSpacing;
            }
        }
    }
}