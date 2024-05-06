using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Linq;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Models.SqlLite.Templates;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.Sender;
using UZonMailService.Services.Settings;
using Uamazing.Utils.Json;
using Newtonsoft.Json.Linq;
using UZonMailService.SignalRHubs.SendEmail;

namespace UZonMailService.Services.EmailSending.WaitList
{
    /// <summary>
    /// 一次发件任务
    /// 包含发件组和收件内容
    /// </summary>
    /// <remarks>
    /// 构造
    /// </remarks>
    /// <param name="sendingGroup"></param>
    public class SendGroupTask(SendingGroup sendingGroup)
    {
        /// <summary>
        /// 组 id
        /// </summary>
        public int GroupId => _sendingGroup.Id;

        #region 初始化
        private static SqlContext Db => EmailSendingService.Instance.Db;
        private readonly SendingGroup _sendingGroup = sendingGroup;

        /// <summary>
        /// 总发件
        /// </summary>
        private int _itemsTotal = 0;
        /// <summary>
        /// 已发送的数量：成功或失败都计入
        /// </summary>
        private int _sentCount = 0;
        private int _success = 0;
        private DateTime _startDate = DateTime.Now;

        /// <summary>
        /// 开始初始化
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Init()
        {
            // 从数据库获取发件项
            var sendingItems = await Db.SendingItems.Where(x => x.SendingGroupId == _sendingGroup.Id
            && x.Status != SendingItemStatus.Success).ToListAsync();
            if (sendingItems.Count == 0) return false;

            // 将新的发件箱添加到发件池中
            var outBoxIds = _sendingGroup.Outboxes.Select(x => x.Id).ToList();
            var existOutboxes = EmailSendingService.Instance.OutboxPool.GetExistOutboxes(_sendingGroup.UserId);
            var newBoxIds = outBoxIds.Except(existOutboxes.Select(x => x.Id)).ToList();
            if (newBoxIds.Count > 0)
            {
                var setting = await UserSettingsWrapper.CreateAsync(Db, _sendingGroup.UserId);
                var outboxes = await Db.Outboxes.Where(x => outBoxIds.Contains(x.Id)).ToListAsync();
                foreach (var outbox in outboxes)
                {
                    var newOutboxAddress = outbox.ToOutboxEmailAddress(setting.OutboxCooldownMs,
                        setting.GetMaxSendCountPerEmailDay(outbox.MaxSendCountPerDay), GroupId);
                    EmailSendingService.Instance.OutboxPool.AddOutbox(_sendingGroup.UserId, newOutboxAddress);
                }
            }
            // 对于已经存在的发件箱，更新发送组 id
            var reuseOutboxes = existOutboxes.Where(x => outBoxIds.Contains(x.Id)).ToList();
            foreach (var outbox in reuseOutboxes)
            {
                outbox.SendingGroupIds.Add(_sendingGroup.Id);
            }

            // 生成发件列表
            _itemsTotal = sendingItems.Count;
            var templates = await PullEmailTemplates();

            foreach (var item in sendingItems)
            {
                // 将 sendingItem 转换成 sendItem
                var sendItem = item.ToSendItem();
                // 添加数据
                sendItem.BodyData = GetExcelData(item);
                // 生成正文
                sendItem.HtmlBody = GetSendingItemOriginBody(sendItem, templates);
                Enqueue(sendItem);
            }

            // 通知用户，任务已开始
            var client = EmailSendingService.GetSignalRClient(_sendingGroup.UserId);
            if (client != null) {
                await client.SendingGroupProgressChanged(new SendingGroupProgressArg()
                    {
                        Current = _sentCount,
                        Total = _itemsTotal,
                        StartDate = _startDate,
                    });
            }

            return true;
        }

        /// <summary>
        /// 获取组中所有的模板
        /// </summary>
        /// <returns></returns>
        private async Task<List<EmailTemplate>> PullEmailTemplates()
        {
            var templateIds = _sendingGroup.Templates.Select(x => x.Id).ToHashSet();
            // 从数据中获取模板
            if (_sendingGroup.Data != null && _sendingGroup.Data.Count > 0)
            {
                // 有数据
                var templateIdsTemp = _sendingGroup.Data.Select(x => x.SelectTokenOrDefault("templateId", 0)).Where(x => x > 0);
                foreach (var item in templateIdsTemp)
                {
                    templateIds.Add(item);
                }
            }
            if (templateIds.Count == 0) return [];

            return await Db.EmailTemplates
                .Where(x => templateIds.Contains(x.Id) && x.UserId == _sendingGroup.UserId)
                .ToListAsync();
        }

        /// <summary>
        /// 获取 excel 数据
        /// </summary>
        /// <param name="sendingItem"></param>
        /// <returns></returns>
        private SendingItemExcelData? GetExcelData(SendingItem sendingItem)
        {
            if (sendingItem.IsSendingBatch) return null;
            if (_sendingGroup.Data == null || _sendingGroup.Data.Count == 0) return null;
            // 查找
            var data = _sendingGroup.Data.FirstOrDefault(x => x.SelectTokenOrDefault("inbox", string.Empty) == sendingItem.Inboxes[0].Email);
            if (data == null) return null;
            return new SendingItemExcelData(data as JObject);
        }

        /// <summary>
        /// 获取原始发件内容
        /// 变量未经过处理
        /// </summary>
        /// <param name="sendItem"></param>
        /// <param name="templates"></param>
        /// <returns></returns>
        private string GetSendingItemOriginBody(SendItem sendItem, List<EmailTemplate> templates)
        {
            if (sendItem.IsSendingBatch)
            {
                // 说明是批量发送
                if (!string.IsNullOrEmpty(_sendingGroup.Body)) return _sendingGroup.Body;
                // 否则返回第一个模板
                return templates.FirstOrDefault()?.Content ?? string.Empty;
            }

            // 非批量发送
            // 当有用户数据时
            if (sendItem.BodyData != null)
            {
                // 判断是否有 body
                if (!string.IsNullOrEmpty(sendItem.BodyData.Body)) return sendItem.BodyData.Body;

                // 判断是否有模板 id 或者模板名称
                if (sendItem.BodyData.TemplateId > 0)
                {
                    var template = templates.FirstOrDefault(x => x.Id == sendItem.BodyData.TemplateId);
                    if (template != null) return template.Content;
                }

                // 判断是否有模板名称
                if (!string.IsNullOrEmpty(sendItem.BodyData.TemplateName))
                {
                    var template = templates.FirstOrDefault(x => x.Name == sendItem.BodyData.TemplateName);
                    if (template != null) return template.Content;
                }
            }

            // 没有数据时，优先使用组中的 body
            if (!string.IsNullOrEmpty(_sendingGroup.Body)) return _sendingGroup.Body;

            // 返回随机模板
            return templates[new Random().Next(templates.Count)].Content;

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
            if (outboxEmailAddress == null) return null;

            if (!_sendItems.TryDequeue(out var sendItem)) return null;

            // 为 sendItem 动态赋值
            sendItem.Outbox = outboxEmailAddress;
            sendItem.SetSendGroupTask(this);
            // 动态邮件标题
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
            Db.Attach(sendingGroup);
            sendingGroup.TotalCount = _itemsTotal;
            sendingGroup.SuccessCount = _success;
            await Db.SaveChangesAsync();

            // 向上传递回调

            // 向用户推送进度
            var hub = EmailSendingService.Instance.HubContext;
        }
    }
}
