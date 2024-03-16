using Microsoft.AspNetCore.Mvc;
using Uamazing.UZonEmail.Server.Models;
using Uamazing.UZonEmail.Server.Services;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;

namespace Uamazing.UZonEmail.Server.Controllers
{
    /// <summary>
    /// 模板控制器
    /// </summary>
    public class TemplateController : CurdController<Template>
    {
        public TemplateController(CRUDService curdService) : base(curdService)
        {
        }

        /// <summary>
        /// 获取所有的模板
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<ResponseResult<IEnumerable<Template>>> GetTemplates()
        {
            var allTemplates =await CurdService.FindAll<Template>(x=>true);
            return allTemplates.ToSuccessResponse();
        }
    }
}
