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

// ���� swagger
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

// ��� signalR
services.AddSignalR();

// ���� hyphen-case ·��
services.SetupSlugifyCaseRoute();

// ������
builder.MapConfiguration(new ConfigurationMapper());

// ����ע�����
services.MapServices();

// ���� jwt ��֤
var secretKey = builder.Configuration["TokenParams:Secret"];
services.AddJWTAuthentication(secretKey);

// �رղ����Զ�����
services.Configure<ApiBehaviorOptions>(o =>
{
    o.SuppressModelStateInvalidFilter = true;
});

// ע�� liteDB
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
