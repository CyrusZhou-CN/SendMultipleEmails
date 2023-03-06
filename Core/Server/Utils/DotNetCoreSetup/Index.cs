using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Uamazing.SME.Server.Utils.DotNetCoreSetup
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
    }
}
