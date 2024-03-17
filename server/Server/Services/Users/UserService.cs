using LiteDB;
using Microsoft.Extensions.Options;
using Uamazing.UZonEmail.Server.Database;
using Uamazing.UZonEmail.Server.Models;
using Uamazing.Utils.Database.LiteDB;
using Uamazing.Utils.Web.Token;

namespace Uamazing.UZonEmail.Server.Services.Litedb
{
    /// <summary>
    /// 用户相关的操作
    /// </summary>
    public class UserService : LiteDbServiceBase
    {
        private TokenParams _tokenParams;
        private SettingService _settingService;

        public UserService(ILiteRepository liteRepository, IOptions<TokenParams> tokenParams, SettingService settingService) : base(liteRepository)
        {
            _tokenParams = tokenParams.Value;
            _settingService = settingService;
        }

        public async Task<User> GetUser(string userId)
        {
            var result = LiteRepository.FirstOrDefault<User>(x => x.UserId == userId);
            return result;
        }

        public async Task<User> GetUser(string userId, string password)
        {
            var result = LiteRepository.FirstOrDefault<User>(x => x.UserId == userId && x.Password == password);
            return result;
        }

        /// <summary>
        /// 获取所有用户数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetUsersCount()
        {
            var result = LiteRepository.Database.GetCollection<User>().Count();
            return result;
        }

        /// <summary>
        /// 新建用户
        /// 若表中没有用户，则将该用户当成管理员
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordMD5"></param>
        /// <returns></returns>
        public async Task<User> CreateUser(string userId, string passwordMD5)
        {
            var user = new User()
            {
                UserId = userId,
                Password = passwordMD5
            };

            // 判断是否是管理员
            var count = await GetUsersCount();
            user.IsAdmin = count < 1;

            LiteRepository.Insert(user);
            return user;
        }

        /// <summary>
        /// 通过用户名和密码来生成 token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordMd5"></param>
        /// <returns></returns>
        public async Task<string> GenerateToken(string userId, string passwordMd5)
        {
            _ = await GetUser(userId, passwordMd5) ?? throw new NullReferenceException("用户名或密码错误");

            return GenerateTokenString(userId);
        }

        /// <summary>
        /// 将 userId 作为 payload 生成 token 字符串
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GenerateTokenString(string userId)
        {
            // 生成 token
            var token = _tokenParams.CreateToken(new Dictionary<string, string>()
            {
                { "userId",userId}
            });
            return token;
        }

        /// <summary>
        /// 用户登陆并生成一个 token 返回
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordMd5"></param>
        /// <returns></returns>
        public async Task<string> SignIn(string userId, string passwordMd5)
        {
            // 获取用户
            _ = await GetUser(userId, passwordMd5) ?? throw new NullReferenceException("用户名或密码错误");

            // 生成 token
            var token = GenerateTokenString(userId);

            // 判断是否有用户设置，如果没有设置，则新建
            await _settingService.CreateUserSetting(userId);

            return token;
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
                new UpdateOptions() { Fields.connectionId });
        }

        /// <summary>
        /// 将用户的 connectionId 取消
        /// </summary>
        /// <param name="userId"></param>
        public void ClearSignalRConnectionId(string userId)
        {
            LiteRepository.UpdateOne(x => x.UserId == userId,
                new User() { ConnectionId = string.Empty },
                new UpdateOptions { Fields.connectionId });
        }
    }
}
