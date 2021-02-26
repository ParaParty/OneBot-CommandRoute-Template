using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using OneBot.CommandRoute.Models.VO;
using OneBot.CommandRoute.Services;
using OneBot.CommandRoute.Services.Implements;
using OneBot.FrameworkDemo.Modules;
using OneBot_CommandRoute.CommandRoute.Utils;
using YukariToolBox.FormatLog;


namespace OneBot.FrameworkDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // 配置机器人核心

            // 设置 OneBot 配置
            services.Configure<CQHttpServerConfigModel>(Configuration.GetSection("CQHttpConfig"));

            // 设置 OneBot 客户端（Sora）
            services.AddSingleton<IBotService, BotService>();

            // 设置指令路由服务
            services.AddSingleton<ICommandService, CommandService>();

            // 设置日志服务，将 Sora 日志服务设置 Microsoft.Extensions.Logging.ILogger
            services.AddSingleton<ILogService, YukariToolBoxLogger>();

            // 添加指令 / 事件
            // 使用单例模式
            services.AddSingleton<IOneBotController, TestModule>();
            // 一行一行地将指令模块加进去
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 初始化
            var serviceProvider = app.ApplicationServices;

            // 初始化机器人核心
            var soraService = serviceProvider.GetService<IBotService>();
            soraService.Start();
        }
    }
}
