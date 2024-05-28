using System.Collections.Generic;
using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using ServerLibrary.Execute;
using ServerLibrary.Protocol;
using SuperSocket.SocketBase.Logging;
using SqlSugar;
using ServerLibrary.Config;

namespace ServerLibrary.Websocket.Commands
{
    class DeleteFile : IWebsocketCommand
    {
        public ILog Logger { get; set; }
        public string Name => CommandClassName.DeleteFile.ToString();

        public void ExecuteCommand(ReceivedMessage message)
        {
            // 解析客户端发送的文件数据
            var fileData = JsonConvert.DeserializeObject<FileData>(message.JObject.ToString());

            if (fileData != null)
            {
                var savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UserConfig.Instance.AttachmentPath, fileData.fileName);
                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                }
                // 发送响应给客户端
                message.Response(new Response(message.Body)
                {
                    ignoreError = true,
                    result = "OK",
                    status = 200,
                });
            }
            else
            {
                message.Response(new Response(message.Body)
                {
                    ignoreError = true,
                    result = "Error",
                    status = 400,
                });
            }
        }
        public class ResultInfo
        {
            public string fileName { get; internal set; }
            public string fullName { get; internal set; }
        }
        // 定义文件数据结构
        public class FileData
        {
            public string fileName { get; set; }
            public string fileContent { get; set; }
        }
    }
}
