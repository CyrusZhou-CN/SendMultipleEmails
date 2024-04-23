namespace UZonMailService.Models.SqlLite.EmailSending
{
    public enum SendingTaskStatus
    {
        /// <summary>
        /// 新建
        /// </summary>
        Created,
        /// <summary>
        /// 计划发件
        /// </summary>
        Scheduled,
        /// <summary>
        /// 发送中
        /// </summary>
        Sending,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause,
        /// <summary>
        /// 停止
        /// </summary>
        Stop,
        /// <summary>
        /// 发送完成
        /// </summary>
        Complete,
    }
}
