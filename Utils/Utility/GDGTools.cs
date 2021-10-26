using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GDG.ECS;
using GDG.ModuleManager;
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
        public static FlowFieldManager FlowFieldController { get => FlowFieldManager.Instance; }
        public static AssetPool AssetPool { get => AssetPool.Instance; }
        public static ResourcesManager ResourceLoder{ get => ResourcesManager.Instance; }
        public static AssetManager AssetLoder { get => AssetManager.Instance; }
        public static PanelManager PanelControl { get => PanelManager.Instance; }
        #endregion

        #region 静态方法
        public static TextMesh CreateWorldText(out GameObject gameobject,string text,int fontSize = 40,Vector3 localPosition = default(Vector3),Transform parent = null,Action<Entity> callback = null,Color color = default(Color),TextAnchor textAnchor = TextAnchor.MiddleCenter,TextAlignment textAlignment = TextAlignment.Left)
        {
            ComponentTypes componentTypes = new ComponentTypes(typeof(GridComponent));

            var entity = World.EntityManager.CreateEntity<GameObjectComponent>((gameObjectComponent) =>
            {
                gameObjectComponent.gameObject = new GameObject("WorldText", typeof(TextMesh));
            });


            if(callback!=null)
            {
                callback(entity);
            }

            gameobject = entity.GetComponent<GameObjectComponent>().gameObject;

            Transform trans = gameobject.transform;
            trans.SetParent(parent, false);
            trans.localPosition = localPosition;

            TextMesh textMesh = gameobject.GetComponent<TextMesh>();
            textMesh.text = text;
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.fontSize = fontSize;
            if(color == null || color == default(Color))
                color = Color.white;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = 1000;
            return textMesh;
        }
        public static Vector3 GetMouseWorldPosition(float z = 0,Camera camera = null)
        {
            if(camera == null)
                camera = Camera.main;
            var playerPos = camera.WorldToScreenPoint(new Vector3(0,0,z));
            var pos = new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, playerPos.z);
            return camera.ScreenToWorldPoint(pos);
        }
        public static bool IsBlittable(object obj)
        {
            var type = obj.GetType();
            if(type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) ||
               type == typeof(ushort) || type == typeof(int) || type == typeof(uint) ||
               type == typeof(long) || type == typeof(ulong) || type == typeof(Single) || type == typeof(double))
                return true;
            return false;
        }
        
        #endregion
    }
}