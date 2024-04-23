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
}, "Server.xml");

// ��� signalR
services.AddSignalR();
// ���� hyphen-case ·��
services.SetupSlugifyCaseRoute();
// ������
services.Configure<AppConfig>(builder.Configuration);
// ע�����ݿ�
services.AddDbContext<SqlContext>();
// ע�� liteDB
//services.AddLiteDB();
// ��� HttpContextAccessor���Թ� service ��ȡ��ǰ������û���Ϣ
services.AddHttpContextAccessor();
// ����ע�����
services.AddServices();

// ���� jwt ��֤
var secretKey = builder.Configuration["TokenParams:Secret"];
services.AddJWTAuthentication(secretKey);
// ���ýӿڼ�Ȩ����
services.AddAuthorizationBuilder()
               // ����
               .AddPolicy("RequireAdmin", policy => policy.RequireClaim(ClaimTypes.Role, "admin"));

// �رղ����Զ�����
services.Configure<ApiBehaviorOptions>(o =>
{
    o.SuppressModelStateInvalidFilter = true;
});

// ����
services.AddCors(options =>
{
    // ��ȡ��������
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

// �޸��ļ��ϴ���С����
services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
});
// ���� Kestrel ������
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});


var app = builder.Build();


app.UseDefaultFiles();
// ������վ�ĸ�Ŀ¼
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "public")),
    RequestPath = "/public"
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

// ��ʼ���ݿ�
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var context = serviceProvider.GetRequiredService<SqlContext>();

    // Ӧ��Ǩ��
    //context.Database.Migrate();

    var appConfig = serviceProvider.GetRequiredService<IOptions<AppConfig>>();
    // ��ʼ����
    var initDb = new InitDatabase(app.Environment, context, appConfig.Value);
    initDb.Init();
}


app.Run();
