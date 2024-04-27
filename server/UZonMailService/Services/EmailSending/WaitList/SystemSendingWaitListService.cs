﻿using System.Collections.Concurrent;
using Uamazing.Utils.Web.Service;
using UZonMailService.Services.EmailSending.Sender;

namespace UZonMailService.Services.EmailSending.WaitList
{
    /// <summary>
    /// 系统级的待发件调度器
    /// 请求时，平均向各个用户请求资源
    /// 每位用户的资源都是公平的
    /// 今后可以考虑加入权重
    /// </summary>
    public class SystemSendingWaitListService : ISendingWaitList,ISingletonService
    {
        private readonly ConcurrentQueue<UserSendingTaskManager> _userTasks = new();

        /// <summary>
        /// 邮件类型总数
        /// </summary>
        /// <returns></returns>
        public int GetOutboxesCount()
        {
            return _userTasks.Select(x => x.GetOutboxesCount()).Sum();
        }

        /// <summary>
        /// 发送模块调用该方法获取发件项
        /// 若返回空，会导致发送任务暂停
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public SendItem? GetSendItem()
        {
            if (_userTasks.IsEmpty)
                return null;

            // 依次获取发件项
            // 返回 null 有以下几种情况：
            // 1. manager 为空
            // 2. 所有发件箱都在冷却中
            int queueCount = _userTasks.Count;
            int index = 0;
            SendItem? sendItem = null;
            while (index < queueCount && sendItem == null)
            {
                index++;

                if (!_userTasks.TryDequeue(out var manager)) continue;

                sendItem = manager.GetSendItem();
                // 若为空，判断是否需要释放
                if (sendItem == null)
                {
                    var status = manager.GetManagerStatus();
                    if (status >= SendingManagerStatus.ShouldDispose)
                    {
                        manager.ChangeStatus(SendingManagerStatus.Disposed);
                        // 不重新入队了
                        continue;
                    }
                }

                // 重新入队
                _userTasks.Enqueue(manager);
            }

            return sendItem;
        }
    }
}
