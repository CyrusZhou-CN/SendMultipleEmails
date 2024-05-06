namespace UZonMailService.SignalRHubs.SendEmail
{
    public interface ISendEmailClient
    {
        /// <summary>
        /// 当前用户所有的邮件发送进度
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task SendingGroupTotalProgressChanged(SendingGroupTotalProgressArg progress);

        /// <summary>
        /// 单个邮件组发送进度
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task SendingGroupProgressChanged(SendingGroupProgressArg progress);

        /// <summary>
        /// 单个邮件组发送进度
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task SendingItemProgressChanged(SendingGroupProgressArg progress);

        /// <summary>
        /// 邮件发送错误
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendError(string message);
    }
}
