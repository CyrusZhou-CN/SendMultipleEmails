using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using UZonMailService.Config;
using UZonMailService.Utils.DotNETCore;
using UZonMailService.Utils.Database;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddControllers()
    .AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

// ���� swagger
services.AddSwaggerGen(new OpenApiInfo()
{
    Title = "UZonMail",
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
services.Configure<AppConfig>(builder.Configuration);
// ע�����ݿ�
services.AddDbContext<SQLiteContext>();
// ע�� liteDB
services.AddLiteDB();
// ��� HttpContextAccessor���Թ� service ��ȡ��ǰ������û���Ϣ
services.AddHttpContextAccessor();
// ����ע�����
services.AddServices();

// ���� jwt ��֤
var secretKey = builder.Configuration["TokenParams:Secret"];
services.AddJWTAuthentication(secretKey);

// �رղ����Զ�����
services.Configure<ApiBehaviorOptions>(o =>
{
    o.SuppressModelStateInvalidFilter = true;
});

// ����
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
// ������վ�ĸ�Ŀ¼
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath,"Files")),
    RequestPath = "/files"
});

// ����
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
