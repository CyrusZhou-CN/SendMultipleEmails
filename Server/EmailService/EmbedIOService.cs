using ServerLibrary.Config;
using ServerLibrary.Http;
using ServerLibrary.Websocket;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public partial class EmbedIOService : ServiceBase
    {
        private readonly HttpServiceMain _httpServiceMain;
        private readonly WebsocketServiceMain _websocketServiceMain;
        private readonly StyletIoC.IContainer container;

        public EmbedIOService()
        {
            InitializeComponent();
            container = Program.CreateContainer();
            _httpServiceMain = container.Get<HttpServiceMain>();
            _websocketServiceMain = container.Get<WebsocketServiceMain>();
        }

        protected override void OnStart(string[] args)
        {
            _httpServiceMain.Start(container);  // 传入容器实例
            _websocketServiceMain.Start(container);
        }

        protected override void OnStop()
        {
            _httpServiceMain.Stop(); 
            _websocketServiceMain.Stop();
        }
    }
}
