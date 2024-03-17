using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Uamazing.UZonEmail.Server.Config;
using Uamazing.UZonEmail.Server.Config.SubConfigs;
using Uamazing.UZonEmail.Server.Models;
using Uamazing.UZonEmail.Server.Services;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Uamazing.UZonEmail.Server.Controllers.Settings
{
    /// <summary>
    /// 系统设置
    /// </summary>
    public class SystemSettingController(CRUDService crudService, IOptions<AppConfig> appConfig) : ControllerBaseV1
    {
        /// <summary>
        /// 获取版权和 ICP 信息
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("login-settings")]
        public async Task<ResponseResult<JObject>> GetCopyrightAndICPInfo()
        {
            bool existUser = crudService.Collection<User>().Exists(x => true);

            return new JObject() {
                { "copyright", appConfig.Value.System.Copyright },
                { "icp", appConfig.Value.System.ICP },
                { "emptyUser",existUser}
            }.ToSuccessResponse();
        }
    }
}
