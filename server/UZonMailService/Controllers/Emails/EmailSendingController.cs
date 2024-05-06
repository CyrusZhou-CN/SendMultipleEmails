using Microsoft.AspNetCore.Mvc;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Models.SqlLite.Templates;
using UZonMailService.Services.EmailSending;

namespace UZonMailService.Controllers.Emails
{
    /// <summary>
    /// 发件相关接口
    /// </summary>
    public class EmailSendingController(SqlContext db, EmailSendingService sendingService) : ControllerBaseV1
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("now")]
        public async Task<ResponseResult<bool>> SendNow([FromBody] SendingGroup sendingData)
        {
            // 校验数据
            sendingData.Validate();
            bool result = await sendingService.CreateAndSendNow(sendingData);
            return result.ToSuccessResponse();
        }
    }
}
