using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Services.EmailSending.Models;

namespace UZonMailService.Services.EmailSending
{
    /// <summary>
    /// 用户单个任务管理
    /// </summary>
    public class UserTaskManager : Dictionary<int, EmailsQueue>
    {
        /// <summary>
        /// 队列数量
        /// </summary>
        private int _queuesCount = 0;
        private List<int> _dicKeys = [];
        private List<OutboxInfo> _outboxes = [];

        /// <summary>
        /// 暂停
        /// 暂停超时后，要将数据清理掉
        /// </summary>
        public void Pause()
        {

        }

        /// <summary>
        /// 发送失败后，需要重新入队
        /// </summary>
        /// <param name="item"></param>
        public void EnqueueSendingItem(SendingItem item)
        {
        }
    }
}
