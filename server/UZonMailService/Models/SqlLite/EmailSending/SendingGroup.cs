using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using Uamazing.ConfValidatation.Core.Entrance;
using Uamazing.ConfValidatation.Core.Validators;
using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.EntityConfigs.Attributes;
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
    public class SendingGroup : SqlId
    {
        #region EF 定义
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
        public string? Body { get; set; }

        /// <summary>
        /// 发件箱
        /// </summary>
        public List<Outbox> Outboxes { get; set; }

        /// <summary>
        /// 收件箱
        /// </summary>
        [JsonMap]
        public List<EmailAddress> Inboxes { get; set; }

        /// <summary>
        /// 抄送箱
        /// </summary>
        [JsonMap]
        public List<EmailAddress>? CcBoxes { get; set; }

        /// <summary>
        /// 密送
        /// </summary>
        [JsonMap]
        public List<EmailAddress>? BccBoxes { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        [JsonMap]
        public List<FileUsage>? Attachments { get; set; }

        /// <summary>
        /// 用户通过 excel 上传的数据
        /// </summary>
        [JsonMap]
        public JArray Data { get; set; }

        /// <summary>
        /// 是否分布式发件
        /// </summary>
        public bool IsDistributed { get; set; }

        /// <summary>
        /// 总发件数量
        /// Inboxes 的数量
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

        #region 定时发件相关
        /// <summary>
        /// 发件类型
        /// </summary>
        public SendingGroupType SendingType { get; set; }

        /// <summary>
        /// 定时发件时间
        /// </summary>
        public DateTime? ScheduleDate { get; set; }
        #endregion
        #endregion

        #region 外部工具方法       
        private List<string>? _subjects;
        private static readonly string[] separator = ["\r\n", "\n", ";", "；"];
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

        /// <summary>
        /// 验证数据
        /// </summary>
        /// <param name="validateOption"></param>
        /// <returns></returns>
        public bool Validate(ValidateOption validateOption = ValidateOption.None)
        {
            return this.Validate(new VdObj()
            {
                { ()=>this.Subjects,new IsString("主题不能为空"){ Required = true} },

                new Or(){
                    { ()=>this.Templates,x=>x!=null && x.Count>0,"模板为空"},
                    { ()=>this.Body,VdNames.IsString,"body 应是非空字符串"}
                },

                { ()=>this.Outboxes,x=>x!=null && x.Count>0,"发件箱为空"},
                { ()=>this.Inboxes,x=>x!=null && x.Count>0,"收件箱为空"},
            }, validateOption);
        }

        #endregion

        #region 生成发件项逻辑
        /// <summary>
        /// 生成发送项
        /// 该接口未进行数据验证，调用前请确保数据已经验证
        /// </summary>
        /// <returns></returns>
        public List<SendingItem> GenerateSendingItems()
        {
            var builder = new SendingItemsBuilder(this);
            return builder.Build();
        }
        #endregion
    }
}
