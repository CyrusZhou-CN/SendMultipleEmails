using ServerLibrary.Config;
using ServerLibrary.Database;
using ServerLibrary.Http;
using ServerLibrary.Websocket;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                // 以控制台应用程序的方式启动服务
                RunAsConsoleApp();
            }
            else
            {
                // 以 Windows 服务的方式启动服务
                RunAsWindowsService();
            }
        }

        private static void RunAsConsoleApp()
        {
            var container = CreateContainer();
            var httpServiceMain = container.Get<HttpServiceMain>();
            var websocketServiceMain = container.Get<WebsocketServiceMain>();
            httpServiceMain.Start(container);  // 传入容器实例
            websocketServiceMain.Start(container);


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            httpServiceMain.Stop();
            websocketServiceMain.Stop();
        }

        private static void RunAsWindowsService()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new EmbedIOService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        public static IContainer CreateContainer()
        {
            // 初始化依赖注入容器
            var builder = new StyletIoC.StyletIoCBuilder();

            // 注册配置和其他依赖
            builder.Bind<UserConfig>().ToInstance(UserConfig.Instance);
            builder.Bind<HttpServiceMain>().ToSelf().InSingletonScope();
            builder.Bind<WebsocketServiceMain>().ToSelf().InSingletonScope();
            var userConfig = ConfigHelper.GetDatabaseConfig();
            builder.Bind<DatabaseConfig>().ToInstance(userConfig);
            return builder.BuildContainer();
        }
    }
}
