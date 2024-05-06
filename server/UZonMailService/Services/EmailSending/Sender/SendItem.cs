using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.WaitList;
using Timer = System.Timers.Timer;

namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 发件项
    /// </summary>
    public class SendItem
    {
        private static SqlContext Db => EmailSendingService.Instance.Db;
        private int _sendItemId;

        public SendItem(int sendItemId)
        {
            _sendItemId = sendItemId;
        }

        /// <summary>
        /// 发送类型
        /// </summary>
        public SendItemType SendItemType { get; set; }

        public OutboxEmailAddress Outbox { get; set; }

        public List<EmailAddress> Inboxes { get; set; }

        /// <summary>
        /// 抄送人
        /// </summary>
        public List<EmailAddress> CC { get; set; }

        /// <summary>
        /// 密送人
        /// </summary>
        public List<EmailAddress> BCC { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { private get; set; }

        /// <summary>
        /// HTML 内容
        /// </summary>
        public string HtmlBody { private get; set; }

        /// <summary>
        /// 正文变量数据
        /// </summary>
        public SendingItemExcelData? BodyData { get; set; }

        /// <summary>
        /// 附件 FileObjectId 列表
        /// </summary>
        public List<int> AttachmentIds { get; set; }

        /// <summary>
        /// 批量发送
        /// </summary>
        public bool IsSendingBatch { get; set; }

        /// <summary>
        /// 验证数据是否满足要求
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            if (string.IsNullOrEmpty(Outbox.Email) || Inboxes == null || Inboxes.Count == 0
                || string.IsNullOrEmpty(Outbox.SmtpHost) || Outbox.SmtpPort == 0
                || string.IsNullOrEmpty(HtmlBody))
            {
                return false;
            }
            return true;
        }

        private List<string> _attachments;
        /// <summary>
        /// 获取附件
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAttachments()
        {
            if (_attachments != null)
            {
                return _attachments;
            }

            // 查找文件
            _attachments = await Db.FileObjects.Where(f => AttachmentIds.Contains(f.Id))
                .Include(x => x.FileBucket)
                .Select(x => $"{x.FileBucket.RootDir}/{x.Path}")
                .ToListAsync();
            return _attachments;
        }

        private string _body;
        /// <summary>
        /// 获取经过变量替换后的正文
        /// </summary>
        /// <returns></returns>
        public string GetBody()
        {
            if (!string.IsNullOrEmpty(_body)) return _body;
            // 替换正文变量
            _body = ComputedVariables(HtmlBody);
            return _body;
        }

        private string ComputedVariables(string originText)
        {
            if (!string.IsNullOrEmpty(originText)) return originText;
            // 替换正文变量
            if (BodyData == null) return originText;

            foreach (var item in BodyData)
            {
                if (item.Value == null) continue;
                // 使用正则进行替换
                var regex = new Regex(@"\{\{\s*" + item.Key + @"\s*\}\}", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                originText = regex.Replace(originText, item.Value.ToString());
            }
            return originText;
        }

        /// <summary>
        /// 获取主题
        /// </summary>
        /// <returns></returns>
        public string GetSubject()
        {
            // 主题中可能有变量
            return ComputedVariables(Subject);
        }

        #region 重发逻辑，该部分仅在主服务器上使用，后期考虑抽象出来
        // 重试次数
        private int _triedCount = 0;
        /// <summary>
        /// 最大重试次数
        /// </summary>
        public int RetryMax = 0;

        /// <summary>
        /// 保存发送状态
        /// 返回状态指示是否需要重试
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<SentStatus> UpdateSendingStatus(bool success, string message)
        {
            // 判断是否需要重试
            if (!success && _triedCount < RetryMax)
            {
                _triedCount++;
                return SentStatus.Retry;
            }

            // 保存到数据库
            await SaveToDb();

            // 调用回调,通知上层处理结果
            await _sendGroupTask.EmailSendComplete(success);

            return success ? SentStatus.OK : SentStatus.Failed;
        }

        /// <summary>
        /// 保存到数据库
        /// </summary>
        /// <returns></returns>
        private async Task SaveToDb()
        {
            // 保存到数据库
            var data = new SendingItem()
            {
                Id = _sendItemId
            };
            Db.Attach(data);

            // 更新数据
            data.FromEmail = Outbox.Email;
            data.Subject = Subject;
            data.Content = HtmlBody;
            data.Inboxes = Inboxes;
            data.CC = CC;
            data.BCC = BCC;

            await Db.SaveChangesAsync();
        }

        private SendGroupTask _sendGroupTask;
        public void SetSendGroupTask(SendGroupTask sendGroupTask)
        {
            _sendGroupTask = sendGroupTask;
        }

        public int _cooldownMilliseconds = 0;
        /// <summary>
        /// 重新入队
        /// </summary>
        public void Enqueue()
        {
            if (_sendGroupTask == null) return;

            if (_cooldownMilliseconds > 0)
            {
                // 设置冷却
                var _timer = new Timer(_cooldownMilliseconds)
                {
                    AutoReset = false,
                    Enabled = true
                };
                _timer.Elapsed += (object? sender, ElapsedEventArgs e) =>
                {
                    _timer.Stop();
                    // 重新入队
                    _sendGroupTask.Enqueue(this);
                    // 通知继续发件
                    EmailSendingService.Instance.TasksService.StartSending(1);
                };
            }
            else
            {
                _sendGroupTask.Enqueue(this);
            }
        }
        #endregion

        #region 发送状态变更
        /// <summary>
        /// 设置状态
        /// </summary>
        public void SetStatus()
        {
            // 设置状态为正在发送

        }
        #endregion
    }
}
