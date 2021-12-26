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

> GDGFramework 是一个基于 Unity3D 引擎的游戏框架，内置了简易实用的 ECS 架构，且支持原生的 Unity Mono 开发。打包好的 unitypackage 可以在[发布版本](https://github.com/Gatongone/GDGFramework/releases)中进行下载。

> [中文](https://github.com/Gatongone/GDGFramework/blob/main/README.md)|[English](https://github.com/Gatongone/GDGFramework/blob/main/README_ENG.md)

* ## 作者

  👤 **Gatongone**

    * Website: https://www.gatongone.site/
    * Github: [@Gatongone](https://github.com/Gatongone)
    * Email: [gatongone@gmail.com]()
---

* ## 包含依赖项:

    1. [LitJson](https://github.com/LitJSON/litjson)
    2. [JsonNet](https://github.com/JamesNK/Newtonsoft.Json)
    3. [EPPlus](https://github.com/EPPlusSoftware/EPPlus)
    4. [SerializableDictionary](https://github.com/azixMcAze/Unity-SerializableDictionary)
    5. [GDGLogger](https://github.com/Gatongone/GDGLogger)

---

* ## ECS 实体组件系统

<<<<<<< HEAD
    基于对象池实现。用法与 DOTS-ECS 类似，支持在 OnUpdate 中进行事件触发时、间隔时间时、间隔帧时对实体进行遍历和过滤。此外还设置了可视化的 EntitiesViewer，用于查看当前正在运行的系统、实体的状态、实体组件的详细信息。
=======
    基于对象池实现，用法与 DOTS-ECS 类似，支持在 OnUpdate 中进行事件触发时、间隔时间时、间隔帧时对实体进行遍历和过滤。此外还设置了可视化的 EntitiesViewer，用于查看当前正在运行的系统、实体的状态、实体组件的详细信息。
>>>>>>> main

    * ### Entity

        每一个游戏对象（不仅仅是GameObject）都可以作为一个实体（Entity）。实体意味着它本身并不代表任何东西，只要被赋予了组件后，它才能拥有一些功能（可以去做什么），而系统则赋予实体具体的行为（怎么去做）。它的结构如下：

        ```C#
        public class Entity : IEquatable<Entity>
        {
            public string Name { get; set; }//实体名称
            public ulong Index { get; }//唯一的ID号，用于区分不同实体
            public int Version { get; }//版本号，用于区分是第几次世代（第几次被回收）
            public uint TypeId { get; }//组件类型ID，表明身上挂载了哪一类型的组件
            public bool IsActived { get; }//该实体是否处于被激活状态，只有被激活才能被System检测到
        }
        ```

        Entity 可以由 EntityManager 创建，EntityManager 提供了以下实体的创建方式：
        
        GameObject 的实体的创建、从AB包中获取的资源的实体创建、从 Resources 文件夹中获取的资源的实体的创建。

        ```C#        
        //创建空实体
        var entity1 = World.EntityManager.CreateEntity();
        
        //创建带有某些组件的实体     
        var entity2 = World.EntityManager.CreateEntity(new ComponentTypes(typeof(MoveComponent), typeof(LogComponent)));
        
        //创建与GameObject绑定的实体
        var entity3 = World.EntityManager.CreateGameEntity(new ComponentTypes(typeof(MoveComponent)), new GameObject());
        
        //创建从AB包中加载出来的实体
        //参数分别为：Asset名、Asset所在包名、主包名、包所在路径、重命名
        var entity4 = World.EntityManager.CreateEntityFromAssetBundle<GameObject>("LoginUI","ui","PC",Application.streamingAssetsPath,null);

        //创建从Resources中加载出来的实体
        //参数为Resources中资源所在路径
        var entity5 = World.EntityManager.CreateEntityFromResources<GameObject>("Prefab/Cube");
        ```

    * ### Component

        每一个 Entity 中的每一份数据都是一个Component（组件），一组不同的组件叫做一个 ComponentTypes，根据组件的不同，ComponentTypes 将会有不同的 TypeId。所有的组件都应该继承 `GDG.ECS.IComponent` 接口。一般来说组件不应该有任何行为，但你可以通过`IEntityNamedable`、`IEntityInitable`、`IEntityEnable`、`IEntityRecyclable`、`IEntityDestoryable` 接口来在实体的生命周期中实现一些自己的逻辑：

        ```C#
        public class MoveComponent : IComponent,IEntityDestoryable
        {
            public int speed = 5;
            public void DestroyEntity(Entity entity)
            {
                Log.Info(entity.Name + "被销毁了");
            }
        }
        ```

        我仅仅内置了两个组件： `GameObjectComponent`：用于获取实体身上绑定的 GameObject，`AssetComponent`：用于获取身上绑定的Asset资源。


    * ### System
        System 负责对 Component 中的数据进行处理，System会根据 `OnUpdate` 中的查询组件的结果来为对应的 Entity 执行相应的功能。在运行时Unity会找到所有的System，并实例化它们，一个简单的用法如下：

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
                    Log.Info("每帧打印");
                })
                .Excute();
                
                // 2 ---------
                Select((Entity entity) =>
                {
                    Log.Info("直到名为 Log 的事件被触发时打印");
                })
                .ExcuteWithEvent("Log",1);//右边的参数为Select的唯一Id号（需要用户自己定义），下同
                
                // 3 ---------
                Select((Entity entity) =>
                {
                    Log.Info("每隔2s打印");
                })
                .ExcuteDelayTime(2f,2);
                
                // 4 ---------
                Select((Entity entity) =>
                {
                    Log.Info("每隔5帧打印");
                })
                .ExcuteDelayFrame(5,3);
            }
        }
        ```
        对组件的过滤：

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
                .WithAll<Entity,LogComponent>()//除Select组件外，还必须包含LogComponent组件
                .Excute();
            }
        }
        ```
    * ### 与 Unity 原生Mono组件的混合开发

        当场景中的 GameObject 被加上一个`GameObjectToEntity` Mono组件时，它就成为一个挂载有 `GameObjectComponent`组件的实体，你可以通过给它加上一个代理（继承至`IEntityProxy`的 MonoBehaviour）来为其增加额外的ECS组件。

---

* ## ProjectSetting 项目管理
  
  在 GDGFramework/ProjectSetting 中打开项目管理界面。

  * ### Logger：日志管理

    为了开发测试与游戏发布分离，可以在游戏发布前将日志关闭，所有的 `Log.XXX` 和 `this.logxxx` 都将不再生效

  * ### Input 输入键位设置

    可以快速的增加一个键名，并且可以像游戏中一样监听键盘按键并快速设置键位，至多支持三个组合键的绑定。

  * ### Audio 音量管理

    静音，或者修改全局音量、BGM音量、音效音量。

  * ### Marco 全局宏管理

    快速设置宏，并且可以选择是否启用。

  * ### Locale：本地化管理方案

    一键切换 UI 语言版本，支持 Text、TMP_Text、Image 的本地化方案，本地化配置表在 Config 文件夹下可以被找到。在 UI 中添加 `LocalizationImage` 或 `LocalizationText` 或 `LocalizationTMPText` 脚本，并设置key值，在 ProjectSetting 中即可为各个语言的 key 值设置不同的 value。这里的 key 指的是每一个 UI 的唯一 handle，value 的含义根据不同的 UI 有不同的含义：

    1. 若为 Text，则 Value 指的是在该语言版本下的文字信息
    2. 若为 Image，则 Value 指的是在 Resources 文件夹下的 Sprite 路径，或者是 AB 包资源路径（格式为 bundleName/assetName）

    `LocalizationText` 和 `LocalizationTMPText` 都可以自定义文字样式，包括字体、间距等。`LocalizationImage` 若设置了 SpriteStyle 则将以 SpriteStyle 中的图片为最终效果，但在该语言版本下必须有至少一个任意内容，否则 SpriteStyle 将无效。

---

* ## GDGTools 常用工具模块
    
    GDGTools 是一个静态工具类，内置了以下游戏中常用工具模块：

    * ### Timer 计时器
        隔帧、或隔时间段执行回调方法。
    * ### EventCenter 事件中心
        基于委托的观察者模式，通过绑定 string 和一个委托封装一个事件。
    * ### MessageCenter 消息中心
        非委托的观察者模式，可以继承至 `MessageObserver` 抽象类或直接创建 `Observer` 实例来实现具体消息。
    * ### PersistTools 数据持久化工具
        支持 Excel、Json、Xml、PlayerPrefs、二进制文件 对实例对象的序列化和反序列化。
    * ### Input 输入控制
        支持自定义键名到键盘按键的映射，支持至多三个的组合键，支持改键。
    * ### AudioController 声音控制
        支持BGM、一般音效、3D音效的控制。
    * ### AssetPool 资源池
        用于对 `UnityEngine.Object` 对象的缓存和复用，减缓实例化对象时CPU的压力。支持定时和空闲时自动清空对象池。
    * ### ResourceLoder Resources文件加载器
        用于对Resources下 `UnityEngine.Object` 的加载与实例化。
    * ### AssetLoder AB包资源加载器
        用于对AB包中资源的加载与实例化。
    * ### PanelController UI管理器
        UI基于Panel管理，Panel需要继承至BasePanel，自动绑定了UI事件。支持对 Panel 加载、暂停、重启、销毁、创建、隐藏、取消隐藏。
    * ### LocalLanguage 本地化语言管理
        通过设置 `CurrentLanguage` 即可立即切换语言版本

---

* ## FlowFieldController 流场寻路

    基于流场的AI寻路系统，支持2D和3D两种网格。使用 `FlowFieldController.GenerateFlowField` 来生成一个流场，默认障碍物Layer为 “Impassible”。通过`SetDestination` 方法来为流场设置一个目的地，通过 `GetFieldDirection` 方法来获得某一网格的流场方向。

---

* ## 日志打印模块

    支持为Log添加tag，方便过滤日志。内置两种模式：Game下的日志（通过`this.log`打印）、Console下日志（通过`Log.Info`打印）。包含了 Info、Sucess、Warning、Error、Editor、Custom 几种风格的打印信息。可以在ProjectSetting选择是否将日志写入文件，在`Application.persistentdatapath/logger/UnityLogger.txt`中找到被写入的日志文件，可以用于将客户端打印信息上传到服务器。

---

* ## Async 异步模块

    * ### AsyncRunner 异步执行器
        通过 `AsyncRunner.RunAsync` 来开启一个多线程任务，支持取消令牌的使用。通过`AsyncRunner.SyncToMainThread`来将多线程任务中 unity 原生 mono 组件的逻辑加入到主线程中执行。

    * ### AsyncWaiter 异步等待器
        功能与协程类似，但是是基于多线程的，可以配合AsyncRunner使用。通过获取 `Current` 属性来获得每一次 yield return 的返回值。你可以通过实现 `IYieldInstruction` 接口来实现具体的等待类，用于告知等待器何时开始下一次迭代。

    * ### AsyncWebRequest 异步网络请求
        用于异步下载网络资源，并且支持 Get 或者 Post 方法向服务器发送请求并获取响应数据。

---

* ## 其它编辑器拓展工具

    * ### Auto Namespace
        用户可以通过设置自定义命名空间来在每一次的 .cs 脚本创建时自动加上。（在创建非.cs资源时如果发生导致编辑器卡在"Application.Message.LeftButtonDown"，应关闭AutoNamespace）
    * ### DataTableConversion
        支持 Json、Xml、Excel 数据表之间的转换
    * ### Remove Missing Scripts
        自动删除场景中以及Assets文件夹下所有 Miss 的脚本

---
