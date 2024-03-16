using Microsoft.Extensions.Options;
using Uamazing.Utils.Web.Token;
using Uamazing.UZonEmail.Server.Config.SubConfigs;

namespace Uamazing.UZonEmail.Server.Config
{
    /// <summary>
    /// 程序所有的配置
    /// </summary>
    public class AppConfig
    {
        public DatabaseConfig DataBase { get; set; }
        public HttpConfig Http { get; set; }
        public LoggerConfig Logger { get; set; }
        public SystemConfig System { get; set; }
        public UserConfig User { get; set; }
        public WebsocketConfig Websocket { get; set; }
        public TokenParams TokenParams { get; set; }
    }
}
