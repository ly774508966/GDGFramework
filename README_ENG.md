![GDGFramework](https://github.com/Gatongone/GDGFramework/blob/main/GDGFramework.png)
<p>
  <img alt="GDGFramework Version" src="https://img.shields.io/badge/Version-v1.0-D90A0E.svg?cacheSeconds=2592000" />
  <img alt="Unity3D Version" src="https://img.shields.io/badge/Unity3D-2020.3.3 ~ 2021.x-D95B0A.svg?cacheSeconds=2592000" />
  <img alt=".Net Version" src="https://img.shields.io/badge/.net->=4.7.1-ABD90A.svg?cacheSeconds=2592000"/>
  <img alt="Author" src="https://img.shields.io/badge/Author-Gatongone-0AD994.svg?cacheSeconds=2592000"/>
  <a href="https://github.com/Gatongone/GDGFramework/actions">
    <img alt="CI" src="https://img.shields.io/badge/CI-passing-0AA3D9.svg?cacheSeconds=2592000" />
  </a>
  <a href="https://gdgframework.docsforge.com/">
    <img alt="Document" src="https://img.shields.io/badge/docs-docsforge-0A4FD9.svg?cacheSeconds=2592000"/>
  </a>
  <a href="https://codecov.io/gh/Gatongone/GDGFramework">
    <img src="https://codecov.io/gh/Gatongone/GDGFramework/branch/main/graph/badge.svg?token=4Z6XFG95MN"/>
  </a>
</p>

> GDGframework is a game framework based on unity3d engine. It has built-in simple and practical ECS architecture and supports the development of native unity mono. Packaged unitypackage can be in [Release Version](https://github.com/Gatongone/GDGFramework/releases) Download from.

> [中文](https://github.com/Gatongone/GDGFramework/blob/main/README.md)|[English](https://github.com/Gatongone/GDGFramework/blob/main/README_ENG.md)

* ## Author

  👤 **Gatongone**

    * Website: https://www.gatongone.site/
    * Github: [@Gatongone](https://github.com/Gatongone)
    * Email: [gatongone@gmail.com]()
---

* ## Include Projects:

    1. [LitJson](https://github.com/LitJSON/litjson)
    2. [JsonNet](https://github.com/JamesNK/Newtonsoft.Json)
    3. [EPPlus](https://github.com/EPPlusSoftware/EPPlus)
    4. [SerializableDictionary](https://github.com/azixMcAze/Unity-SerializableDictionary)
    5. [GDGLogger](https://github.com/Gatongone/GDGLogger)

---

* ## ECS (Entity-Component-System)

    Based on the implementation of object pool. The usage is similar to that of dots-ecs. It supports traversal and filtering of entities during event triggering, interval time and interval frame in OnUpdate. In addition, a visual EntitiesViewer is also set to view the current running systems, entities status and component details.

    * ### Entity

        Each game object (not just GameObject) can be used as an entity. Entity means that it does not represent anything. As long as it is given components, it can have some functions (what it can do), while the system gives the entity specific behavior (how to do it). Its structure is as follows:

        ```C#
        public class Entity : IEquatable<Entity>
        {
            public string Name { get; set; }//entity name
            public ulong Index { get; }//unique ID number used to distinguish different entities
            public int Version { get; }//Used to distinguish the number of times it is recycled
            public uint TypeId { get; }//Component type ID, indicating which type of component is mounted on the entity
            public bool IsActived { get; }//Only when entity is activated can it be detected by the system
        }
        ```

        Entity can be created by `EntityManager`, which provides the creation method of the following entities:
        
        * Created as GameObject entity
        * Created from AssetBundle
        * Created from Resources folder.

        ```C#        
        //Create empty entity
        var entity1 = World.EntityManager.CreateEntity();
        
        //Create entity with some Component
        var entity2 = World.EntityManager.CreateEntity(new ComponentTypes(typeof(MoveComponent), typeof(LogComponent)));
        
        //Crate entity as GameObject
        var entity3 = World.EntityManager.CreateGameEntity(new ComponentTypes(typeof(MoveComponent)), new GameObject());
        
        //Created from AssetBundle, The parameters are：
        //Asset name
        //AssetBundle name
        //main bundle Name
        //bundle path
        //rename
        var entity4 = World.EntityManager.CreateEntityFromAssetBundle<GameObject>("LoginUI","ui","PC",Application.streamingAssetsPath,null);

        //Created from Resources folder.
        //The parameter is the path under Resources folder where the asset is located
        var entity5 = World.EntityManager.CreateEntityFromResources<GameObject>("Prefab/Cube");
        ```

    * ### Component
        
        Each piece of data in each entity is a **Component**. A group of different components is called a **Componenttypes**. Componenttypes will have different typeids according to different components. All components should implement the `GDG.ECS.Icomponent` interface. Normally, Components shouldn't have any Behavioral logic, but you can implement some of your own logic in the entity life cycle by `IEntityNamedable`, `IEntityInitable`, `IEntityEnable`, `IEntityRecyclable`, `IEntityDestoryable` interfaces:

        ```C#
        public class MoveComponent : IComponent,IEntityDestoryable
        {
            public int speed = 5;
            public void DestroyEntity(Entity entity)
            {
                Log.Info(entity.Name + "Destroyed");
            }
        }
        ```

        There are only 2 Components in base Framework：
        
        * `GameObjectComponent`: Will be added into entity when you try to create a entity just like GameObject.
        
        * `AssetComponent`: Will be added into entity when you try to instantiate asset (like load from Resources or AssetBundle).


    * ### System
        
        System is responsible for processing the data in the component. System will perform corresponding functions for the corresponding entity according to the results of the query component in `OnUpdate`. At PlayMode, Unity will find all systems and instantiate them. A simple usage is as follows:

        ```C#
        public class LogSystem : SystemBase<LogSystem>
        {
            public override void OnStart()
            {
                
            }
        
            public override void OnUpdate()
            {
                // 1 ---------
                Select((Entity entity) =>
                {
                    Log.Info("Printed every frame");
                })
                .Excute();

                // 2 ---------
                Select((Entity entity) =>
                {
                    Log.Info("Printed until an event named 'Log' is triggered");
                })
                .ExcuteWithEvent("Log",1);//The parameter on the right is the unique ID number of Select (defined by the user), the same below

                // 3 ---------
                Select((Entity entity) =>
                {
                    Log.Info("Printed every 2 seconds");
                })
                .ExcuteDelayTime(2f,2);

                // 4 ---------
                Select((Entity entity) =>
                {
                    Log.Info("Printed every 5 frames");
                })
                .ExcuteDelayFrame(5,3);
            }
        }
        ```
        Filtering of components:

        ```C#
        public class MoveSystem : SystemBase<MoveSystem>
        {
            public override void OnStart()
            {
                
            }
        
            public override void OnUpdate()
            {
                Select((Entity entity,MoveComponent move,GameObjectComponent game) =>
                {
                    game.transform.Translate(Vector3.forward * move.speed * Time.deltaTime);
                })
                .WithAll<Entity,LogComponent>()//In addition to the Component in Select, the 'LogComponent' must also be included
                .Excute();
            }
        }
        ```
    * ### Hybrid development with MonoBehavior script

        When a `GameObjectToEntity` MonoBehaviour script is added to a GameObject in the scene, it'll become an entity with a `GameObjectComponent` attached. You can add additional ECS components to it by adding a proxy (MonoBehavior script which inherited from `EntityProxy`).

---

* ## ProjectSetting
  
  Open GDGFramework/ProjectSetting. There are some global setting.

  * ### Logger

   In order to separate the development test from the game release, you can close the log before the game release, and all `Log.xxx` and '`this.logxxx` will no longer take effect

  * ### Input

    You can quickly add a key name, add the key listener and quickly set the key as in the game. It support the binding of up to three key combinations.

  * ### Audio

    Mute, or modify the global volume, BGM volume and sound volume.

  * ### Marco

    Quickly set up macros, and you can choose whether to enable them or not.

  * ### Locale

    One click switch UI language, support text, TMP_Text The and Image, and the localization configuration table can be found in the config folder. Add a `LocalizationImage` or `LocalizationText` or `LocalizationTMPtext` script in the UI and set the key value. In projectsetting, you can set different values for the key value of each language. Key here refers to the unique handle of each UI, and Value has different meanings according to different UIs:

    1. If it is Text/TMP_Text, value refers to the text information in the language version

    2. If it is Image, value refers to the sprite path under the Resources folder or the AB package resource path (in the format of Bundlename/Assetname)

    `LocalizationText ` and `LocalizationTMPtext ` can customize text styles, including font, spacing, etc` Localizationimage ` if sprite style is set, the final effect will be the picture in sprite style, but there must be at least one arbitrary content in the language, otherwise sprite style will be invalid.

---

* ## GDGTools
    
    GDGTools is a static tool class with built-in tool modules commonly used in the game:

    * ### Timer
        Performs callback methods at intervals of frames or time.
    * ### EventCenter
        Observer mode based on delegation. Encapsulates an event by binding a string and a delegate.
    * ### MessageCenter
        Non-delegated observer mode. You can inherit to the `MessageObserver` abstract class or create an `Observer` instance directly to implement the specific message.
    * ### PersistTools
        Supports serialization and deserialization of instance objects by Excel, Json, Xml, PlayerPrefs, and binary files.
    * ### Input
        Support custom key name to keyboard mapping, support up to three key combinations, support key changes.
    * ### AudioController
        Supports BGM, general sound, 3D sound control.
    * ### AssetPool
        Used for caching and reusing `UnityEngine.Object` objects to ease CPU pressure when instantiating objects. Supports automatic emptying of object pools on a timer and idle basis.
    * ### ResourceLoder
        Used to load and instantiate `UnityEngine.Object` from Resources folder.
    * ### AssetLoder
        Used to load and instantiate assets from  AssetBundle.
    * ### PanelController
        UI is based on panel management, all panels need to inherit to` BasePanel` and automatically bind UI events. Supports loading, pausing, restarting, destroying, creating, hiding, and unhiding Panels.
    * ### LocalLanguage
        Switch language versions immediately by setting `CurrentLanguage`.

---

* ## FlowFieldController

    AI routing system based on flow field supports 2D and 3D grids. Use `FlowFieldController.GenerateFlowField` to generate a flow field with the default barrier Layer being `Impassible`. Set a destination for the flow field by the `SetDestination` method and obtain the flow direction for a grid by the `GetFieldDirection` method.

---

* ## Logger module

    It supports adding tags to logs to facilitate log filtering. There are two built-in log methods: log under game (printed by `this. Log` API) and log under console (printed through `Log. Info`). It contains printing information in the styles of Info, Sucess, Warning, Error, Editor and Custom. You can choose whether to write the log to the file in projectsetting. The written log file can be found in `Application.persistentdatapath/logger/UnityLogger.txt`, which can be used to upload the client logger information to the server.

---

* ## Async

    * ### AsyncRunner

        Start a multithreaded Task through `AsyncRunner.Runasync`, which supports the use of cancellation tokens. Use `AsyncRunner.Synctomainthread` to add the logic of the unit native mono component in the multithreaded task to the main thread for execution.

    * ### AsyncWaiter

        The usage is similar to the Couroutine, but it is based on multithreading. You can use it with AsyncRunner. Get the return value of each yield return by the `Current` property. You can achieve your wait functions by implementing the `IYieldInstruction` interface, which is used to tell the AsyncWaiter when to start the next iteration.

    * ### AsyncWebRequest
        Used to download network resources asynchronously, and supports Get or Post methods to send requests to the server and obtain response data.

---

* ## Other Editor Extension Tools

    * ### Auto Namespace
        You can set a custom namespace to automatically add it each time a ".cs" script is created. (AutoNamespace should be closed if creating a non".cs" asset causes the editor to be stuck in Application.Message.LeftButtonDown)
    * ### DataTableConversion
        Supports conversion between Json, Xml and Excel data tables
    * ### Remove Missing Scripts
        Automatically delete all missing scripts in the scene and under the Assets path.

---
