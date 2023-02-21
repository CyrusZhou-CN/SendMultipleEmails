using LiteDB;
using Newtonsoft.Json.Linq;
using Server.Http.Controller;
using Server.Http.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class HistoryGroup:AutoObjectId
    {
      
        public string UserId { get; set; }

        // 发送人的id
        public List<string> SenderIds { get; set; }

        // 收件人的id
        public List<string> ReceiverIds { get; set; }

        // 通用的抄送人id
        public List<string> CopyToUserIds { get; set; }

        public DateTime CreateDate { get; set; }

        public string Subject { get; set; }
        public string TemplateId { get; set; }

        public string TemplateName { get; set; }

        // json 格式的数据
        public string Data { get; set; }
        public SendStatus SendStatus { get; set; }

        // 临时数据:发送成功的数量
        public int SuccessCount { get; set; }

        public override string GetFilterString()
        {
            return base.GetFilterString() + Subject + TemplateName + SenderIds.Count + ReceiverIds.Count;
        }
    }
}
