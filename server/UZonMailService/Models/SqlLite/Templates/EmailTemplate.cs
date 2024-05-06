using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.EmailSending;

namespace UZonMailService.Models.SqlLite.Templates
{
    /// <summary>
    /// 邮箱模板
    /// </summary>
    public class EmailTemplate : SqlId
    {
        /// <summary>
        /// 用户 id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        public string? Thumbnail { get; set; }

        /// <summary>
        /// 使用的组
        /// </summary>
        public List<SendingGroup>? SendingGroups { get; set; }
    }
}
