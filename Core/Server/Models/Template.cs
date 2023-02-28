using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.SME.Server.Models
{
    /// <summary>
    /// 模板集合
    /// </summary>
    public class Template : AutoObjectId
    {
        /// <summary>
        /// 模板名称，也是唯一标识
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// html 内容
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// 关联的主题
        /// 当使用随机模板、非固定标题时，使用该主题
        /// </summary>
        public List<string> Subjects { get; set; }
       
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
