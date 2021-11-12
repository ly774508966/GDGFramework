# GDGFramework

GDGFramework 是一个基于Unity引擎的游戏框架，内置了简易实用的 ECS 架构，且支持原生的 Unity Mono 开发。打包好的 unitypackage 可以在发布版本中进行下载。

GDGFramework 内置了以下模块：

* ## ECS 实体组件系统

    基于对象池实现，基本无GC。用法与 DOTS-ECS 类似，支持在 OnUpdate 中进行事件触发时、间隔时间时、间隔帧时对实体进行遍历和过滤。此外还设置了可视化的 EntitiesViewer，用于查看当前正在运行的系统、实体的状态、实体组件的详细信息。

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

        Entity 可以由 EntityManager 创建，EntityManager 提供了绑定了 GameObject 的实体的创建、绑定了从AB包中获取的资源的实体创建、绑定了从 Resources 文件夹中获取的资源的实体的创建。

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

        每一个 Entity 中的每一份数据都是一个Component（组件），一组不同的组件叫做一个 ComponentTypes，根据组件的不同，ComponentTypes 将会有不同的 TypeId。所有的组件都应该继承 `GDG.ECS.IComponent` 接口。一般来说组件不应该有任何行为，但你可以通过 IEntityRecyclable、IEntityDestoryable、IEntityCreateable 接口来在实体的生命周期中实现一些自己的逻辑：

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
                Select((Entity entity) =>
                {
                    Log.Info("每帧打印");
                })
                .Excute();

                Select((Entity entity) =>
                {
                    Log.Info("直到名为 Log 的事件被触发时打印");
                })
                .ExcuteWithEvent("Log",1);//右边的参数为Select的唯一Id号（需要用户自己定义），下同

                Select((Entity entity) =>
                {
                    Log.Info("每隔2s打印");
                })
                .ExcuteDelayTime(2f,2);

                Select((Entity entity) =>
                {
                    Log.Info("每隔5帧打印");
                })
                .ExcuteDelayFrame(5,3);//右边的参数为Select的Id号，下同	
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

* ## GDGTools 常用工具模块
    
    GDGTools 是一个静态工具类，内置了以下常用工具模块：

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
    * ### PanelController UI加载器
        UI基于Panel管理，Panel需要继承至BasePanel，自动绑定了UI事件。支持对 Panel 暂停、重启、销毁、创建、隐藏、取消隐藏。

* ## 日志打印模块

    支持为Log添加tag，方便过滤日志。内置两种模式：Game下的日志（通过`this.log`打印）、Console下日志（通过`Log.Info`打印）。包含了 Info、Sucess、Warning、Error、Editor、Custom 几种风格的打印信息。可以在ProjectSetting选择是否将日志写入文件，在`User/Logger/UnityLogger.txt`中找到被写入的日志文件，可以用于将客户端打印信息上传到服务器。

* ## Async 异步模块

    * ### AsyncRunner 异步执行器
        通过 `AsyncRunner.RunAsync` 来开启一个多线程任务，支持取消令牌的使用。通过`AsyncRunner.SyncToMainThread`来将多线程任务中 unity 原生 mono 组件的逻辑加入到主线程中执行。

    * ### AsyncWaiter 异步等待器
        功能与协程类似，但是是基于多线程的，通过获取 `Current` 属性来获得每一次 yield return 的返回值。你可以通过实现 `IYieldInstruction` 接口来实现具体的等待类，用于告知等待器何时开始下一次迭代。

    * ### AsyncWebRequest 异步网络请求
        用于异步下载网络资源，并且支持 Get 或者 Post 方法向服务器发送请求并获取响应数据。

* ## 其它编辑器拓展工具

    * ### Auto Namespace
        用户可以通过设置自定义命名空间来在每一次的 .cs 脚本创建时自动加上。
    * ### Persist Tools
        支持 Json、Xml、Excel 数据表之间的转换
    * ### Remove Missing Scripts
        自动删除场景中以及Assets文件夹下所有 Miss 的脚本
    * ### ProjectSetting
        项目设置，包括音量、输入（支持Editor模式下添加键盘输入映射）、全局宏管理、日志管理
    * ### EntitiesViewer
        实时地查看 ECS 相关信息
