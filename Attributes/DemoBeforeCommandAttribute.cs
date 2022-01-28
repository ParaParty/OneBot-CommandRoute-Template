
using OneBot.CommandRoute.Attributes;
using OneBot.CommandRoute.Models;

using Sora.Entities.Info;
using Sora.Enumeration.ApiType;
using Sora.Enumeration.EventParamsType;
using Sora.EventArgs.SoraEvent;

namespace OneBot.FrameworkDemo.Attributes;

/// <summary>
/// 在触发指令前的切片
/// </summary>
public class DemoBeforeCommandAttribute : BeforeCommandAttribute
{
    public override async void Invoke(OneBotContext context)
    {
        BaseSoraEventArgs baseSoraEventArgs = context.SoraEventArgs;

        if (baseSoraEventArgs is not GroupMessageEventArgs p) return;

        var (apiStatus, memberInfo) = await p.SoraApi.GetGroupMemberInfo(p.SourceGroup, p.Sender.Id, true);

        if (apiStatus.RetCode != ApiStatusType.Ok)
        {
            return;
        }

        GroupMemberInfo uInfo = memberInfo;

        if (uInfo.Role != MemberRoleType.Member)
        {
            Console.WriteLine($"发送者 {uInfo.UserId} 是管理员");
        }
        else
        {
            Console.WriteLine($"发送者 {uInfo.UserId} 不是管理员");
        }
    }
}
