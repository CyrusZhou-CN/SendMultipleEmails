using UZonMailService.Models.SqlLite.EntityConfigs.Attributes;

namespace UZonMailService.Models.SqlLite.Settings
{
    /// <summary>
    /// 系统代理
    /// </summary>
    public class SystemProxy : UserProxy
    {
        /// <summary>
        /// 若是系统代理，可设置忽略的用户
        /// </summary>
        [JsonMap]
        public List<int>? IgnoreUserIds { get; set; }

        /// <summary>
        /// 若是系统代理，通过这个设置匹配的用户id
        /// 只有匹配到的用户才会使用代理
        /// </summary>
        [JsonMap]
        public List<int>? MachedUserIds { get; set; }

        /// <summary>
        /// 是否共享
        /// 共享的代理，所有用户都可以使用
        /// </summary>
        public bool IsShared { get; set; }
    }
}
