namespace UZonMailService.SignalRHubs.SendEmail
{
    public class EmailTotalProgress
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 当前数量
        /// </summary>
        public int Current { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }
}
