namespace UZonMailService.Models.SqlLite.EmailSending
{
    /// <summary>
    /// 邮件状态
    /// </summary>
    public enum SendingItemStatus
    {
        /// <summary>
        /// 初始状态
        /// </summary>
        Created,

        /// <summary>
        /// 发送状态
        /// </summary>
        Sending,

        /// <summary>
        /// 发送成功
        /// </summary>
        Success,

        /// <summary>
        /// 发送失败
        /// </summary>
        Failed,
    }
}
