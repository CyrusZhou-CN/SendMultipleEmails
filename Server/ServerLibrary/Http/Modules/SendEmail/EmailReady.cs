using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerLibrary.Config;
using ServerLibrary.Database;
using ServerLibrary.Database.Extensions;
using ServerLibrary.Database.Models;
using ServerLibrary.Http.Definitions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Http.Modules.SendEmail
{
    class EmailReady : EmailPreview
    {
        public static bool CreateEmailReady(string userId, JToken data, ISqlSugarClient sqlDb, out string message)
        {
            // 判断是否有发送任务正在进行
            if (InstanceCenter.SendTasks[userId] != null && !InstanceCenter.SendTasks[userId].SendStatus.HasFlag(SendStatus.SendFinish))
            {
                message = "有邮件正在发送中";
                return false;
            }

            EmailReady temp = new EmailReady(userId, data, sqlDb);

            InstanceCenter.EmailReady.Upsert(userId, temp);

            message = "success";
            return true;
        }
        private string _userId;
        public EmailReady(string userId, JToken data, ISqlSugarClient sqlDb) : base(data, sqlDb)
        {
            _userId = userId;
        }
        private GenerateInfo _info;

        public override GenerateInfo Generate()
        {
            _info = new GenerateInfo();

            // 覆盖生成逻辑,不能用异步，因为要计算实际发件量
            GenerateSendItems();

            return _info;
        }

        /// <summary>
        /// 生成之后的操作
        /// </summary>
        /// <param name="sendItems"></param>
        /// <param name="receiveBoxes"></param>
        protected override void PreviewItemCreated(List<SendItem> sendItems, List<ReceiveBox> receiveBoxes)
        {
            List<SendBox> senders = TraverseSendBoxes(Senders);

            // 添加历史
            HistoryGroup historyGroup = new HistoryGroup()
            {
                userId = _userId,
                createDate = DateTime.Now,
                subject = Subject,
                data = JsonConvert.SerializeObject(Data),
                receiverIds = receiveBoxes.ConvertAll(rec => rec._id),
                templateId = Template._id,
                templateName = Template.name,
                senderIds = senders.ConvertAll(s => s._id),
                sendStatus = SendStatus.Sending,
            };
            try
            {
                // 使用事务处理插入操作
                SqlDb.Ado.BeginTran();
                // 插入历史记录
                SqlDb.Insert(historyGroup);

                // 设置返回信息
                _info.historyId = historyGroup._id;
                _info.selectedReceiverCount = (Receivers == null || Receivers.Count < 1) ? 0 : receiveBoxes.Count;
                _info.dataReceiverCount = Data.Count;
                _info.acctualReceiverCount = sendItems.Count;
                _info.ok = true;
                _info.senderCount = senders.Count;

                // 为每个发送项目设置历史记录ID，并批量插入
                sendItems.ForEach(item => item.historyId = historyGroup._id);
                SqlDb.Fastest<SendItem>().PageSize(1000).BulkCopy(sendItems);

                // 提交事务
                SqlDb.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                // 回滚事务
                SqlDb.Ado.RollbackTran();
                throw ex;
            }
        }

        /// <summary>
        /// 通过_id来获取发件人
        /// </summary>
        /// <param name="senderIds"></param>
        /// <returns></returns>
        private List<SendBox> TraverseSendBoxes(JArray senderIds)
        {
            List<SendBox> sendBoxes = new List<SendBox>();
            // 获取当前收件人或组下的所有人
            foreach (JToken jt in senderIds)
            {
                // 判断 type
                string type = jt.Value<string>(Fields.type_);
                string id = jt.Value<string>(Fields._id);
                if (type == Fields.group)
                {
                    // 找到group下所有的用户
                    var boxes = SqlDb.Fetch<SendBox>(r => r.groupId == id).ToList();

                    // 如果没有，才添加
                    foreach (var box in boxes)
                    {
                        if (sendBoxes.Find(item => item._id == box._id) == null) sendBoxes.Add(box);
                    }
                }
                else
                {
                    // 选择了单个用户
                    var box = SqlDb.SingleOrDefault<SendBox>(r => r._id == id);
                    if (box != null && sendBoxes.Find(item => item._id == box._id) == null) sendBoxes.Add(box);
                }
            }

            return sendBoxes;
        }
    }
}
