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
        [BsonField("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// 邮箱名称
        /// </summary>
        [BsonField("email")]
        public string Email { get; set; }

        /// <summary>
        /// 组 Id
        /// </summary>
        [BsonField("groupId")]
        public string GroupId { get; set; }

        public override string GetFilterString()
        {
            return $"{base.GetFilterString()}{UserName}{Email}";
        }
    }
}
