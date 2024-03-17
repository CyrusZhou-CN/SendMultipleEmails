using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.SME.Server.Config
{
    /// <summary>
    /// 系统配置
    /// </summary>
    internal class ConfigContainer
    {
        private ConfigContainer() { }

        private string _appDir = string.Empty;
        /// <summary>
        /// 程序根目录
        /// </summary>
        public string RootDir
        {
            get
            {
                if (string.IsNullOrEmpty(_appDir))
                {
                    _appDir = AppContext.BaseDirectory;
                }
                return _appDir;
            }
        }

        public SytemConfig Sytem { get; set; }

        public HttpConfig Http { get; set; }

        public WebsocketConfig Websocket { get; set; }

        public DatabaseConfig Database { get; set; }

        public LoggerConfig Logger { get; set; }

        public UserConfig User { get; set; }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <returns></returns>
        public static ConfigContainer LoadConfig()
        {
            // config 路径为 Config/xx.config.json
            // 如果有多个 config.json 文件，会将这个配置合并

            var configDir = Path.Combine(AppContext.BaseDirectory, "Config");
            if (!Directory.Exists(configDir)) throw new ArgumentNullException("Config 目录不存在");

            var jObject = Utils.Json.JsonHelper.ReadAndMergeJsonFiles(configDir, "*.config.json");
            return jObject.ToObject<ConfigContainer>(new JsonSerializer()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}
