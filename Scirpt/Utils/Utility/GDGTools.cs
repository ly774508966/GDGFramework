using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GDG.Localization;
using GDG.ModuleManager;
using GDG.UI;
using UnityEngine;

namespace GDG.Utils
{
    public class GDGTools
    {
        #region 静态属性
        public static TimerManager Timer{ get => TimerManager.Instance; }
        public static EventManager EventCenter{ get => EventManager.Instance; }
        public static MessageManager MessageCenter{ get => MessageManager.Instance; }
        public static PersistManager PersistTools{ get => PersistManager.Instance; }
        public static InputManager Input { get => InputManager.Instance; }
        public static AudioManager AudioController { get => AudioManager.Instance; }
        public static AssetPool AssetPool { get => AssetPool.Instance; }
        public static ResourcesManager ResourceLoder{ get => ResourcesManager.Instance; }
        public static AssetManager AssetLoder { get => AssetManager.Instance; }
        public static PanelManager PanelControl { get => PanelManager.Instance; }
        public static LocalizationConfig LocalLanguage { get => LocalizationConfig.Instance; }
        #endregion

        #region 静态方法
        public static Vector3 GetMouseWorldPosition(float z = 0,Camera camera = null)
        {
            if(camera == null)
                camera = Camera.main;
            var playerPos = camera.WorldToScreenPoint(new Vector3(0,0,z));
            var pos = new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, playerPos.z);
            return camera.ScreenToWorldPoint(pos);
        }
        
        #endregion
    }
}