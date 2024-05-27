using System;
using ServerLibrary.Config;
using ServerLibrary.Http;
using Stylet;

namespace Server.Pages
{
    public class ShellViewModel : Screen
    {        
        public string Url { get; set; }
        public ShellViewModel(UserConfig userConfig)
        {
            // 从配置里面读取
            Url = $"{userConfig.HttpHost}:{userConfig.HttpPort}";
        }
    }
}
