﻿using System.Collections.Generic;
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
    class SelectFiles : IWebsocketCommand
    {
        public ILog Logger { get; set; }
        public string Name => CommandClassName.SelectFiles.ToString();

        public void ExecuteCommand(ReceivedMessage message)
        {
            // 解析客户端发送的文件数据
            var fileData = JsonConvert.DeserializeObject<FileData>(message.JObject.ToString());

            if (fileData != null)
            {
                string extension = Path.GetExtension(fileData.fileName);
                // 保存文件到指定路径
                var fullName = Path.Combine($"{Guid.NewGuid()}{extension}");
                var savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UserConfig.Instance.AttachmentPath, fullName);
                var fileBytes = Convert.FromBase64String(fileData.fileContent);

                File.WriteAllBytes(savePath, fileBytes);
                var result =new ResultInfo { fileName = fileData.fileName, fullName = fullName };

                // 发送响应给客户端
                message.Response(new Response(message.Body)
                {
                    ignoreError = true,
                    result = result,
                    status = 200,
                });
            }
            else
            {
                message.Response(new Response(message.Body)
                {
                    ignoreError = true,
                    result = new ResultInfo(),
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
