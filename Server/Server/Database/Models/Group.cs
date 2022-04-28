using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class Group:AutoObjectId
    {
        [BsonField("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// 父组
        /// </summary>
        [BsonField("parentId")]
        public string ParentId { get; set; }

        [BsonField("name")]
        public string Name { get; set; }

        [BsonField("description")]
        public string Description { get; set; }

        [BsonField("groupType")]
        public string GroupType { get; set; } = "default";
    }
}
