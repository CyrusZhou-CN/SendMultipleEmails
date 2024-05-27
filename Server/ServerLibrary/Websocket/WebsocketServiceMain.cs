using System;
using System.Collections.Generic;
using ServerLibrary.Protocol;
using System.Collections.Concurrent;
using System.Threading;
using ServerLibrary.Execute;
using SuperSocket.WebSocket;
using log4net.Config;
using log4net;
using System.IO;
using ServerLibrary.Config;
using StyletIoC;
using System.Runtime.InteropServices;
using Swan.Logging;

namespace ServerLibrary.Websocket
{
    public class WebsocketServiceMain
    {
        private WebSocketServer _websocketServer;

        // 收到的消息
        public ConcurrentQueue<ReceivedMessage> Queue { get; private set; }

        private Thread _thread = null;

        // 线程锁
        private EventWaitHandle _waitHandle = null;

        // ioc 容器
        private IContainer _container;

        public void Start(IContainer container)
        {
            _container = container;

            UserConfig userConfig = container.Get<UserConfig>();
            _websocketServer = new WebSocketServer();

            _websocketServer.NewMessageReceived += Ws_NewMessageReceived;//当有信息传入时

            _websocketServer.NewSessionConnected += Ws_NewSessionConnected;//当有用户连入时

            _websocketServer.SessionClosed += Ws_SessionClosed;//当有用户退出时

            _websocketServer.NewDataReceived += Ws_NewDataReceived;//当有数据传入时

            // 设置参数
            var serverConfig = new SuperSocket.SocketBase.Config.ServerConfig()
            {
                Ip = "Any",
                Port = userConfig.WebsocketPort,
                MaxRequestLength = 1024000,
            };


            if (_websocketServer.Setup(serverConfig))//绑定端口
            {
                if (_websocketServer.Start())//启动服务
                {
                    Logger.Info("Web Socket Server 启动成功");

                    //打开另一个线程监听数据的变动
                    _waitHandle = new AutoResetEvent(false);
                    _thread = new Thread(new ThreadStart(UnpakageData))
                    {
                        IsBackground = true,
                    };
                    _thread.Start();

                    Queue = new ConcurrentQueue<ReceivedMessage>();
                }
                else Logger.Info("Web Socket Server 启动失败");
            }
            else
            {
                Logger.Info("Web Socket Server 初始化失败");
            }
        }
        public void Stop()
        {
            _thread?.Abort();
            _thread= null;
            _websocketServer?.Stop();
            _websocketServer?.Dispose();
            _websocketServer = null;
            Logger.Info("Web Socket Server stopped.");
        }
        private void UnpakageData()
        {
            while (true)
            {
                // 等待信号通知
                _waitHandle.WaitOne();

                // 判断是否有内容需要如磁盘 从列队中获取内容，并删除列队中的内容
                while (Queue.TryDequeue(out ReceivedMessage data))
                {
                    //响应消息                    
                    if (data != null) ExecuteTrigger.Instance.Execute(data);
                }
                //重新设置信号
                _waitHandle.Reset();
                Thread.Sleep(1);
            }
        }

        private void Ws_NewDataReceived(WebSocketSession session, byte[] value)
        {
            Console.WriteLine("Ws_NewDataReceived");

        }

        private void Ws_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            Console.WriteLine("Ws_SessionClosed");
        }

        private void Ws_NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine("Ws_NewSessionConnected");
        }

        private void Ws_NewMessageReceived(WebSocketSession session, string value)
        {
            ReceivedMessage receivedMessage = new ReceivedMessage(session, value);
            //放入线程池中
            Queue.Enqueue(receivedMessage);
            _waitHandle.Set();
        }
    }
}
