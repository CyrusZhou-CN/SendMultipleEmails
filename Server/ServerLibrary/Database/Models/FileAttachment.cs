using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Database.Models
{
    public  class FileAttachment
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string  FileId { get; set; }
        public string UserId { get; set; }
        public string SubPath { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
    }
}
