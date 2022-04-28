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
    public class Template:AutoObjectId
    {
        [BsonField("imageUrl")]
        public string ImageUrl { get; set; }

        [BsonField("html")]
        public string Html { get; set; }

        [BsonField("name")]
        public string Name { get; set; }

        [BsonField("userId")]
        public string UserId { get; set; }

        [BsonField("createDate")]
        public DateTime CreateDate { get; set; }
    }
}
