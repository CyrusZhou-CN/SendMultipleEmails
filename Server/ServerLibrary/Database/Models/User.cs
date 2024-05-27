using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Database.Models
{
    public  class User:AutoObjectId
    {
        public string userId { get; set; }
        public string password { get; set; }
        public DateTime createDate { get; set; }=DateTime.Now;
        /// <summary>
        /// 头像
        /// </summary>
        public string avatar { get; set; } = "";
    }
}
