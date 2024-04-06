using Microsoft.EntityFrameworkCore;
using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.UserInfos;
using Uamazing.Utils.Extensions;
using Uamazing.Utils.Web.Extensions;
using UZonMailService.Utils.DotNETCore.Exceptions;
using Uamazing.Utils.Web.Token;
using UZonMailService.Config;
using Microsoft.Extensions.Options;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Utils.ASPNETCore.PagingQuery;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace UZonMailService.Services.UserInfos
{
    /// <summary>
    /// 只在请求生命周期内有效的服务
    /// </summary>
    public class UserService(SqlContext db, IOptions<AppConfig> appConfig) : IScopedService
    {
        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> ExistUser(string userId)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.UserId.ToLower() == userId.ToLower()) != null;
        }

        /// <summary>
        /// 创建一个新的用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password">原密码,不需要加密</param>
        /// <returns></returns>
        public async Task<User> CreateUser(string userId, string password)
        {
            var user = new User()
            {
                UserId = userId,
                Password = password.EncryptMD5()
            };
            db.Add(user);
            await db.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// 通过用户 ID 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<User> GetUserInfo(int userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId) ?? throw new KnownException(userId + "用户不存在");
            // 将密码置空
            user.Password = string.Empty;
            return user;
        }

        /// <summary>
        /// 通过 userId 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public async Task<User> GetUserInfo(string userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId) ?? throw new KnownException(userId + "用户不存在");
            // 将密码置空
            user.Password = string.Empty;
            return user;
        }

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password">密码为原值</param>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public async Task<UserSignInResult> UserSignIn(string userId, string password)
        {
            User user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId && x.Password == password.EncryptMD5())
                ?? throw new KnownException("用户名或密码错误");
            var userInfo = new User()
            {
                Id = user.Id,
                Avatar = user.Avatar,
                UserId = user.UserId,
            };

            // 生成 token
            string token = GenerateToken(user);


            // 查找用户的权限
            var userAccess = await db.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .ThenInclude(x => x.PermissionCodes)
                .FirstOrDefaultAsync(x => x.Id == user.Id);
            List<string> access = userAccess
                .UserRoles
                .SelectMany(x => x.Role.PermissionCodes)
                .Select(x => x.Code)
                .ToList();

            return new UserSignInResult()
            {
                Token = token,
                Access = access,
                UserInfo = userInfo
            };
        }
        /// <summary>
        /// 生成 token
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private string GenerateToken(User userInfo)
        {
            string token = JWTToken.CreateToken(new TokenParams()
            {
                Secret = appConfig.Value.TokenParams.Secret,
                Expire = appConfig.Value.TokenParams.Expire
            }, new Dictionary<string, string>()
            {
                { "userId",userInfo.Id.ToString()},
                { ClaimTypes.Role,userInfo.IsSuperAdmin?"admin":"user"}
            });
            return token;
        }

        /// <summary>
        /// 获取用户数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetFilteredUsersCount(string filter)
        {
            return await FilterUser(filter).CountAsync();
        }
        private IQueryable<User> FilterUser(string filter)
        {
            return db.Users.Where(x => !x.Hidden && !x.IsSuperAdmin)
                .Where(x => string.IsNullOrEmpty(filter) || x.UserId.Contains(filter));
        }

        /// <summary>
        /// 获取分页的用户
        /// 返回除了密码之外的所有信息
        /// 调用接口对数据进行再处理
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetFilteredUsersData(string filter, PageDataPick<User> pagination)
        {
            return await FilterUser(filter).Page(pagination).ToListAsync();
        }

        /// <summary>
        /// 获取用户默认密码
        /// </summary>
        /// <returns></returns>
        public string GetUserDefaultPassword()
        {
            return appConfig.Value.User.DefaultPassword;
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="KnownException"></exception>
        public async Task<bool> ResetUserPassword(string userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                throw new KnownException("用户不存在");
            }
            user.Password = appConfig.Value.User.DefaultPassword.EncryptMD5();
            await db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<bool> ChangeUserPassword(int userId, string oldPassword, string newPassword)
        {
            if (userId <= 0 || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                throw new KnownException("修改密码失败");
            }

            // 查找用户
            string encryptOldPassword = oldPassword.EncryptMD5();
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId && x.Password == encryptOldPassword);
            if (user==null)
            {
                throw new KnownException("原密码错误");
            }
            user.Password = newPassword.EncryptMD5();
            await db.SaveChangesAsync();
            return true;
        }
    }
}
