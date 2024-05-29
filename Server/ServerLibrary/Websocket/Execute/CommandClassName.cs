using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Execute
{
    /// <summary>
    /// 命令类的键值
    /// </summary>
    public enum CommandClassName
    {
        None,

        /// <summary>
        /// 登陆模块
        /// </summary>
        Login,

        /// <summary>
        /// 注销用户
        /// </summary>
        Logout,

        /// <summary>
        /// 选择文件
        /// </summary>
        SelectFiles,

        /// <summary>
        /// 删除文件
        /// </summary>
        DeleteFile,
    }
}
