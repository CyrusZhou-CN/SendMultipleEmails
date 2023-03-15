using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Uamazing.Utils.DotNETCore.Configure
{
    public abstract class ConfigurationMapperBase
    {
        public abstract IServiceCollection Map(WebApplicationBuilder builder);
    }
}
