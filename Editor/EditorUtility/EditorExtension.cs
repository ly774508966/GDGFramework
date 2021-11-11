using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GDG.ECS;
using System.IO;

namespace GDG.Editor
{
    public class EditorExtension
    {
        public static string TemplatePath = "Assets/GDGFramework/Editor/ScriptTemplate";

        #region Hierarchy
        [MenuItem("GameObject/Create GameEntity", false, 0)]
        private static void CreateGameEntity()
        {
            var obj = new GameObject("Entity");
            obj.AddComponent<GameObjectToEntity>();
        }      
        [MenuItem("GameObject/3D Entity/Cube", false,1)]
        private static void CreateCube()
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "Cube";
            obj.AddComponent<GameObjectToEntity>();
        }
        [MenuItem("GameObject/3D Entity/Sphere", false,1)]
        private static void CreateSphere()
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.name = "Sphere";
            obj.AddComponent<GameObjectToEntity>();
        }
        [MenuItem("GameObject/3D Entity/Capsule", false,1)]
        private static void CreateCapsule()
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            obj.name = "Capsule";
            obj.AddComponent<GameObjectToEntity>();
        }
        [MenuItem("GameObject/3D Entity/Cylinder", false,1)]
        private static void CreateCylinder()
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.name = "Cylinder";
            obj.AddComponent<GameObjectToEntity>();
        }
        #endregion
        #region Project
        public static string GetCurrentSelectPath()
        {
            string path = "Assets";
            foreach (var item in Selection.GetFiltered<Object>(SelectionMode.Assets))
            {
                //获得当前选择的路径
                path = AssetDatabase.GetAssetPath(item);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }
        public static void CreateCshapScriptFromTemplate(string defaultName, string templatePath)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<CSScriptCreator>(),
                GetCurrentSelectPath() + "/" + defaultName,
                null,
                templatePath
                );
        }

        [MenuItem("Assets/Create/Component", false, 50)]
        private static void CreateComponent()
        {
            CreateCshapScriptFromTemplate("TestComponent.cs", TemplatePath + "/ComponentTemplate.txt");
        }
        [MenuItem("Assets/Create/System", false, 51)]
        private static void CreateSystem()
        {
            CreateCshapScriptFromTemplate("TestSystem.cs", TemplatePath + "/SystemTemplate.txt");
        }
        [MenuItem("Assets/Create/Proxy", false, 52)]
        private static void CreateProxy()
        {
            CreateCshapScriptFromTemplate("EntityProxy.cs", TemplatePath + "/ProxyTemplate.txt");
        }
        [MenuItem("Assets/Create/Class", false, 31)]
        private static void CreateClass()
        {
            CreateCshapScriptFromTemplate("Test.cs", TemplatePath + "/ClassTemplate.txt");
        }
        [MenuItem("Assets/Create/Interface", false, 32)]
        private static void CreateInterface()
        {
            CreateCshapScriptFromTemplate("IInterface.cs", TemplatePath + "/InterfaceTemplate.txt");
        }
        [MenuItem("Assets/Create/Enum", false, 32)]
        private static void CreateEnum()
        {
            CreateCshapScriptFromTemplate("TestType.cs", TemplatePath + "/EnumTemplate.txt");
        }
        #endregion
    }
}