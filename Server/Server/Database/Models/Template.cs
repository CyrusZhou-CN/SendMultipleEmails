using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    /// <summary>
    /// 模板集合
    /// </summary>
    public class Template : AutoObjectId
    {
        /// <summary>
        /// 图片 url
        /// 如果是一张图片，就通过这个发送
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// HTML 内容
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 所属用户名称
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
