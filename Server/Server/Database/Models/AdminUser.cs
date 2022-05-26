using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    /// <summary>
    /// 管理员账号
    /// </summary>
    internal class AdminUser:AutoObjectId
    {
        /// <summary>
        /// 管理员账号
        /// </summary>
        [BsonRef]
        public User User { get; set; }
    }
}
