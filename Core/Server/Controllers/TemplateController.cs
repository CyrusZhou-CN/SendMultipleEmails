using Microsoft.AspNetCore.Mvc;
using Uamazing.SME.Server.Models;
using Uamazing.SME.Server.Services;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;

namespace Uamazing.SME.Server.Controllers
{
    /// <summary>
    /// 模板控制器
    /// </summary>
    public class TemplateController : CurdController<Template>
    {
        public TemplateController(CurdService curdService) : base(curdService)
        {
        }

        /// <summary>
        /// 获取所有的模板
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<ResponseResult<IEnumerable<Template>>> GetTemplates()
        {
            var allTemplates =await CurdService.GetAllModels<Template>(x=>true);
            return allTemplates.ToSuccessResponse();
        }
    }
}
