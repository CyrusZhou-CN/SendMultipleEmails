using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    /// <summary>
    /// 邮箱分组
    /// </summary>
    public class EmailGroup:AutoObjectId
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 父组
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        public string Name { get; set; }

       /// <summary>
       /// 组的描述
       /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 组的类型
        /// 默认为为发件组
        /// </summary>
        public GroupType GroupType { get; set; } = GroupType.Send;
    }

    /// <summary>
    /// 组类型
    /// </summary>
    public enum GroupType
    {
        /// <summary>
        /// 发件组
        /// </summary>
        Send,

        /// <summary>
        /// 收件组
        /// </summary>
        Receive,
    }
}
