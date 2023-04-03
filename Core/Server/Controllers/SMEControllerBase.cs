using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Uamazing.Utils.DotNETCore.Token;
using Uamazing.Utils.Extensions;
using Uamazing.Utils.Token.Extensions;

namespace Uamazing.SME.Server.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SMEControllerBase : ControllerBase
    {
        #region 控制器通用的方法
        /// <summary>
        /// 获取 token 值
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        protected string GetToken()
        {
            string tokenHeader = Request.Headers[HeaderNames.Authorization].ToString();
            if (string.IsNullOrEmpty(tokenHeader))
                throw new ArgumentNullException("缺少token!");

            string pattern = "^Bearer (.*?)$";
            if (!Regex.IsMatch(tokenHeader, pattern))
                throw new Exception("token格式不对!格式为:Bearer {token}");

            string? token = Regex.Match(tokenHeader, pattern)?.Groups[1]?.ToString();
            if (string.IsNullOrEmpty(token))
                throw new Exception("token不能为空!");

            return token;
        }

        /// <summary>
        /// 获取 token 中的信息，第一个值是 userId,第二个值目前空缺
        /// </summary>
        /// <returns></returns>
        protected (string, string) GetTokenInfo(TokenParams tokenParams)
        {
            var token = GetToken();
            var userId = tokenParams.GetTokenPayload(token).ValueOrDefault("userId", "");
            return (userId, token);
        }

        private  JObject _requestBody;
        protected JObject RequestBody
        {
            get
            {
                if (_requestBody == null)
                {
                    StreamReader sr = new StreamReader(Request.Body);
                    var json = sr.ReadToEndAsync().GetAwaiter().GetResult();
                    // 读取数据
                    _requestBody = JObject.Parse(json);
                }

                return _requestBody;
            }
        }
        #endregion

    }
}
