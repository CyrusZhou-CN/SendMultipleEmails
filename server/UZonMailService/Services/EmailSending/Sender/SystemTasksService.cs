using System.Timers;
using Uamazing.Utils.Web.Service;
using UZonMailService.Services.EmailSending.WaitList;
using Timer = System.Timers.Timer;

namespace UZonMailService.Services.EmailSending.Sender
{
    /// <summary>
    /// 系统级发件调度中心
    /// </summary>
    public class SystemTasksService : ISingletonService
    {
        private CancellationTokenSource? _tokenSource = new();
        /// <summary>
        /// 发件任务
        /// </summary>
        private readonly List<EmailSendingTask> _sendingTasks = [];

        #region 外部调用的方法
        private ISendingWaitList _waitList;

        /// <summary>
        /// 外部调用该方法开始发件
        /// 为了简化逻辑，每次调用会打开所有的线程
        /// </summary>
        public void StartSending(int activeCount = 0)
        {
            // 获取需要的发件数，根据发件数，智能增加任务
            int emailTypesCount = _waitList.GetOutboxesCount();
            // 获取核心数
            int coreCount = Environment.ProcessorCount;

            int tasksCount = Math.Min(emailTypesCount, coreCount);
            if (tasksCount == 0)
            {
                // 没有发件箱时，清理任务
                ClearTasks();
                return;
            }

            // 所有线程共用取消信号
            _tokenSource ??= new CancellationTokenSource();

            // 有可能任务已经存在，则只需要增量新建
            // 创建任务
            for (int i = _sendingTasks.Count; i < tasksCount; i++)
            {
                // 单个线程的信号
                var autoResetEvent = new AutoResetEventWrapper(false);
                EmailSendingTask task = new(async () =>
                {
                    // 任务开始
                    await DoWork(_tokenSource, autoResetEvent);
                }, _tokenSource)
                {
                    AutoResetEventWrapper = autoResetEvent
                };

                _sendingTasks.Add(task);
                task.Start();
            }

            if (activeCount <= 0)
            {
                // 全部激活
                // 激活特定数量的线程,使其工作
                for (int i = 0; i < tasksCount; i++)
                {
                    _sendingTasks[i].AutoResetEventWrapper.Set();
                }
            }
            else
            {
                // 只激活指定数量的线程
                for (int i = 0; i < tasksCount; i++)
                {
                    var task = _sendingTasks[i];
                    if (task.AutoResetEventWrapper.IsWaiting)
                    {
                        task.AutoResetEventWrapper.Reset();
                        activeCount--;

                        // 激活达到指定数量后，退出
                        if (activeCount == 0)
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清除现有的任务
        /// </summary>
        private void ClearTasks()
        {
            if (_sendingTasks.Count == 0)
            {
                return;
            }

            // 取消所有任务
            _tokenSource?.Cancel();
            _tokenSource = null;
            _sendingTasks.Clear();
        }

        private Timer? _timer;
        /// <summary>
        /// 线程动态扩容
        /// </summary>
        private void DynamicExendTasks()
        {
            // 每 1 分钟检查一次
            _timer = new Timer(60000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }
        private void Timer_Elapsed(object? sender, ElapsedEventArgs? e)
        {
            // 每隔 1 分钟激活一次线程，防止线程一直处于等待状态
            StartSending();
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        private async Task DoWork(CancellationTokenSource tokenSource, AutoResetEventWrapper autoResetEvent)
        {
            // 当线程没有取消时
            while (!tokenSource.IsCancellationRequested)
            {
                // 激活后，从队列中取出任务
                var sendItem = _waitList.GetSendItem();
                if (sendItem == null)
                {
                    // 没有任务，继续等待
                    autoResetEvent.Reset();
                    continue;
                }

                // 发送邮件
                var sendMethod = SendMethodFactory.BuildSendMethod(sendItem);
                var status = await sendMethod.Send();
                if (status == SentStatus.Retry)
                {
                    // 发送失败，重新加入队列，可能会分配到其它线程去执行
                    sendItem.Enqueue();
                    continue;
                }
            }
        }
        #endregion
    }
}
