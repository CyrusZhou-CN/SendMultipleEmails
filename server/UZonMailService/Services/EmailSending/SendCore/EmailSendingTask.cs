
namespace UZonMailService.Services.EmailSending.SendCore
{
    /// <summary>
    /// 自定义的 Task
    /// </summary>
    public class EmailSendingTask : Task
    {
        /// <summary>
        /// 取消标记
        /// </summary>
        public readonly CancellationTokenSource CancelTokenSource;

        /// <summary>
        /// 线程信号
        /// </summary>
        public AutoResetEvent AutoResetEvent { get; set; }

        public EmailSendingTask(Action action, CancellationTokenSource tokenSource) : base(action, tokenSource.Token)
        {
            CancelTokenSource = tokenSource;
        }
    }
}
