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
using LiteDB;

namespace Uamazing.SME.Server.Controllers
{
    /// <summary>
    /// 邮箱
    /// </summary>
    [Route("api/v1/email-box-group/{groupId}/email-box")]
    public class EmailBoxController : SMEControllerBase
    {
        private TokenParams _tokenParams;
        private CurdService _curdService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="curdService"></param>
        public EmailBoxController(CurdService curdService, IOptions<TokenParams> options)
        {
            _tokenParams = options.Value;
            _curdService = curdService;
        }

        /// <summary>
        /// 获取邮箱数量
        /// </summary>
        /// <returns></returns>
        [HttpPost("count")]
        public async Task<ResponseResult<int>> GetDatasCount(string groupId, [FromBody] JObject body)
        {
            var filter = body.SelectTokenOrDefault("filter", new FilterModel());
            var query = BsonExpression.Create($"$.groupId='{groupId}'");              
            if(!string.IsNullOrEmpty(filter.Filter))
            {
                query = Query.And(query,Query.Or(Query.Contains("email", filter.Filter), Query.Contains("description", filter.Filter)));
            }

            var count = await _curdService.GetPageModelsCount<EmailBox>(query);
            return count.ToSuccessResponse();
        }

        /// <summary>
        /// 获取邮箱分页数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("datas")]
        public async Task<ResponseResult<IEnumerable<EmailBox>>> GetDatas(string groupId, [FromBody] JObject body)
        {
            var filter = body.SelectTokenOrDefault("filter", new FilterModel());
            var pagination = body.SelectTokenOrDefault("pagination", new PaginationModel());

            var query = BsonExpression.Create($"$.groupId='{groupId}'");
            if (!string.IsNullOrEmpty(filter.Filter))
            {
                query = Query.And(query, Query.Or(Query.Contains("email", filter.Filter), Query.Contains("description", filter.Filter)));
            }
            var results = await _curdService.GetPageModels<EmailBox>(query, pagination);
            return results.ToSuccessResponse();
        }


        /// <summary>
        /// 创建邮箱
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult<EmailBox>> Create(string groupId, [FromBody] JObject data)
        {
            var (userId, _) = GetTokenInfo(_tokenParams);

            // 获取组
            var group = _curdService.Collection<EmailBoxGroup>().FindOne(x => x.Id == groupId) ??  throw new ArgumentNullException($"未找到组 {groupId}");

            // 根据组类型，实例数据
            EmailBox? emailBox;
            if (group.GroupType == GroupType.InboxGroup) emailBox = data.ToObject<Inbox>();
            else emailBox = data.ToObject<Outbox>();

            // 验证数据
            emailBox.Validate(new VdObj
            {
                { ()=>emailBox.Email,new IsString("邮箱不能为空")}
            });           

            // 判断邮箱是否重复
            if (await _curdService.GetFirstOrDefault<EmailBox>(x => x.Email == emailBox.Email && x.GroupId==groupId) != null) return new ErrorResponse<EmailBox>($"{emailBox.Email} 已经存在");

            // 创建邮箱
            var newResult = await _curdService.Create(emailBox);
            return newResult.ToSuccessResponse();
        }

        /// <summary>
        /// 通过 id 删除邮箱
        /// </summary>
        /// <param name="emailBoxId"></param>
        /// <returns></returns>
        [HttpDelete("/api/v1/email-box/{emailBoxId}")]
        public async Task<ResponseResult<bool>> DeleteById(string emailBoxId)
        {
            await _curdService.DeleteModel<EmailBox>(emailBoxId);
            return new SuccessResponse<bool>();
        }

        /// <summary>
        /// 更新邮箱设置
        /// </summary>
        /// <param name="emailBoxId"></param>
        /// <returns></returns>
        [HttpPut("/api/v1/email-box/{emailBoxId}/settings")]
        public async Task<ResponseResult<Outbox>> UpdateOutboxSettings(string emailBoxId, [FromBody] JObject data)
        {
            // 找到邮箱
            var emailBox = _curdService.Collection<Outbox>().FindById(emailBoxId)?? throw new ArgumentNullException($"email box:{emailBoxId} not exist");
            // 修改邮箱数据
            emailBox = JsonHelper.UpdateModelByJObject(emailBox,data);
            _curdService.Collection<EmailBox>().Update(emailBox);

            return emailBox.ToSuccessResponse();
        }
    }
}
