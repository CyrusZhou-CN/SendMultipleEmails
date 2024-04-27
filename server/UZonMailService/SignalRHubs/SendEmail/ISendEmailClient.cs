namespace UZonMailService.SignalRHubs.SendEmail
{
    public interface ISendEmailClient
    {
        /// <summary>
        /// 当前用户所有的邮件发送进度
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task EmailSendingTotalProgressChanged(EmailTotalProgress progress);

        /// <summary>
        /// 单个邮件组发送进度
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task EmailGroupSendingProgressChanged(EmailGroupSendingProgress progress);

        /// <summary>
        /// 单个邮件组发送进度
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task EmailStatusChanged(EmailGroupSendingProgress progress);

        /// <summary>
        /// 邮件发送错误
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task EmailSendingError(string message);
    }
}
