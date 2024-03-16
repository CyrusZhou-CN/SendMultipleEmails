using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Uamazing.UZonEmail.Server.Services;
using Uamazing.Utils.Database.LiteDB;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;

namespace Uamazing.UZonEmail.Server.Controllers
{
    /// <summary>
    /// 对单表的增删改查
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// 构造函数
    /// </remarks>
    /// <param name="liteRepository"></param>
    public abstract class CurdController<T>(CRUDService curdService) : ControllerBaseV1 where T : AutoObjectId
    {
        protected CRUDService CurdService { get; set; } = curdService;

        /// <summary>
        /// 新建文档
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ResponseResult<T>> Create([FromBody] T data)
        {
            await CurdService.Create(data);
            return data.ToSuccessResponse();
        }

        /// <summary>
        /// 通过 id 更新其数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public virtual async Task<ResponseResult<T>> UpdateOne(string id, [FromBody] T data)
        {
            // 修改某个值
            var result = await CurdService.UpdateOne(x => x.Id == id, data, null);
            return result.ToSuccessResponse();
        }

        /// <summary>
        /// 通过 id 获取值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public virtual async Task<ResponseResult<T>> GetById(string id)
        {
            var result = await CurdService.FirstOrDefault<T>(x => x.Id == id);
            return result.ToSuccessResponse();
        }

        /// <summary>
        /// 通过 id 删除值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public virtual async Task DeleteById(string id)
        {
            await CurdService.DeleteById<T>(id);
        }
    }
}
