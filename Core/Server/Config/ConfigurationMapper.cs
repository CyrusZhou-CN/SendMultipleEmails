using Microsoft.Extensions.Options;
using Uamazing.SME.Server.Utils.DotNetCoreSetup;

namespace Uamazing.SME.Server.Config
{
    public class ConfigurationMapper : ConfigurationMapperBase
    {
        private WebApplicationBuilder _builder = null;
        public override IServiceCollection Map(WebApplicationBuilder builder)
        {
            _builder = builder;
            var services = builder.Services;

            // 系统配置
            services.Configure<SystemConfig>(GetSection<SystemConfig>())
                .Configure<HttpConfig>(GetSection<HttpConfig>())
                .Configure<DatabaseConfig>(GetSection<DatabaseConfig>())
                .Configure<UserConfig>(GetSection<UserConfig>())
                .Configure<WebsocketConfig>(GetSection<WebsocketConfig>())
                .Configure<LoggerConfig>(GetSection<LoggerConfig>());

            return builder.Services;
        }

        public IConfigurationSection GetSection<T>(string sectionName="")
        {
            string sectionNameTemp = sectionName;
            if (string.IsNullOrEmpty(sectionName)) sectionNameTemp = typeof(T).Name.Replace("Config","");

            return _builder.Configuration.GetSection(sectionNameTemp);
        }
    }
}
