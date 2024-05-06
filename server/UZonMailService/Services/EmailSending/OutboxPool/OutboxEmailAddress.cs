using System.Timers;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.EmailSending;
using Timer = System.Timers.Timer;

namespace UZonMailService.Services.EmailSending.OutboxPool
{
    /// <summary>
    /// 发件箱地址
    /// </summary>
    public class OutboxEmailAddress : EmailAddress
    {
        #region 所属分类
        /// <summary>
        /// 所属的发件箱组 id
        /// </summary>
        public HashSet<int> SendingGroupIds = [];
        #endregion

        #region 构造
        private long _cooldownMilliseconds = 0;
        private int _maxPerDay = 0;

        /// <summary>
        /// 初始化发件地址
        /// </summary>
        /// <param name="cooldownMilliseconds">时间间距,单位 ms, 为 0 时表示不限制</param>
        /// <param name="maxPerDay">每天最大发件量，超过这个值，不能发件, 为 0 时表示不限制</param>
        public OutboxEmailAddress(long cooldownMilliseconds, int maxPerDay)
        {
            _cooldownMilliseconds = cooldownMilliseconds;
            _maxPerDay = maxPerDay;
        }
        #endregion

        #region 设置
        private string _authUserName;
        /// <summary>
        /// 授权用户名
        /// </summary>
        public string AuthUserName
        {
            get { return string.IsNullOrEmpty(_authUserName) ? Email : _authUserName; }
            set { _authUserName = value; }
        }

        /// <summary>
        /// 授权密码
        /// </summary>
        public string AuthPassword { get; set; }

        public string SmtpHost { get; set; }

        public int SmtpPort { get; set; }

        public bool EnableSSL { get; set; }
        #endregion

        #region 状态
        private bool _isCooldown = false;

        /// <summary>
        /// 设置冷却
        /// </summary>
        /// <returns></returns>
        public void SetCooldown()
        {
            if (_maxPerDay == 0) return;
            _isCooldown = true;

            // 启动 _timer 用于解除冷却
            var timer = new Timer(_cooldownMilliseconds)
            {
                AutoReset = false,
                Enabled = true
            };
            timer.Elapsed += (sender, args) =>
            {
                timer.Stop();
                _isCooldown = false;

                // 通知可以继续发件
                EmailSendingService.Instance.TasksService.StartSending(1);
            };
        }
        /// <summary>
        /// 是否不可用
        /// </summary>
        public bool Disable => _isCooldown;
        #endregion
    }
}
