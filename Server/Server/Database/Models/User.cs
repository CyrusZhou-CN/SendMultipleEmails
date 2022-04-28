using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public  class User:AutoObjectId
    {
        [BsonField("userId")]
        public string UserId { get; set; }

        [BsonField("password")]
        public string Password { get; set; }

        [BsonField("createDate")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [BsonField("avatar")]
        public string Avatar { get; set; }
    }
}
