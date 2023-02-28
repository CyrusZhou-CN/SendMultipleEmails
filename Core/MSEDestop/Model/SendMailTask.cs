namespace Uamazing.SME.Server.Model
{
    /// <summary>
    /// 发件任务
    /// </summary>
    public class SendMailTask:LinkingUserId
    {      
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 发送成功的数量
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount { get; set; }

        public override string GetFilterString()
        {
            return base.GetFilterString() + Name + Description;
        }
    }
}
