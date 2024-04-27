using System.Collections.Concurrent;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Services.EmailSending.Models;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.Sender;

namespace UZonMailService.Services.EmailSending.WaitList
{
    /// <summary>
    /// 用户发件任务管理
    /// </summary>
    public class UserSendingTaskManager : List<SendGroupTask>, ISendingWaitList, IMSendingManagerStatus
    {
        // 完整的发件池
        // 每个发件组也有一个子发件池
        public int GetOutboxesCount()
        {
            return EmailSendingService.Instance.OutboxPool.Count;
        }

        /// <summary>
        /// 获取组中的发件项
        /// </summary>
        /// <returns></returns>
        public SendItem? GetSendItem()
        {
            if(EmailSendingService.Instance.OutboxPool.Count == 0)

                return null;
            if(this.Count==0)return null;

            // 依次获取发件项
            SendItem? sendItem = null;
            foreach (var groupTask in this)
            {
                sendItem = groupTask.GetSendItem();
                if (sendItem != null)
                {                  
                    break;
                }
            }

            return sendItem;
        }

        #region 用户任务管理器状态
        private SendingManagerStatus _status = SendingManagerStatus.Normal;
        /// <summary>
        /// 获取管理器状态
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public SendingManagerStatus GetManagerStatus()
        {
            if (_status >= SendingManagerStatus.ShouldDispose)
                return _status;

            // 动态计算
            // 若发件池为空，则释放资源
            if (EmailSendingService.Instance.OutboxPool.Count == 0)
                return SendingManagerStatus.ShouldDispose;

            return SendingManagerStatus.Normal;
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="status"></param>
        public void ChangeStatus(SendingManagerStatus status)
        {
            _status = status;
        }
        #endregion
    }
}
