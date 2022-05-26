using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    /// <summary>
    /// 邮箱信息
    /// </summary>
    public class EmailInfo : AutoObjectId
    {
        /// <summary>
        /// name 具有唯一性
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 邮箱号
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 邮箱组 Id
        /// </summary>
        public string GroupId { get; set; }

        public override string GetFilterString()
        {
            return $"{base.GetFilterString()}{UserName}{Email}";
        }
    }
}
