using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using UZonMailService.Config;
using UZonMailService.Utils.DotNETCore;
using UZonMailService.Utils.Database;
using UZonMailService.Models.SqlLite;
using Microsoft.EntityFrameworkCore;
using UZonMailService.Models.SqlLite.Init;
using UZonMailService.Utils.DotNETCore.Filters;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddControllers(option =>
{
    option.Filters.Add(new KnownExceptionFilter());
})
.AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

// 配置 swagger
services.AddSwaggerGen(new OpenApiInfo()
{
    Title = "UZonMail",
    Contact = new OpenApiContact()
    {
        Name = "galens",
        Url = new Uri("https://galens.uamazing.cn"),
        Email = "gmx_galens@163.com"
    }
}, "Server.xml");

// 添加 signalR
services.AddSignalR();
// 设置 hyphen-case 路由
services.SetupSlugifyCaseRoute();
// 绑定配置
services.Configure<AppConfig>(builder.Configuration);
// 注入数据库
services.AddDbContext<SqlContext>();
// 注入 liteDB
//services.AddLiteDB();
// 添加 HttpContextAccessor，以供 service 获取当前请求的用户信息
services.AddHttpContextAccessor();
// 批量注册服务
services.AddServices();

// 配置 jwt 验证
var secretKey = builder.Configuration["TokenParams:Secret"];
services.AddJWTAuthentication(secretKey);
// 配置接口鉴权策略
services.AddAuthorizationBuilder()
               // 超管
               .AddPolicy("RequireAdmin", policy => policy.RequireClaim(ClaimTypes.Role, "admin"));

// 关闭参数自动检验
services.Configure<ApiBehaviorOptions>(o =>
{
    o.SuppressModelStateInvalidFilter = true;
});

// 跨域
services.AddCors(options =>
{
    // 获取跨域配置
    string[]? corsConfig = builder.Configuration.GetSection("Cors").Get<string[]>();
    List<string> cors = ["http://localhost:9000"];
    if (corsConfig?.Length > 0) cors.AddRange(corsConfig);

    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(cors.ToArray())
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

// 修改文件上传大小限制
services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
});
// 配置 Kestrel 服务器
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});


var app = builder.Build();


app.UseDefaultFiles();
// 设置网站的根目录
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "public")),
    RequestPath = "/public"
});

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

// 初始数据库
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var context = serviceProvider.GetRequiredService<SqlContext>();

    // 应用迁移
    //context.Database.Migrate();

    var appConfig = serviceProvider.GetRequiredService<IOptions<AppConfig>>();
    // 初始数据
    var initDb = new InitDatabase(app.Environment, context, appConfig.Value);
    initDb.Init();
}


app.Run();
