using LiteDB;
using Uamazing.UZonEmail.Server.Modules.DotNETCore.Service;
using Uamazing.Utils.Web.Service;

namespace Uamazing.UZonEmail.Server.Services.Litedb
{
    /// <summary>
    /// 所有服务的基类
    /// </summary>
    public abstract class LiteDbServiceBase(ILiteRepository liteRepository) : IScopedService
    {
        /// <summary>
        /// liteDB 数据仓库
        /// </summary>
        protected ILiteRepository LiteRepository { get; private set; } = liteRepository;

        /// <summary>
        /// liteDB 数据库
        /// </summary>
        protected ILiteDatabase Database => LiteRepository.Database;

        /// <summary>
        /// 根据集合名称获取集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="autoId"></param>
        /// <returns></returns>
        public ILiteCollection<T> Collection<T>(string? collectionName = null, BsonAutoId autoId = BsonAutoId.ObjectId)
        {
            return Database.GetCollection<T>(collectionName, autoId);
        }
    }
}
