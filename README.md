# OneBot - Command Route Project Template

## 使用说明
### 使用反向 WS
1. 克隆本项目。
2. 将 `appsettings-reverse_ws-example.Development.json` 重命名为 `appsettings.Development.json`。
3. 将 `appsettings-reverse_ws-example.json` 重命名为 `appsettings.json`。
4. 根据需要编辑配置文件中的信息。
5. 编译运行。

### 使用正向 WS
1. 克隆本项目。
2. 将 `appsettings-ws-example.Development.json` 重命名为 `appsettings.Development.json`。
3. 将 `appsettings-ws-example.json` 重命名为 `appsettings.json`。
4. 根据需要编辑配置文件中的信息。
5. 编译运行。

## 简单的功能说明
### 指令路由
- 见类 `OneBot.FrameworkDemo.Modules.TestModule`。
    ```cs
        /// <summary>
        /// 再定义一个指令：
        ///
        /// <para>参数不必要按顺序、参数会触发依赖注入</para>
        /// </summary>
        /// <param name="gid">操作群号</param>
        /// <param name="uid">被禁言用户</param>
        /// <param name="duration">禁言时长</param>
        [Command("mute <gid> <uid> [duration]", Alias = "禁言 <gid> <uid> [duration], 口球 <gid> <uid> [duration],", EventType = EventType.GroupMessage | EventType.PrivateMessage)]
        public void MuteInGroupWithGroupId(Group gid, User uid, Duration duration)
        {
            if (duration == null) duration = new Duration(600);
            Console.WriteLine($"禁言 {gid.Id} 群里的 {uid.Id} 用户 {duration.Seconds} 秒。");
        }  
    ```
- 使用属性 `[Command]` 来将本函数定义为一个指令函数。若接收到合适的消息，本方法会被调用。
- 方法参数会被服务容器进行依赖注入，注入环境为 Scope。

### 小程序信息监听
- 见类 `OneBot.FrameworkDemo.Modules.TestModule`。
    ```cs
        /// <summary>
        /// 小程序简单监听方法，
        /// 这里是监听了群签到。
        /// </summary>
        /// <param name="e"></param>
        [CQJson("com.tencent.qq.checkin", EventType = EventType.GroupMessage)]
        public void CheckInListener(GroupMessageEventArgs e)
        {
            _logger.LogInformation($"{e.Sender.Id} 签到成功！");

            // 当然这里也是可以返回 0 或 1 的。
        }
    ```
- 使用属性 `[CQJson]` 来将本函数定义为一个小程序监听函数。若接收到合适的消息，本方法会被调用。
- 方法参数会被服务容器进行依赖注入，注入环境为 Scope。

### 智能类型转换
- 见类 `OneBot.FrameworkDemo.Models.Duration`
    ```cs
        /// <summary>
        /// 实现一个从字符串到 Duration 的隐式转换。
        /// 1d2h3m4s -> 1天2小时3分钟4秒
        /// </summary>
        /// <param name="value">时长</param>
        public static implicit operator Duration(string value)
    ```
- 实现一个从 `string` 到对应的类型的隐式转换即可让指令路由系统支持这个类型的转换。
- 若无法转换可抛出异常使得指令路由系统不处理该消息。

### 指令触发前事件
- 见类 `OneBot.FrameworkDemo.Attributes.DemoBeforeCommandAttribute`。
- - 若有需要可以自行定义一个 `BeforeCommandAttribute` 的子类来实现在解析指令后触发指令前进行拦截。（如实现权限判断和指令冷却时间等）

### 依赖注入
- 每一条消息都在 `Microsoft.Extensions.DependencyInjection` 的一个 `Scope` 中。可以根据需要添加自己的服务。
- 指令参数中多余的参数会被 scope 容器中的服务填充。
- 指令方法的参数填充优先级：
  1. 指令参数（指令定义的参数名和形参列表保持一致，若不一致可使用 `CommandParameter` 指定）
  2. 指令参数数组（`[ParsedArguments] object[] args`）
  3. 原始事件信息 `BaseSoraEventArgs`、`PrivateMessageEventArgs`、`GroupMessageEventArgs`
  4. Scope 对象 `IServiceScope`
  5. 剩余的未被上述可能成功填充的参数均会被从 Scope 容器中获取的对象填充。

### 基本事件
- 见类 `OneBot.FrameworkDemo.Modules.TestModule`。
    ```cs
        public TestModule(ICommandService commandService, ILogger<TestModule> logger)
        {
            // 通过构造函数获得指令路由服务对象

            // 添加自己的处理事件方法
            commandService.Event.OnGroupMessage += (scope, args) =>
            {
                // 在控制台中复读群里的信息
                logger.LogInformation($"{args.SourceGroup.Id} : {args.Sender.Id} : {args.Message.RawText}");
                return 0;
            };

            // 全局异常处理
            commandService.Event.OnException += (scope, args, exception) =>
            {
                logger.LogError($"{exception.Message}");
            };
        }
    ```
- 若不需要指令路由，依然可以通过监听事件来添加自己的处理逻辑。

### 消息处理链
- 指令函数可以返回 `int` 类型，基本事件返回 `int` 类型。若返回值为 1 则该事件阻断，不再传递给后续指令或事件监听函数；若返回值为 0 则继续传递。