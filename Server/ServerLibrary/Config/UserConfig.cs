﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Config
{
    public class UserConfig
    {
        private static UserConfig _instance;
        public static UserConfig Instance
        {
            get
            {
                if (_instance == null) _instance = new UserConfig();

                return _instance;
            }
        }

        public UserConfig()
        {
            // 读取配置文件，如果有的话，会覆盖原来的配置

            // 检查磁盘目录是否存在，不存在要新建

            var dirPath = Path.GetDirectoryName(HttpLogPath);
            Directory.CreateDirectory(dirPath);
            dirPath = Path.GetDirectoryName(WebsocketLogPath);
            Directory.CreateDirectory(dirPath);
            dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AttachmentPath);
            if (!System.IO.Directory.Exists(AttachmentPath))
            {
                System.IO.Directory.CreateDirectory(AttachmentPath);
            }
        }


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
                    string fullName = Process.GetCurrentProcess().MainModule.FileName;
                    string dirName = Path.GetDirectoryName(fullName);
                }
                return _appDir;
            }
            private set
            {
                _appDir = value;
            }
        }

        public int HttpPort { get; private set; } = 22345;

        public int WebsocketPort { get; private set; } = 22346;

        public string StaticName { get; private set; } = "public";

        public string BaseRoute { get; private set; } = "/api/v1";

        public string HttpLogPath { get; set; } = "Logs\\httpLog.txt";

        public string AttachmentPath { get; set; } = "Attachment";

        public string WebsocketLogPath { get; set; } = "Logs\\wsLog.txt";

        public string TokenSecret { get; set; } = "helloworld001";

        public string DefaultAvatar { get; set; } = "https://i.loli.net/2021/08/13/uOIcVFAlDbYRiCk.png";

        public string HttpHost = "http://127.0.0.1";
    }
}
