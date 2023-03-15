using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Uamazing.ConfValidatation.Core.Entrance;
using Uamazing.ConfValidatation.Core.Validators;
using Uamazing.SME.Server.Models;
using Uamazing.SME.Server.Services;
using Uamazing.Utils.DotNETCore.Token;
using Uamazing.Utils.Web.ResponseModel;

namespace Uamazing.SME.Server.Controllers
{
    /// <summary>
    /// 分组的控制器
    /// </summary>
    public class OutboxGroupController : CurdController<OutboxGroup>
    {
        private TokenParams _tokenParams;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="curdService"></param>
        public OutboxGroupController(CurdService<OutboxGroup> curdService,  IOptions<TokenParams> options) : base(curdService)
        {
            _tokenParams = options.Value;
        }

        ///// <summary>
        ///// 创建组
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        public override async Task<ResponseResult<OutboxGroup>> Create([FromBody] OutboxGroup data)
        {
            var (userId, _) = GetTokenInfo(_tokenParams);

            // 验证数据
            data.Validate(new VdObj
            {
                { ()=>data.Name,new IsString("组名不能为空")}
            });
            data.UserId = userId;

            // 判断组名是否重复
            if (CurdService.GetFirstOrDefault(x => x.Name == data.Name) != null) return new ErrorResponse<OutboxGroup>($"{data.Name} 已经存在");

            return await base.Create(data);
        }
    }
}
