using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.SME.Server.Model
{
    /// <summary>
    /// 用户关联表
    /// 所有的表都应继承该类
    /// </summary>
    public class LinkingUserId:AutoObjectId
    {
        /// <summary>
        /// 用户名
        /// 全局唯一
        /// </summary>
        public string UserId { get; set; }  
    }
}
