using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Uamazing.SME.Server.Models;
using Uamazing.SME.Server.Services;
using Uamazing.Utils.DotNETCore.Token;
using Uamazing.Utils.Extensions;
using Uamazing.Utils.Validate;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;

namespace Uamazing.SME.Server.Controllers
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UserController : SMEControllerBase
    {
        private readonly ILiteRepository _liteRepository;
        private readonly UserService _userService;
        private readonly TokenParams _tokenParams;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="liteRepository"></param>
        public UserController(ILiteRepository liteRepository, UserService userService, IOptions<TokenParams>  tokenParams)
        {
            _liteRepository = liteRepository;
            _userService = userService;
            _tokenParams = tokenParams.Value;
        }

        /// <summary>
        /// 新建用户
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ResponseResult<User>> CreateUser([FromBody] User user)
        {
            // 用户名重复检查
            var existUser = await _userService.GetUser(user.UserId);
            if (existUser != null) return new ErrorResponse<User>($"用户 {existUser.UserId} 已经存在");

            // 密码校验
            if (string.IsNullOrEmpty(user.Password)) return new ErrorResponse<User>("请输入密码");

            // 返回新建用户
            var newUser = await _userService.CreateUser(user);
            return newUser.ToSuccessResponse();
        }

        /// <summary>
        /// 获取 token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("/token")]
        public async Task<ResponseResult<string>> GetToken([FromQuery] string userId, [FromQuery] string password)
        {
            // 验证用户名密码是否正确
            if (string.IsNullOrEmpty(userId)) return new ErrorResponse<string>("请输入用户名");
            if (string.IsNullOrEmpty(password)) return new ErrorResponse<string>("请输入密码");

            // 验证用户名和密码
            var passwordMd5 = password.EncryptMD5().Data;
            var user = await _userService.GetUser(userId, passwordMd5);
            if (user == null) return new ErrorResponse<string>("用户名或密码错误");

            // 生成 token
            var token = JWTToken.CreateToken(_tokenParams, new Dictionary<string, string>()
            {
                { "userId",user.UserId}
            });
            return token.Data.ToSuccessResponse();
        }
    }
}
