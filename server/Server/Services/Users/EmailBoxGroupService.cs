using LiteDB;
using Uamazing.UZonEmail.Server.Models;
using Uamazing.UZonEmail.Server.Services.Litedb;

namespace Uamazing.UZonEmail.Server.Services
{
    /// <summary>
    /// 删除组
    /// </summary>
    public class EmailBoxGroupService(ILiteRepository liteRepository) : TreeLikeService<EmailBoxGroup>(liteRepository)
    {

        /// <summary>
        /// 通过 Id 删除邮箱组，同时会删除邮件组中的邮箱
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteById(string groupId)
        {
            var deletedIds =await DeleteNodeById(groupId);

            // 删除发件箱
            Collection<Outbox>().DeleteMany(x => deletedIds.Contains(x.GroupId));

            // 删除收件箱
            Collection<Inbox>().DeleteMany(x=>deletedIds.Contains(x.GroupId));

            return true;
        }
    }
}
