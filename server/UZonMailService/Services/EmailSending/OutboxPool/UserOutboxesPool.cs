using Uamazing.Utils.Web.Service;

namespace UZonMailService.Services.EmailSending.OutboxPool
{
    /// <summary>
    /// 用户的发件箱池
    /// 每个邮箱账号共用冷却池
    /// key: 用户 id，value: 发件箱列表
    /// </summary>
    public class UserOutboxesPool : Dictionary<int, List<OutboxEmailAddress>>, ISingletonService
    {
        /// <summary>
        /// 获取发件箱数量
        /// </summary>
        /// <returns></returns>
        public int GetOutboxesCount()
        {
            return this.Sum(o => o.Value.Count);
        }

        /// <summary>
        /// 获取发件箱组
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="sendingGroupId">发件组id</param>
        /// <returns></returns>
        public OutboxEmailAddress? GetOutbox(int userId, int sendingGroupId)
        {
            var result = this[userId].FirstOrDefault(o => !o.Disable && o.SendingGroupIds.Contains(sendingGroupId));
            // 取出来后，进入冷却时间
            // 避免被其它线程取出
            result?.SetCooldown();
            return result;
        }
    }
}
