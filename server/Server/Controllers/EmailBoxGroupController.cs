using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Uamazing.ConfValidatation.Core.Entrance;
using Uamazing.ConfValidatation.Core.Validators;
using Uamazing.UZonEmail.Server.Models;
using Uamazing.UZonEmail.Server.Services;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.RequestModel;
using Uamazing.Utils.Web.ResponseModel;
using Uamazing.Utils.Json;
using Uamazing.Utils.Extensions;
using Uamazing.UZonEmail.Server.Services.Litedb;
using Uamazing.UZonEmail.Server.Services.Settings;

namespace Uamazing.UZonEmail.Server.Controllers
{
    /// <summary>
    /// 分组的控制器
    /// </summary>
    /// <remarks>
    /// 构造函数
    /// </remarks>
    /// <param name="curdService"></param>
    public class EmailBoxGroupController(EmailBoxGroupService curdService,TokenService tokenService) : CurdController<EmailBoxGroup>(curdService)
    {
        /// <summary>
        /// 根据类型获取所有的组
        /// </summary>
        /// <param name="groupType"></param>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<ResponseResult<IEnumerable<EmailBoxGroup>>> GetAllEmailGroupsByType([FromQuery] GroupType groupType)
        {
            var (userId,_) = tokenService.GetTokenInfo();
            var results = await curdService.FindAll<EmailBoxGroup>(x => x.UserId== userId && x.GroupType == groupType);
            return results.ToSuccessResponse();
        }

        /// <summary>
        /// 创建组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task<ResponseResult<EmailBoxGroup>> Create([FromBody] EmailBoxGroup data)
        {
            var (userId, _) = tokenService.GetTokenInfo();

            // 验证数据
            data.Validate(new VdObj
            {
                { ()=>data.Name,new IsString("组名不能为空")}
            });
            data.UserId = userId;

            // 判断组名是否重复
            if (await CurdService.FirstOrDefault<EmailBoxGroup>(x => x.GroupType == data.GroupType && x.Name == data.Name && x.ParentId == data.ParentId) != null)
                return new ErrorResponse<EmailBoxGroup>($"{data.Name} 已经存在");

            // 添加序号
            if (data.Order < 1) data.Order = DateTime.Now.ToTimestamp();

            var result = await curdService.AddTreeNode(data);
            return result.ToSuccessResponse();
        }
    }
}
