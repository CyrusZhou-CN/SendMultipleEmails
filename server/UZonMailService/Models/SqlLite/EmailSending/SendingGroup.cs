using Newtonsoft.Json.Linq;
using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.Files;
using UZonMailService.Models.SqlLite.Templates;
using UZonMailService.Models.SqlLite.UserInfos;

namespace UZonMailService.Models.SqlLite.EmailSending
{
    /// <summary>
    /// 发件组
    /// 此处只记录统计数据
    /// 具体的数据由 EmailItem 记录
    /// </summary>
    public class SendingGroup:SqlId
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 主题
        /// 多个主题使用分号或者换行分隔
        /// </summary>
        public string Subjects { get; set; }

        /// <summary>
        /// 模板
        /// </summary>
        public List<EmailTemplate> Templates { get; set; }

        /// <summary>
        /// 正文内容
        /// </summary>
        public string EmailBody { get; set; }

        /// <summary>
        /// 发件箱
        /// </summary>
        public List<Outbox> Outboxes { get; set; }

        /// <summary>
        /// 收件箱
        /// </summary>
        public List<string> Inboxes { get; set; }

        /// <summary>
        /// 抄送箱
        /// </summary>
        public List<string> CCBoxes { get; set;}

        /// <summary>
        /// 附件
        /// </summary>
        public List<FileUsage> Attachments { get; set; }

        /// <summary>
        /// 用户数据
        /// </summary>
        public List<JToken> Data { get; set; }

        /// <summary>
        /// 是否分布式发件
        /// </summary>
        public bool IsDistributed { get; set; }

        /// <summary>
        /// 总发件数量
        /// = 显示收件箱数量+数据中的收件箱数量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 成功的数量
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public SendingGroupStatus Status { get; set; }

        /// <summary>
        /// 发送开始时间
        /// </summary>
        public DateTime SendStartDate { get; set; }

        /// <summary>
        /// 发送结束时间
        /// </summary>
        public DateTime SendEndDate { get; set; }

        private List<string>? _subjects;
        private static readonly string[] separator = ["\r\n", "\n", ";","；"];

        /// <summary>
        /// 若有多个主题，则获取随机主题
        /// </summary>
        /// <returns></returns>
        public string GetSubject()
        {
            if (_subjects == null)
            {
                // 说明没有初始化
                if (string.IsNullOrEmpty(Subjects))
                {
                    _subjects = [string.Empty];
                    return string.Empty;
                }

                // 分割主题
                _subjects = [.. Subjects.Split(separator, StringSplitOptions.RemoveEmptyEntries)];
            }

            // 返回随机主题
            return _subjects[new Random().Next(_subjects.Count)];
        }
    }
}
