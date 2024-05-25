using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class Group : AutoObjectId
    {
        public string userId { get; set; }

        /// <summary>
        /// 父组
        /// </summary>
        [SugarColumn(IsNullable =true )]
        public string parentId { get; set; }

        public string name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string description { get; set; }

        public string groupType { get; set; } = "default";
    }
}
