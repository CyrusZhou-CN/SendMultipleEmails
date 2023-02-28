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
    public abstract class Group : LinkingUserId
    {
        /// <summary>
        /// 父组
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 全路径
        /// </summary>
        public string Path { get; set; }
    }
}
