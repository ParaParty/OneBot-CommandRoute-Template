using OneBot.CommandRoute.Mixin;
using OneBot.CommandRoute.Models.VO;
using OneBot.CommandRoute.Services;
using OneBot.FrameworkDemo.Middleware;
using OneBot.FrameworkDemo.Modules;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
IConfiguration configuration = builder.Configuration;

#region ConfigureServices
// 配置机器人核心
// 设置 OneBot 配置
services.Configure<CQHttpServerConfigModel>(configuration.GetSection("CQHttpConfig"))
    .ConfigureOneBot();

// 添加中间件
// 单例模式或原型模式都可以，问题不大。
// services.AddSingleton<IOneBotMiddleware, TestMiddleware>();
services.AddScoped<IOneBotMiddleware, TestMiddleware>()

// 添加指令 / 事件
// 推荐使用单例模式（而实际上框架代码也是当单例模式使用的）
    .AddSingleton<IOneBotController, TestModule>();
// 一行一行地将指令模块加进去
#endregion

var app = builder.Build();

#region Configure
// 再也不需要手动启动 Sora 服务了
// 具体改动见 https://github.com/ParaParty/OneBot-CommandRoute-Template/issues/2
#endregion

app.Run();