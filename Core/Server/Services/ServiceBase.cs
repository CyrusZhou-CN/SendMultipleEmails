using LiteDB;
using Uamazing.Utils.DotNETCore.Service;

namespace Uamazing.SME.Server.Services
{
    /// <summary>
    /// 所有服务的基类
    /// </summary>
    public abstract class ServiceBase:IService
    {
        /// <summary>
        /// liteDB 数据仓库
        /// </summary>
        protected ILiteRepository LiteRepository { get; private set; }

        /// <summary>
        /// liteDB 数据库
        /// </summary>
        protected ILiteDatabase Database=>LiteRepository.Database;

        public ServiceBase(ILiteRepository liteRepository)
        {
            LiteRepository = liteRepository;
        }

        /// <summary>
        /// 根据集合名称获取集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public ILiteCollection<T> Collection<T>(string? collectionName = null)
        {
           return Database.GetCollection<T>(collectionName);
        }
    }
}
