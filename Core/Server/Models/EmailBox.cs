using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uamazing.Utils.Database.LiteDB;

namespace Uamazing.SME.Server.Models
{
    /// <summary>
    /// 邮箱信息
    /// </summary>
    public class EmailBox : AutoObjectId
    {
        /// <summary>
        /// 邮箱名称
        /// 具有唯一性
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 邮箱组 id
        /// </summary>
        public string GroupId { get; set; }


        public override string GetFilterString()
        {
            return $"{base.GetFilterString()}{Description}{Email}";
        }
    }
}
