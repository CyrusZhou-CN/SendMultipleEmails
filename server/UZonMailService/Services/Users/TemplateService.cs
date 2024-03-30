using LiteDB;

namespace UZonMailService.Services.Litedb
{
    /// <summary>
    /// 模板服务
    /// </summary>
    public class TemplateService : CRUDService
    {
        public TemplateService(ILiteRepository liteRepository) : base(liteRepository)
        {
        }
    }
}
