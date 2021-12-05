using System;
using System.Collections.Generic;
using GDG.ModuleManager;
using GDG.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace GDG.UI
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
            GDGTools.ResourceLoder.TryLoadResourceAsync<Sprite>(text, (sprite) =>
             {
                 this.img.sprite = sprite;
                 if (this.img.sprite == null)
                 {
                     string[] strs = text.Split('/');
                     GDGTools.AssetLoder.TryLoadAssetAsync<Texture2D>(strs[0], strs[1], (texture) =>
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