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
        [BsonField("userId")]
        public string UserId { get; set; }

        // 发送人的id
        [BsonField("senderIds")]
        public List<string> SenderIds { get; set; }

        // 收件人的id
        [BsonField("receiverIds")]
        public List<string> ReceiverIds { get; set; }

        // 通用的抄送人id
        [BsonField("copyToUserIds")]
        public List<string> CopyToUserIds { get; set; }

        [BsonField("createDate")]
        public DateTime CreateDate { get; set; }

        [BsonField("subject")]
        public string Subject { get; set; }

        [BsonField("templateId")]
        public string TemplateId { get; set; }

        [BsonField("templateName")]
        public string TemplateName { get; set; }

        // json 格式的数据
        [BsonField("data")]
        public string Data { get; set; }

        [BsonField("sendStatus")]
        public SendStatus SendStatus { get; set; }

        // 临时数据:发送成功的数量
        [BsonField("successCount")]
        public int SuccessCount { get; set; }

        public override string GetFilterString()
        {
            return base.GetFilterString() + Subject + TemplateName + SenderIds.Count + ReceiverIds.Count;
        }
    }
}
