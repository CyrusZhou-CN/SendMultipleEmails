using System.Collections.Concurrent;
using UZonMailService.Models.SqlLite.EmailSending;

namespace UZonMailService.Services.EmailSending
{
    /// <summary>
    /// 邮件队列
    /// </summary>
    public class EmailsQueue : ConcurrentQueue<SendingItem>
    {
        private DateTime _lastSendDate = DateTime.MinValue;

        /// <summary>
        /// 默认同类型的邮件发送间隔为 1 s
        /// </summary>
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// 是否可以出队
        /// </summary>
        /// <returns></returns>
        public bool CanDequeue()
        {
            if (this.IsEmpty) return false;
            return DateTime.Now - _lastSendDate > Interval;
        }

        /// <summary>
        /// 出队列
        /// </summary>
        /// <returns></returns>
        public SendingItem? Dequeue()
        {
            // 更新最后发送时间
            _lastSendDate = DateTime.Now;

            if (this.TryDequeue(out var item))
            {
                return item;
            }
            return null;
        }
    }
}
