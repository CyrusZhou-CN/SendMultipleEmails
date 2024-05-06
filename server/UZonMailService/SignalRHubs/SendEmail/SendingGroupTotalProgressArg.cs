namespace UZonMailService.SignalRHubs.SendEmail
{
    public class SendingGroupTotalProgressArg
    {
        /// <summary>
        /// 总数
        /// </summary>
        public double Total { get; set; }

        /// <summary>
        /// 当前数量
        /// </summary>
        public double Current { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }
}
