using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Uamazing.SME.Server.Config;
using Uamazing.Utils.DotNETCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

// 配置 swagger
services.AddSwaggerGen(new OpenApiInfo()
{
    Title = "SendMutipleEmails",
    Contact = new OpenApiContact()
    {
        Name = "galens",
        Url = new Uri("https://galens.uamazing.cn"),
        Email = "gmx_galens@163.com"
    }
},"Server.xml");

// 添加 signalR
services.AddSignalR();

// 设置 hyphen-case 路由
services.SetupSlugifyCaseRoute();

// 绑定配置
builder.MapConfiguration(new ConfigurationMapper());

// 批量注册服务
services.MapServices();

// 配置 jwt 验证
var secretKey = builder.Configuration["TokenParams:Secret"];
services.AddJWTAuthentication(secretKey);

// 关闭参数自动检验
services.Configure<ApiBehaviorOptions>(o =>
{
    o.SuppressModelStateInvalidFilter = true;
});

// 注册 liteDB
builder.AddLiteDB();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
