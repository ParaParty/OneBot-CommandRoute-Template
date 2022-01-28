using OneBot.CommandRoute.Attributes;
using OneBot.CommandRoute.Models.Enumeration;
using OneBot.CommandRoute.Services;
using OneBot.FrameworkDemo.Attributes;
using OneBot.FrameworkDemo.Models;

using Sora.Entities;
using Sora.EventArgs.SoraEvent;

namespace OneBot.FrameworkDemo.Modules;

/// <summary>
/// 这个是示例处理代码，所有的 OneBot 处理对象均需实现 IOneBotController 接口
/// </summary>
public class TestModule : IOneBotController
{
    private readonly ILogger<TestModule> _logger;

    public TestModule(ICommandService commandService, ILogger<TestModule> logger)
    {
        // 通过构造函数获得指令路由服务对象

        // 基本事件处理例子
        // 如果你不想要指令路由，可以使用这个方法来注册最原始的事件监听方法
        commandService.Event.OnGroupMessage += (scope) =>
        {
            var args = scope.WrapSoraEventArgs<GroupMessageEventArgs>();

            // 在控制台中复读群里的信息
            logger.LogInformation("{SourceGroup} : {Sender} : {Message}", args.SourceGroup.Id, args.Sender.Id, args.Message.RawText);

            return 0;
            // 这里返回 0，表示继续传递该事件给后续的指令或监听。
        };

        // 全局异常处理事件
        commandService.Event.OnException += (context, exception) =>
        {
            logger.LogError(exception, "{Exception}", exception.Message);
        };

        _logger = logger;
    }

    /// <summary>
    /// 指令定义例子
    /// 
    /// 定义一个指令：
    /// <para>使用 &lt; &gt; 括起来的是必选参数；</para>
    /// <para>使用 [ ] 括起来的是可选参数 </para>
    /// </summary>
    /// <param name="duration">禁言时长</param>
    /// <param name="args">全参数列表</param>
    /// <param name="userInfo">被禁言用户</param>
    /// <param name="e">原始事件信息</param>
    [Command("mute <uid> [duration]", Alias = new[] { "禁言 <uid> [duration]", "口球 <uid> [duration]" }, EventType = EventType.GroupMessage)]
    [DemoBeforeCommand]
    public void MuteInGroupWithDuration(Duration duration, [ParsedArguments] object[] args, [CommandParameter("uid")] User userInfo, GroupMessageEventArgs e)
    {
        duration ??= new(600);
        _logger.LogInformation("{Sender} 使用指令禁言 {User} 用户 {Duration} 秒。", e.Sender.Id, userInfo.Id, duration.Seconds);

        // 这个指令没有返回值，其隐式返回 1，在执行完毕后不再传递该事件给后续的指令或监听。
    }

    /// <summary>
    /// 再定义一个指令：
    /// 
    /// <para>参数不必要按顺序、参数会触发依赖注入</para>
    /// </summary>
    /// <param name="gid">操作群号</param>
    /// <param name="uid">被禁言用户</param>
    /// <param name="duration">禁言时长</param>
    /// <param name="e">原始事件信息</param>
    [Command("mute <gid> <uid> [duration]", Alias = new[] { "禁言 <gid> <uid> [duration]", "口球 <gid> <uid> [duration]" }, EventType = EventType.GroupMessage | EventType.PrivateMessage)]
    public int MuteInGroupWithGroupId(Group gid, User uid, Duration duration, BaseSoraEventArgs e)
    {
        duration ??= new(600);

        switch (e)
        {
            case GroupMessageEventArgs s1:
                _logger.LogInformation("{Sender} 使用指令禁言 {Gid} 群里的 {Uid} 用户 {Duration} 秒。", s1.Sender.Id, gid.Id, uid.Id, duration.Seconds);
                break;
            case PrivateMessageEventArgs s2:
                _logger.LogInformation("{Sender} 使用指令禁言 {Gid} 群里的 {Uid} 用户 {Duration} 秒。", s2.Sender.Id, gid.Id, uid.Id, duration.Seconds);
                break;
        }

        return 1;
        // 这里是显示返回 1，在执行完毕后不再传递该事件给后续的指令或监听。
        // return 0;
        // 当然也可以返回 0，表示继续传递。
    }

    /// <summary>
    /// 小程序简单监听方法，
    /// 这里是监听了群签到。
    /// </summary>
    /// <param name="e"></param>
    [CQJson("com.tencent.qq.checkin", EventType = EventType.GroupMessage)]
    public void CheckInListener(GroupMessageEventArgs e)
    {
        _logger.LogInformation("{Sender} 签到成功！", e.Sender.Id);

        // 当然这里也是可以返回 0 或 1 的。
    }

    /// <summary>
    /// 全局异常处理函数测试
    /// </summary>
    [Command("exception", EventType = EventType.GroupMessage | EventType.PrivateMessage)]
    public void ExceptionTest()
    {
        throw new Exception("测试全局异常处理");
    }
}

