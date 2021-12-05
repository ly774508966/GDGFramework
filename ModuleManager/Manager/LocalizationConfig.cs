using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.Utils;
using UnityEditor;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GDG.ModuleManager
{
    
    public enum Language
    {
        None,
        Arabic,
        Chinese_Simplified,
        Chinese_Traditional,
        English,
        Farsi,
        French,
        German,
        Hausa,
        Hindi,
        Italian,
        Japanese,
        Javanese,
        Korean,
        Malay,
        Marathi,
        Portuguese,
        Punjabi,
        Russian,
        Spanish,
        Swahili,
        Tamil,
        Telugu,
        Vietnamese,

    }
    public class LocalizationConfig
    {
        static LocalizationConfig instance;
        static object syncRootObject = new object();
        public static LocalizationConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRootObject)
                    {
                        if (instance == null)
                        {
                            instance = GDGTools.PersistTools.LoadData_Json<LocalizationConfig>(Config.Configurations.ConfigPath + "\\LocalizationConfig.json");
                        }
                    }
                }
                return instance;
            }
        }
        Language currentLanguage;
        [JsonConverter(typeof(StringEnumConverter))]
        public Language CurrentLanguage { 
            get
            {
                return currentLanguage;
            }
            set
            {
                SetLanguage(value);
            } 
        }
        internal bool isInit = false;
        public Dictionary<Language, Dictionary<string, string>> LanguageMapping;
        internal void SetLanguage(Language language)
        {
            if (currentLanguage.Equals(language))
                return;
            currentLanguage = language;

            if (language == Language.None)
                return;

            if (LanguageMapping != null && LanguageMapping.TryGetValue(language, out Dictionary<string, string> dic))
            {
                foreach (var kv in dic)
                {
                    GDGTools.EventCenter.ActionTrigger<Language,string, string>("SetLocalizationText", currentLanguage,kv.Key, kv.Value);
                }
            }
        }
    }
}