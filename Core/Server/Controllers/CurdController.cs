using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Uamazing.SME.Server.Services;
using Uamazing.Utils.Database.LiteDB;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;

namespace Uamazing.SME.Server.Controllers
{
    public abstract class CurdController<T> : SMEControllerBase where T : AutoObjectId
    {
        protected CurdService<T> CurdService { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="liteRepository"></param>
        public CurdController(CurdService<T> curdService)
        {
            CurdService = curdService;
        }

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
        public virtual async Task<ResponseResult<T>> UpdateOne(int id, [FromBody] T data)
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
        public virtual async Task<T> GetById(int id)
        {
            var result = await CurdService.GetFirstOrDefault(x => x.Id == id);
            return result;
        }

        /// <summary>
        /// 通过 id 删除值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public virtual async Task DeleteById(int id)
        {
            await CurdService.DeleteModel(id);
        }
    }
}
