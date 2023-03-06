namespace Uamazing.SME.Server.Utils.DotNetCoreSetup
{
    public abstract class ConfigurationMapperBase
    {
        public abstract IServiceCollection Map(WebApplicationBuilder builder);
    }
}
