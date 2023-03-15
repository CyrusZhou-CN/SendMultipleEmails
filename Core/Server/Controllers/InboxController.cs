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
    public class InboxController : CurdController<Inbox>
    {
        private TokenParams _tokenParams;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="curdService"></param>
        public InboxController(CurdService<Inbox> curdService, IOptions<TokenParams> options) : base(curdService)
        {
            _tokenParams = options.Value;
        }

        ///// <summary>
        ///// 创建收件箱
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        public override async Task<ResponseResult<Inbox>> Create([FromBody] Inbox data)
        {
            var (userId, _) = GetTokenInfo(_tokenParams);

            // 验证数据
            data.Validate(new VdObj
            {
                { ()=>data.Email,new IsString("邮箱不能为空")}
            });
            data.UserId = userId;

            // 判断组名是否重复
            if (CurdService.GetFirstOrDefault(x => x.Email == data.Email) != null) return new ErrorResponse<Inbox>($"{data.Email} 已经存在");

            return await base.Create(data);
        }
    }
}
