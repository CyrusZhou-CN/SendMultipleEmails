using Microsoft.EntityFrameworkCore;

namespace UZonMailService.Models.SqlLite.Emails
{
    /// <summary>
    /// 代理类
    /// 格式为：username:password@host:port
    /// </summary>
    [Keyless]
    public class EmailProxy
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string? Password { get; set; }

        public EmailProxy() { }
        public EmailProxy(string proxyString)
        {
            // 将字符串转换为代理
            Uri uri = new(proxyString);
            Host = uri.Host;
            Port = uri.Port;
            var userInfos = uri.UserInfo.Split(':');
            if (userInfos.Length > 0)
            {
                Username = userInfos[0];                
            }
            if (userInfos.Length > 1)
            {
                Password = userInfos[1];
            }
        }

        /// <summary>
        /// 与字符串的转换
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Username}:{Password}@{Host}:{Port}";
        }
    }
}
