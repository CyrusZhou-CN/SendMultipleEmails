using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.SME.Server.Models
{
    /// <summary>
    /// 所有的组都继承这个表
    /// </summary>
    public class EmailBoxGroup : TreeNode
    {
        /// <summary>
        /// 组类型
        /// </summary>
        public GroupType GroupType { get; set; }
    }

    public enum GroupType
    {
        /// <summary>
        /// 默认组
        /// </summary>
        DefaultGroup = 0,

        /// <summary>
        /// 发件箱组
        /// </summary>
        OutboxGroup = 1,

        /// <summary>
        /// 收件箱组
        /// </summary>
        InboxGroup = 2
    }
}
