using OneBot.CommandRoute.Mixin;
using OneBot.CommandRoute.Models.VO;
using OneBot.CommandRoute.Services;
using OneBot.FrameworkDemo.Middleware;
using OneBot.FrameworkDemo.Modules;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
IConfiguration configuration = builder.Configuration;

#region ConfigureServices
// ���û����˺���
// ���� OneBot ����
services.Configure<CQHttpServerConfigModel>(configuration.GetSection("CQHttpConfig"))
    .ConfigureOneBot();

// ����м��
// ����ģʽ��ԭ��ģʽ�����ԣ����ⲻ��
// services.AddSingleton<IOneBotMiddleware, TestMiddleware>();
services.AddScoped<IOneBotMiddleware, TestMiddleware>()

// ���ָ�� / �¼�
// �Ƽ�ʹ�õ���ģʽ����ʵ���Ͽ�ܴ���Ҳ�ǵ�����ģʽʹ�õģ�
    .AddSingleton<IOneBotController, TestModule>();
// һ��һ�еؽ�ָ��ģ��ӽ�ȥ
#endregion

var app = builder.Build();

#region Configure
// ��Ҳ����Ҫ�ֶ����� Sora ������
// ����Ķ��� https://github.com/ParaParty/OneBot-CommandRoute-Template/issues/2
#endregion

app.Run();