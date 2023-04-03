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
        public GroupType GroupType { get; set; } = GroupType.DefaultGroup;
    }

    public enum GroupType
    {
        /// <summary>
        /// 默认组
        /// </summary>
        DefaultGroup,

        /// <summary>
        /// 发件箱组
        /// </summary>
        OutboxGroup,

        /// <summary>
        /// 收件箱组
        /// </summary>
        InboxGroup
    }
}
