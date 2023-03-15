using LiteDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Uamazing.Utils.DotNETCore.Configure;
using Uamazing.Utils.DotNETCore.Convention;
using Uamazing.Utils.DotNETCore.Service;

namespace Uamazing.Utils.DotNETCore
{
    public static class Index
    {
        /// <summary>
        /// 设置 slugify-case 形式的路由
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection SetupSlugifyCaseRoute(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(
                                             new SlugifyParameterTransformer()));
            });
            return services;
        }

        /// <summary>
        /// 将配置映射成对象
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configurationMapper"></param>
        /// <returns></returns>
        public static IServiceCollection MapConfiguration(this WebApplicationBuilder builder, ConfigurationMapperBase configurationMapper)
        {
            return configurationMapper.Map(builder);
        }

        /// <summary>
        /// 批量映射服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection MapServices(this IServiceCollection services)
        {
            // 批量注入 Services 单例
            var serviceBaseType = typeof(IService);          
            var serviceTypeList = Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(x => !x.IsAbstract && serviceBaseType.IsAssignableFrom(x))
                .ToList();
            serviceTypeList.ForEach(type => services.AddTransient(type));
            return services;
        }

        /// <summary>
        /// 配置 swagger
        /// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        /// </summary>
        /// <param name="services"></param>
        /// <param name="apiInfo"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerGen(this IServiceCollection services, OpenApiInfo apiInfo,string xmlCommentsPath)
        {
            services.AddSwaggerGen(swaggerOptions =>
            {
                swaggerOptions.SwaggerDoc("v1", apiInfo);

                // Set the comments path for the Swagger JSON and UI.    
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsPath);
                swaggerOptions.IncludeXmlComments(xmlPath);

                // Bearer 的scheme定义
                var securityScheme = new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    //参数添加在头部
                    In = ParameterLocation.Header,
                    //使用Authorize头部
                    Type = SecuritySchemeType.Http,
                    //内容为以 bearer开头
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                };

                //把所有方法配置为增加bearer头部信息
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                           Reference = new OpenApiReference
                           {
                               Type = ReferenceType.SecurityScheme,
                               Id = "bearerAuth"
                           }
                        },
                        Array.Empty<string>()
                    }
                };

                //注册到swagger中
                swaggerOptions.AddSecurityDefinition("bearerAuth", securityScheme);
                swaggerOptions.AddSecurityRequirement(securityRequirement);
            });

            return services;
        }

        /// <summary>
        /// 配置 jwt 验证
        /// </summary>
        /// <param name="services"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static IServiceCollection AddJWTAuthentication(this IServiceCollection services,string secretKey)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // 是否验证令牌有效期
                    ValidateLifetime = true,
                    // 每次颁发令牌，令牌有效时间
                    ClockSkew = TimeSpan.FromMinutes(120)
                };
            });
            return services;
        }

        /// <summary>
        /// 注册 LiteDB 实例
        /// liteDB 数据库文件名在配置中用 Database:LiteDbPath 指定，相对于程序目录
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddLiteDB(this WebApplicationBuilder builder)
        {
            // 从配置中读取 LiteDB 设置
            var dbPath = builder.Configuration["Database:LiteDbPath"];
            if (dbPath == null) throw new ArgumentNullException("Database:LiteDbPath未定义");
            var fullPath = Path.Combine(AppContext.BaseDirectory, dbPath);
            // 创建目录
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // 初始化 litedb
            var instance = new LiteRepository(new ConnectionString()
            {
                Filename = fullPath,
                Upgrade = true
            }, new BsonMapper() { }.UseCamelCase());
            builder.Services.AddSingleton<ILiteRepository>(instance);
            builder.Services.AddSingleton(instance.Database);

            return builder.Services;
        }
    }
}
