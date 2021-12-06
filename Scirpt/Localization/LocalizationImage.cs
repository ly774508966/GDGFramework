using System;
using System.Collections.Generic;
using GDG.ModuleManager;
using GDG.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace GDG.Localization
{
    public class LocalizationImage : MonoBehaviour
    {
        public string Key;
        [SerializeField]
        public LanguageImageTextDictionary SpriteStyle = new LanguageImageTextDictionary();
        private Image img;
        void Awake()
        {
            this.img = GetComponent<Image>();
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
            ResourcesManager.Instance.TryLoadResourceAsync<Sprite>(text, (sprite) =>
             {
                 this.img.sprite = sprite;
                 if (this.img.sprite == null)
                 {
                     string[] strs = text.Split('/');
                     AssetManager.Instance.TryLoadAssetAsync<Texture2D>(strs[0], strs[1], (texture) =>
                     {
                         this.img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero); ;
                         if (SpriteStyle.TryGetValue(language, out ImageStyle style))
                         {
                             this.img.sprite = style.sprite;
                         }
                     });
                 }
                 else if (SpriteStyle.TryGetValue(language, out ImageStyle style))
                 {
                     this.img.sprite = style.sprite;
                 }
             });
        }
    }
}