using LiteDB;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Uamazing.UZonEmail.Server.Services.Litedb;
using Uamazing.Utils.Database.LiteDB;
using Uamazing.Utils.Web.RequestModel;

namespace Uamazing.UZonEmail.Server.Services
{
    /// <summary>
    /// 通用的增删查改组件
    /// </summary>
    public partial class CRUDService : LiteDbServiceBase
    {
        public CRUDService(ILiteRepository liteRepository) : base(liteRepository)
        {
        }

        /// <summary>
        /// 新增
        /// 会在原来数据上增加 id 字段，返回值与传入参数是一个引用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<T> Create<T>(T model)
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
        public async Task<T> UpdateOne<T>(Expression<Func<T, bool>> filter, T model,UpdateOptions updateOptions)
        {
            var defaultUpdateOptions = updateOptions;
            if (updateOptions == null)
            {
                defaultUpdateOptions = UpdateOptions.CreateIgnoreDefaultValue(model);
            }
                
            var result = LiteRepository.UpdateOne(filter, model, defaultUpdateOptions);
            return result;
        }

        /// <summary>
        /// 获取第一个值或者返回默认值
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefault<T>(Expression<Func<T, bool>> filter)
        {
            var result = LiteRepository.FirstOrDefault(filter);
            return result;
        }

        /// <summary>
        /// 获取所有匹配项
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindAll<T>(Expression<Func<T, bool>> filter)
        {
            var results = LiteRepository.Fetch(filter);
            return results;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paginationModel"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindAllInPaper<T>(BsonExpression filter,PaginationModel paginationModel)
        {
            var query = LiteRepository.Query<T>().Where(filter);
            if (paginationModel.Descending)
            {
                query = query.OrderByDescending($"$.{paginationModel.SortBy}");
            }
            else
            {
                query = query.OrderBy($"$.{paginationModel.SortBy}");
            }
            var results = query.Skip(paginationModel.Skip).Limit(paginationModel.Limit);
            return results.ToList();
        }

        /// <summary>
        /// 通过 id 删除值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteById<T>(string id)
        {
            LiteRepository.Delete<T>(id);
        }

        /// <summary>
        /// 通过过滤条件删除值
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task DeleteMany<T>(Expression<Func<T, bool>> filter)
        {
            LiteRepository.DeleteMany(filter);
        }
    }
}
