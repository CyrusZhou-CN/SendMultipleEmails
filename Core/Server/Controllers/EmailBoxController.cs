using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Uamazing.ConfValidatation.Core.Entrance;
using Uamazing.ConfValidatation.Core.Validators;
using Uamazing.SME.Server.Models;
using Uamazing.SME.Server.Services;
using Uamazing.Utils.DotNETCore.Token;
using Uamazing.Utils.Web.RequestModel;
using Uamazing.Utils.Web.ResponseModel;
using Uamazing.Utils.Json;
using System.Text.RegularExpressions;
using Uamazing.Utils.Web.Extensions;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace Uamazing.SME.Server.Controllers
{
    /// <summary>
    /// 邮箱
    /// </summary>
    [Route("api/v1/email-box-group/{groupId:int}/email-box")]
    public abstract class EmailBoxController : SMEControllerBase
    {
        private TokenParams _tokenParams;
        private CurdService<EmailBox> _curdService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="curdService"></param>
        public EmailBoxController(CurdService<EmailBox> curdService, IOptions<TokenParams> options)
        {
            _tokenParams = options.Value;
            _curdService = curdService;
        }

        /// <summary>
        /// 获取邮箱数量
        /// </summary>
        /// <returns></returns>
        [HttpPost("count")]
        public async Task<ResponseResult<int>> GetEmailBoxesCount(int groupId)
        {
            var filter = RequestBody.SelectTokenOrDefault("fields", new FilterModel());
            var regex = new Regex(filter.Filter, RegexOptions.IgnoreCase);
            var count = await _curdService.Count(x => x.GroupId == groupId && (regex.IsMatch(x.Email) || regex.IsMatch(x.UserName)));
            return count.ToSuccessResponse();
        }

        /// <summary>
        /// 获取邮箱分页数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("datas")]
        public async Task<ResponseResult<IEnumerable<EmailBox>>> GetEmailBoxDatas(int groupId)
        {
            var filter = RequestBody.SelectTokenOrDefault("fields", new FilterModel());
            var pagination = RequestBody.SelectTokenOrDefault("pagination", new PaginationModel());

            var regex = new Regex(filter.Filter, RegexOptions.IgnoreCase);
            var results = await _curdService.GetPageModels(x => x.GroupId == groupId && (regex.IsMatch(x.Email) || regex.IsMatch(x.UserName)), pagination);
            return results.ToSuccessResponse();
        }


        ///// <summary>
        ///// 创建邮箱
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        [HttpPost()]
        public override async Task<ResponseResult<EmailBox>> Create(int groupId, [FromBody] JObject data)
        {
            var (userId, _) = GetTokenInfo(_tokenParams);

            // 找到组

            // 根据组类型，来判断
            if (!_curdService.Collection<EmailBoxGroup>().Exists(x => x.Id == groupId)) throw new ArgumentNullException($"未找到组 {groupId}");            

            // 判断是否在组里
            if(_curdService.Collection<EmailBox>().Exists(x=>x.GroupId== groupId && x.Email == data.SelectToken())) 
                throw new ArgumentNullException($"邮箱 {data.Email} 已经存在");

            _curdService.Collection<EmailBox>().Insert()
            // 将数据转换成
            // 验证数据
            data.Validate(new VdObj
            {
                { ()=>data.Email,new IsString("邮箱不能为空")}
            });
            data.UserId = userId;

            // 判断组名是否重复
            if (_curdService.GetFirstOrDefault(x => x.Email == data.Email) != null) return new ErrorResponse<T>($"{data.Email} 已经存在");

            return await base.Create(data);
        }
    }
}
