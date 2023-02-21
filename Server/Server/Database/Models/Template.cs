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
        public string ImageUrl { get; set; }

        public string Html { get; set; }

        public string Name { get; set; }
        public string UserId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
