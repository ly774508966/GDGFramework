using System;
using System.Collections.Generic;
using GDG.ModuleManager;
using GDG.Utils;
using TMPro;
using UnityEngine;

namespace GDG.Localization
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