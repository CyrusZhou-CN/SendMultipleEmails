using LiteDB;

namespace Uamazing.SME.Server.Services
{
    /// <summary>
    /// 模板服务
    /// </summary>
    public class TemplateService:CurdService
    {
        public TemplateService(ILiteRepository liteRepository) : base(liteRepository)
        {
        }
    }
}
