using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Uamazing.ConfValidatation.Core.Entrance;
using Uamazing.ConfValidatation.Core.Validators;
using Uamazing.SME.Server.Models;
using Uamazing.SME.Server.Services;
using Uamazing.Utils.DotNETCore.Token;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.RequestModel;
using Uamazing.Utils.Web.ResponseModel;
using Uamazing.Utils.Json;
using Uamazing.Utils.Extensions;

namespace Uamazing.SME.Server.Controllers
{
    /// <summary>
    /// 分组的控制器
    /// </summary>
    public class EmailBoxGroupController : CurdController<EmailBoxGroup>
    {
        private TokenParams _tokenParams;
        private EmailBoxGroupService _emailBoxGroupService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="curdService"></param>
        public EmailBoxGroupController(EmailBoxGroupService curdService, IOptions<TokenParams> options) : base(curdService)
        {
            _tokenParams = options.Value;
            _emailBoxGroupService = curdService;
        }

        /// <summary>
        /// 根据类型获取所有的组
        /// </summary>
        /// <param name="groupType"></param>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<ResponseResult<IEnumerable<EmailBoxGroup>>> GetAllEmailGroupsByType([FromQuery] GroupType groupType)
        {
            var (userId,_) = GetTokenInfo(_tokenParams);
            var results = await _emailBoxGroupService.GetAllModels<EmailBoxGroup>(x => x.UserId== userId && x.GroupType == groupType);
            return results.ToSuccessResponse();
        }

        /// <summary>
        /// 创建组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task<ResponseResult<EmailBoxGroup>> Create([FromBody] EmailBoxGroup data)
        {
            var (userId, _) = GetTokenInfo(_tokenParams);

            // 验证数据
            data.Validate(new VdObj
            {
                { ()=>data.Name,new IsString("组名不能为空")}
            });
            data.UserId = userId;

            // 判断组名是否重复
            if (await CurdService.GetFirstOrDefault<EmailBoxGroup>(x => x.GroupType == data.GroupType && x.Name == data.Name && x.ParentId == data.ParentId) != null)
                return new ErrorResponse<EmailBoxGroup>($"{data.Name} 已经存在");

            // 添加序号
            if (data.Order < 1) data.Order = DateTime.Now.ToTimestamp();

            var result = await _emailBoxGroupService.AddTreeNode(data);
            return result.ToSuccessResponse();
        }
    }
}
