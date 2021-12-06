using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GDG.Utils;
using GDG.ModuleManager;
using System;

namespace GDG.Localization
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
            EventManager.Instance.AddActionListener<Language, string, string>("SetLocalizationText", SetLocalizationText);
        }
        void Start()
        {
            if (!LocalizationConfig.Instance.isInit)
            {
                LocalizationConfig.Instance.SetLanguage(LocalizationConfig.Instance.CurrentLanguage);
                LocalizationConfig.Instance.isInit = true;
            }
        }
        void OnDestroy()
        {
            EventManager.Instance.RemoveActionListener<Language, string, string>("SetLocalizationText", SetLocalizationText);
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