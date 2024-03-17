using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Uamazing.Utils.Extensions;
using Uamazing.Utils.Json;

namespace Uamazing.UZonEmail.Server.Controllers
{
    /// <summary>
    /// 所有控制器基接口
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public abstract class ControllerBaseV1 : ControllerBase
    {
        #region 控制器通用的方法
        private  JObject _requestBody;
        protected JObject RequestBody
        {
            get
            {
                if (_requestBody == null)
                {
                    StreamReader sr = new(Request.Body);
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
