using LiteDB;
using Uamazing.SME.Server.Models;

namespace Uamazing.SME.Server.Services
{
    /// <summary>
    /// 删除组
    /// </summary>
    public class EmailBoxGroupService:TreeLikeService<EmailBoxGroup>
    {
        public EmailBoxGroupService(ILiteRepository liteRepository) : base(liteRepository)
        {
        }

        /// <summary>
        /// 通过 Id 删除邮箱组，同时会删除邮件组中的邮箱
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteById(int groupId)
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
