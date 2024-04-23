using System.Timers;
using UZonMailService.Services.EmailSending.WaitList;
using Timer = System.Timers.Timer;

namespace UZonMailService.Services.EmailSending.SendCore
{
    /// <summary>
    /// 系统级发件调度中心
    /// </summary>
    public class SystemTasksManager
    {
        #region 单例
        // 保证线程安全
        private static readonly Lazy<SystemTasksManager> _instance = new(() => new SystemTasksManager());
        /// <summary>
        /// 任务管理中心的单例
        /// </summary>
        public static SystemTasksManager Instance => _instance.Value;
        #endregion

        private CancellationTokenSource? _tokenSource = new CancellationTokenSource();
        /// <summary>
        /// 发件任务
        /// </summary>
        private readonly List<EmailSendingTask> _sendingTasks = [];

        private SystemTasksManager()
        {
            DynamicExendTasks();
        }

        #region 外部调用的方法
        private IWaitList _waitList;

        /// <summary>
        /// 外部调用该方法开始发件
        /// </summary>
        public void StartSending()
        {
            // 获取需要的发件数，根据发件数，智能创建任务
            int emailTypesCount = _waitList.GetEmailTypesCount();
            // 获取核心数
            int coreCount = Environment.ProcessorCount;

            int taskCount = Math.Min(emailTypesCount, coreCount);
            if (taskCount == 0)
            {
                // 没有发件时，清理任务
                ClearTasks();
                return;
            }

            // 所有线程共用取消信号
            _tokenSource ??= new CancellationTokenSource();

            // 有可能任务已经存在，则只需要增量新建
            // 创建任务
            for (int i = _sendingTasks.Count; i < taskCount; i++)
            {
                // 单个线程的信号
                var autoResetEvent = new AutoResetEvent(false);
                EmailSendingTask task = new(async () =>
                {
                    // 任务开始
                    await DoWork(_tokenSource, autoResetEvent);
                }, _tokenSource)
                {
                    AutoResetEvent = autoResetEvent
                };

                _sendingTasks.Add(task);
                task.Start();
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
            StartSending();
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        private async Task DoWork(CancellationTokenSource tokenSource, AutoResetEvent autoResetEvent)
        {
            // 当线程没有取消时
            while (!tokenSource.IsCancellationRequested)
            {
                // 等待被激活                
                autoResetEvent.WaitOne();

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
                bool isSuccess = await sendMethod.Send();
                if (!isSuccess)
                {
                    // 发送失败，重新加入队列，可能会分配到其它线程去执行
                    _waitList.QueueSendItem(sendItem);
                    continue;
                }
            }
        }
        #endregion
    }
}
