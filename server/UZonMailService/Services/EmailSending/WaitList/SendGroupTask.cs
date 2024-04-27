using System.Collections.Concurrent;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.Sender;

namespace UZonMailService.Services.EmailSending.WaitList
{
    /// <summary>
    /// 一次发件任务
    /// 包含发件组和收件内容
    /// </summary>
    public class SendGroupTask
    {
        #region 初始化
        private readonly SqlContext _db;
        private readonly SendingGroup _sendingGroup;

        /// <summary>
        /// 总发件
        /// </summary>
        private int _itemsTotal = 0;
        /// <summary>
        /// 已发送的数量：成功或失败都计入
        /// </summary>
        private int _sentCount = 0;
        private int _success = 0;

        public SendGroupTask(SqlContext db, SendingGroup sendingGroup, IList<SendItem> sendItems)
        {
            _db = db;
            _sendingGroup = sendingGroup;

            _itemsTotal = sendItems.Count;
            _sentCount = 0;
            _success = 0;

            foreach (var sendItem in sendItems)
            {
                _sendItems.Enqueue(sendItem);
            }
        }
        #endregion

        /// <summary>
        /// 发件列表
        /// </summary>
        private Queue<SendItem> _sendItems = new();

        /// <summary>
        /// 获取发件项
        /// </summary>
        /// <returns></returns>
        public SendItem? GetSendItem()
        {
            // 获取发件箱
            OutboxEmailAddress? outboxEmailAddress = EmailSendingService.Instance.OutboxPool.GetOutbox(_sendingGroup.UserId, _sendingGroup.Id);
            if(outboxEmailAddress == null) return null;

            if (!_sendItems.TryDequeue(out var sendItem)) return null;

            // 为 sendItem 动态赋值
            sendItem.Outbox = outboxEmailAddress;
            sendItem.SetSendGroupTask(this);
            // 计算邮件标题
            sendItem.Subject = _sendingGroup.GetSubject();
            return sendItem;
        }

        /// <summary>
        /// 外部调用，添加发件项
        /// </summary>
        /// <param name="sendItem"></param>
        /// <returns></returns>
        public bool Enqueue(SendItem? sendItem)
        {
            if (sendItem == null) return false;
            _sendItems.Enqueue(sendItem);
            return true;
        }

        /// <summary>
        /// 邮件发送完成
        /// 只储存 邮件组 数据，具体的每次发件数据在 SendItem 中处理
        /// </summary>
        /// <param name="success">是否发送成功</param>
        public async Task EmailSendComplete(bool success)
        {
            _sentCount++;
            if (success) _success++;

            // 向数据库中保存状态
            var sendingGroup = new SendingGroup
            {
                Id = _sendingGroup.Id
            };
            _db.Attach(sendingGroup);
            sendingGroup.TotalCount = _itemsTotal;
            sendingGroup.SuccessCount = _success;
            await _db.SaveChangesAsync();

            // 向用户推送进度
            var hub = EmailSendingService.Instance.HubContext;
        }
    }
}
