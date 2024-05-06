namespace UZonMailService.SignalRHubs.SendEmail
{
    public class SendingGroupProgressArg : SendingGroupTotalProgressArg
    {
        /// <summary>
        /// 开始时间
        /// 方便计算耗时
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 发件组 id
        /// </summary>
        public int SendingGroupId { get; set; }
    }
}
