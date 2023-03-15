using LiteDB;
using System.Linq.Expressions;
using Uamazing.Utils.LiteDB;

namespace Uamazing.SME.Server.Services
{
    /// <summary>
    /// 通用的增删查改组件
    /// </summary>
    public class CurdService<T> : ServiceBase
    {
        public CurdService(ILiteRepository liteRepository) : base(liteRepository)
        {
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<T> Create(T model)
        {
            LiteRepository.Insert(model);
            return model;
        }

        /// <summary>
        /// 通过条件进行更新
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="model"></param>
        /// <param name="updateOptions"></param>
        /// <returns></returns>
        public async Task<T> UpdateOne(Expression<Func<T, bool>> filter, T model,UpdateOptions updateOptions)
        {
            var result = LiteRepository.UpdateOne(filter, model, updateOptions);
            return result;
        }

        /// <summary>
        /// 获取第一个值或者返回默认值
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            var result = LiteRepository.FirstOrDefault(filter);
            return result;
        }

        /// <summary>
        /// 获取所有匹配项
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllModels(Expression<Func<T, bool>> filter)
        {
            var results = LiteRepository.Fetch(filter);
            return results;
        }

        /// <summary>
        /// 通过 id 删除值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteModel(int id)
        {
            LiteRepository.Delete<T>(id);
        }

        /// <summary>
        /// 通过过滤条件删除值
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task DeleteAllModels(Expression<Func<T, bool>> filter)
        {
            LiteRepository.DeleteMany(filter);
        }
    }
}
