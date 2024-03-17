using System;
using Stylet;
using StyletIoC;
using log4net.Core;
using log4net;
using Server.Pages;
using Uamazing.SME.Server.Pages;
using Uamazing.SME.Server.Config;
using Uamazing.SME.Server.Database;
using Uamazing.SME.Server.Http;

namespace Uamazing.SME.Server
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Bootstrapper));
        private HttpServiceMain _httpServer;
        private Websocket.WebsocketServiceMain _websocket;

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Configure the IoC container in here
            // 配置
            var config = ConfigContainer.LoadConfig();
            builder.Bind<ConfigContainer>().ToInstance(config);

            // 数据库
            var liteDb = new LiteDBManager(config.Database.LiteDbPath);
            builder.Bind<LiteDBManager>().ToInstance(liteDb);

            // service 注册
            base.ConfigureIoC(builder);
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts

            // 加载静态网页服务
            _httpServer = new HttpServiceMain();
            _httpServer.Start(Container);

            // 加载 websocket
            _websocket = new Websocket.WebsocketServiceMain(Container);
        }

        protected override void OnStart()
        {
            // 检查 webview2 环境
            //using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3C4FE00-EFD5-403B-9569-398A20F1BA4A}"))
            //{
            //    if (key == null)
            //    {
            //        System.Windows.Forms.MessageBox.Show("环境缺失","本系统需要 webview2 运行环境，请先安装！");
            //        // 退出程序
            //    }
            //}

            Stylet.Logging.LogManager.Enabled = true;

            // 添加对所有未捕获异常的读取
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Error("未捕获异常:" + e.ExceptionObject.ToString());
        }
    }
}
