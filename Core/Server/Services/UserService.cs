using LiteDB;
using Uamazing.SME.Server.Database;
using Uamazing.SME.Server.Models;
using Uamazing.SME.Server.Utils.LiteDB;

namespace Uamazing.SME.Server.Services
{
    /// <summary>
    /// 用户相关的操作
    /// </summary>
    public class UserService : ServiceBase
    {
        public UserService(LiteRepository liteRepository) : base(liteRepository) { }

        /// <summary>
        /// 新建用户
        /// </summary>
        /// <returns></returns>
        public User NewUser(User user)
        {
            return null;
        }

        /// <summary>
        /// 将 connectionId 保存到用户表中
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connectionId"></param>
        public void SetSignalRConnectionId(string userId, string connectionId)
        {
            LiteRepository.Update2(x => x.UserId == userId,
                new User() { ConnectionId = connectionId },
                new UpdateOptions() { FieldMap.connectionId });
        }

        /// <summary>
        /// 将用户的 connectionId 取消
        /// </summary>
        /// <param name="userId"></param>
        public void ClearSignalRConnectionId(string userId)
        {
            LiteRepository.Update2(x => x.UserId == userId, 
                new User() { ConnectionId = string.Empty }, 
                new UpdateOptions { FieldMap.connectionId });
        }
    }
}
