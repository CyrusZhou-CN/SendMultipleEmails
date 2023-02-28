using LiteDB;

namespace Uamazing.SME.Server.Services
{
    /// <summary>
    /// 所有服务的基类
    /// </summary>
    public abstract class ServiceBase
    {
        /// <summary>
        /// liteDB 数据仓库
        /// </summary>
        protected LiteRepository LiteRepository { get; private set; }

        /// <summary>
        /// liteDB 数据库
        /// </summary>
        protected ILiteDatabase Database=>LiteRepository.Database;

        public ServiceBase(LiteRepository liteRepository)
        {
            LiteRepository = liteRepository;
        }
    }
}
