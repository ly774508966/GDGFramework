using System;
using System.Collections.Generic;
using GDG.ModuleManager;
using GDG.Utils;
using TMPro;
using UnityEngine;

namespace GDG.UI
{
    public class LocalizationTMPText : MonoBehaviour
    {
        public string Key;
        private TMP_Text tmpText;
        [SerializeField]
        public LanguageTMPTextDictionary textStyle = new LanguageTMPTextDictionary();
        void Awake()
        {
            this.tmpText = GetComponent<TMP_Text>();
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
            if (this.tmpText != null)
            {
                this.tmpText.text = text;
            }
            if (textStyle.TryGetValue(language, out TMPTextStyle style))
            {
                tmpText.font = style.fontAsset;
                tmpText.fontStyle = style.fontStyle;
                tmpText.fontSize = style.fontSize;
                tmpText.lineSpacing = style.lineSpacing;
                tmpText.wordSpacing = style.wordSpacing;
                tmpText.characterSpacing = style.characterSpacing;
                tmpText.paragraphSpacing = style.paragraphSpacing;
            }
        }
    }
}