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
       
        public string UserId { get; set; }

        /// <summary>
        /// 父组
        /// </summary>
        public string ParentId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string GroupType { get; set; } = "default";
    }
}
