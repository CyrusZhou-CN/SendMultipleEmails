using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Uamazing.SME.Server.Config;
using Uamazing.Utils.DotNETCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddControllers()
    .AddNewtonsoftJson();

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

// 跨域
services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://127.0.0.1:9528", "http://localhost:9528")
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

var app = builder.Build();


app.UseDefaultFiles();
// 设置网站的根目录
app.UseStaticFiles();

// 跨域
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
