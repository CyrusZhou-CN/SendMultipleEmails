using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Uamazing.ConfValidatation.Core.Entrance;
using Uamazing.ConfValidatation.Core.Validators;
using Uamazing.SME.Server.Models;
using Uamazing.SME.Server.Services;
using Uamazing.Utils.DotNETCore.Token;
using Uamazing.Utils.Extensions;
using Uamazing.Utils.Token.Extensions;
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
        private readonly UserService _userService;
        private readonly TokenParams _tokenParams;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userService"></param>
        public UserController(UserService userService, IOptions<TokenParams> tokenParams)
        {
            _userService = userService;
            _tokenParams = tokenParams.Value;
        }

        /// <summary>
        /// 新建用户
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ResponseResult<User>> SignUp([FromBody] User user)
        {
            // 用户名重复检查
            var existUser = await _userService.GetUser(user.UserId);
            if (existUser != null) return new ErrorResponse<User>($"用户 {existUser.UserId} 已经存在");

            // 验证用户
            user.Validate(new VdObj
            {
                { ()=>user.UserId,new IsString("用户名最小长度不小于3个字符"){ MinLength=3} },
                { ()=>user.Password,new IsString("密码最小长度不小于6个字符"){ MinLength=6} }
            }, ValidateOption.ThrowError);

            // 密码校验
            if (string.IsNullOrEmpty(user.Password)) return new ErrorResponse<User>("请输入密码");

            // 返回新建用户
            var newUser = await _userService.CreateUser(user.UserId,user.Password.EncryptMD5());
            return newUser.ToSuccessResponse();
        }

        /// <summary>
        /// 获取 token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("token")]
        public async Task<ResponseResult<string>> GetToken([FromQuery] string userId, [FromQuery] string password)
        {
            // 验证用户名密码是否正确
            if (string.IsNullOrEmpty(userId)) return new ErrorResponse<string>("请输入用户名");
            if (string.IsNullOrEmpty(password)) return new ErrorResponse<string>("请输入密码");

            // 生成加密密码
            var passwordMd5 = password.EncryptMD5();

            // 生成 token            
            var token = await _userService.GenerateToken(userId, passwordMd5);
            return token.ToSuccessResponse();
        }


        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns></returns>
        [HttpPost("sign-in"), AllowAnonymous]
        public async Task<ResponseResult<string>> SignIn([FromBody] User user)
        {
            // 验证用户
            user.Validate(new VdObj
            {
                { ()=>user.UserId,new NotNullOrEmpty(),"用户名为空"},
                { ()=>user.Password,new NotNullOrEmpty(),"密码为空" }
            }, ValidateOption.ThrowError);

            var token = await _userService.SignIn(user.UserId, user.Password.EncryptMD5());
            return token.ToSuccessResponse();
        }

        /// <summary>
        /// 获取当前用户的信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("info")]
        public async Task<ResponseResult<User>> GetCurrentUserInfo()
        {
            var (userId, _) = GetTokenInfo(_tokenParams);
            var user =await _userService.GetUser(userId);

            // 去掉密码
            user.Password = string.Empty;
            return user.ToSuccessResponse();
        }

        /// <summary>
        /// 退出登陆
        /// </summary>
        /// <returns></returns>
        [HttpPut("sign-out")]        
        public async Task<ResponseResult<bool>> SignOut()
        {
            // 从 token 中获取当前用户信息
            var (userId, _) = GetTokenInfo(_tokenParams);

            // 清除用户的 signR 连接


            // 返回退出成功消息
            return true.ToSuccessResponse();
        }
    }
}
