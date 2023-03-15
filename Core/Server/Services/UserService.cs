using LiteDB;
using Uamazing.SME.Server.Database;
using Uamazing.SME.Server.Models;
using Uamazing.Utils.Extensions;
using Uamazing.Utils.LiteDB;

namespace Uamazing.SME.Server.Services
{
    /// <summary>
    /// 用户相关的操作
    /// </summary>
    public class UserService : ServiceBase
    {
        public UserService(ILiteRepository liteRepository) : base(liteRepository) { }

        public async Task<User> GetUser(string userId)
        {
            var result = LiteRepository.FirstOrDefault<User>(x => x.UserId == userId);
            return result;
        }

        public async Task<User> GetUser(string userId ,string password)
        {
            var result = LiteRepository.FirstOrDefault<User>(x => x.UserId == userId && x.Password==password);
            return result;
        }

        /// <summary>
        /// 新建用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<User> CreateUser(User user)
        {
            // 对密码进行加密
            user.Password = user.Password.EncryptMD5();
            LiteRepository.Insert(user);
            return user;
        }

        /// <summary>
        /// 将 connectionId 保存到用户表中
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connectionId"></param>
        public void SetSignalRConnectionId(string userId, string connectionId)
        {
            LiteRepository.UpdateOne(x => x.UserId == userId,
                new User() { ConnectionId = connectionId },
                new UpdateOptions() { FieldMap.connectionId });
        }

        /// <summary>
        /// 将用户的 connectionId 取消
        /// </summary>
        /// <param name="userId"></param>
        public void ClearSignalRConnectionId(string userId)
        {
            LiteRepository.UpdateOne(x => x.UserId == userId,
                new User() { ConnectionId = string.Empty },
                new UpdateOptions { FieldMap.connectionId });
        }
    }
}
