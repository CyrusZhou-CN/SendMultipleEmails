using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Uamazing.ConfValidatation.Core.Entrance;
using Uamazing.ConfValidatation.Core.Validators;
using UZonMailService.Services;
using Uamazing.Utils.Web.RequestModel;
using Uamazing.Utils.Web.ResponseModel;
using Uamazing.Utils.Json;
using System.Text.RegularExpressions;
using Uamazing.Utils.Web.Extensions;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using LiteDB;
using UZonMailService.Services.Settings;
using UZonMailService.Models.LiteDB;

namespace UZonMailService.Controllers
{
    /// <summary>
    /// 邮箱
    /// </summary>
    [Route("api/v1/email-box-group/{groupId}/email-box")]
    public class EmailBoxController(CRUDService curdService, TokenService tokenService) : ControllerBaseV1
    {
        /// <summary>
        /// 获取邮箱数量
        /// </summary>
        /// <returns></returns>
        [HttpPost("count")]
        public async Task<ResponseResult<int>> GetDatasCount(string groupId, [FromBody] JObject body)
        {
            var filter = body.SelectTokenOrDefault("filter", new FilterModel());
            var query = BsonExpression.Create($"$.groupId='{groupId}'");
            if (!string.IsNullOrEmpty(filter.Filter))
            {
                query = Query.And(query, Query.Or(Query.Contains("email", filter.Filter), Query.Contains("description", filter.Filter)));
            }

            var count = await curdService.GetCountInCurrentPage<EmailBox>(query);
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
            var results = await curdService.FindAllInPaper<EmailBox>(query, pagination);
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
            var (userId, _) =tokenService.GetTokenInfo();

            // 获取组
            var group = curdService.Collection<EmailBoxGroup>().FindOne(x => x.Id == groupId) ?? throw new ArgumentNullException($"未找到组 {groupId}");

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
            if (await curdService.FirstOrDefault<EmailBox>(x => x.Email == emailBox.Email && x.GroupId == groupId) != null) return new ErrorResponse<EmailBox>($"{emailBox.Email} 已经存在");

            // 创建邮箱
            var newResult = await curdService.Create(emailBox);
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
            await curdService.DeleteById<EmailBox>(emailBoxId);
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
            var emailBox = curdService.Collection<Outbox>().FindById(emailBoxId) ?? throw new ArgumentNullException($"email box:{emailBoxId} not exist");
            // 修改邮箱数据
            emailBox = JsonHelper.UpdateModelByJObject(emailBox, data);
            curdService.Collection<EmailBox>().Update(emailBox);

            return emailBox.ToSuccessResponse();
        }
    }
}
