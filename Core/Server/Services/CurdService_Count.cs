using LiteDB;
using System.Linq.Expressions;
using Uamazing.Utils.Web.RequestModel;

namespace Uamazing.SME.Server.Services
{
    /// <summary>
    /// 数量相关的接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class CurdService
    {
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> Count<T>()
        {
            return Collection<T>().Count();
        }

        public async Task<int> Count<T>(BsonExpression predicate)
        {
            return Collection<T>().Count(predicate);
        }

        /// <summary>
        /// 摘要:
        /// Count documents matching a query. This method does not deserialize any document.
        /// Needs indexes on query expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<int> Count<T>(string predicate, BsonDocument parameters)
        {
            return Collection<T>().Count(predicate, parameters);
        }

        /// <summary>
        ///  Count documents matching a query. This method does not deserialize any document.
        ///  Needs indexes on query expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<int> Count<T>(string predicate, params BsonValue[] args)
        {
            return Collection<T>().Count(predicate, args);
        }

        /// <summary>
        ///  Count documents matching a query. This method does not deserialize any documents.
        ///  Needs indexes on query expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<int> Count<T>(Expression<Func<T, bool>> predicate)
        {
            return Collection<T>().Count(predicate);
        }


        /// <summary>
        /// Count documents matching a query. This method does not deserialize any documents.
        /// Needs indexes on query expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<int> Count<T>(Query query)
        {
            return Collection<T>().Count(query);
        }

        /// <summary>
        /// 获取当前页的数据数量
        /// </summary>
        /// <param name="filterModel"></param>
        /// <returns></returns>
        public async Task<int> GetPageModelsCount<T>(BsonExpression expression)
        {
            var count =await Count<T>(expression);
            return count;
        }
    }
}
