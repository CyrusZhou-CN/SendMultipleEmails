using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Json.Internal;
using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.Sender;
using UZonMailService.Services.EmailSending.WaitList;
using UZonMailService.Services.Settings;
using UZonMailService.Services.UserInfos;
using UZonMailService.SignalRHubs;
using UZonMailService.Utils.Database;

namespace UZonMailService.Services.EmailSending
{
    /// <summary>
    /// 单例级别的服务
    /// </summary>
    public class EmailSendingService : IScopedService
    {
        public static EmailSendingService Instance { get; private set; }

        public SqlContext Db { get; private set; }
        public IHubContext<UzonMailHub, IUzonMailClient> HubContext { get; private set; }
        public SystemSendingWaitListService WaitList { get; private set; }
        public SystemTasksService TasksService { get; private set; }
        public UserOutboxesPool OutboxPool { get; private set; }

        private TokenService _tokenService;

        /// <summary>
        /// 初始化构造
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sendHub"></param>
        /// <param name="waitList"></param>
        /// <param name="tasksService"></param>
        /// <param name="outboxPool"></param>
        public EmailSendingService(SqlContext db
            , IHubContext<UzonMailHub, IUzonMailClient> sendHub
            , SystemSendingWaitListService waitList
            , SystemTasksService tasksService
            , UserOutboxesPool outboxPool
            , TokenService tokenService
            )
        {
            Instance = this;

            Db = db;
            HubContext = sendHub;
            WaitList = waitList;
            TasksService = tasksService;
            OutboxPool = outboxPool;

            _tokenService = tokenService;
        }

        /// <summary>
        /// 获取指定用户的 SignalR 客户端
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static IUzonMailClient GetSignalRClient(int userId)
        {
            return Instance.HubContext.Clients.User(userId.ToString());
        }

        /// <summary>
        /// 创建发送组
        /// </summary>
        /// <param name="sendingGroup"></param>
        /// <returns></returns>
        public async Task<SendingGroup> CreateSendingGroup(SendingGroup sendingGroup)
        {
            int userId = _tokenService.GetIntUserId();
            // 使用事务
            await Db.RunTransaction(async ctx =>
            {
                // 添加数据
                // 跟踪数据
                sendingGroup.Templates?.ForEach(x => ctx.Attach(x));
                sendingGroup.Outboxes?.ForEach(x => ctx.Attach(x));

                // 增加数据
                sendingGroup.UserId = userId;
                sendingGroup.Status = SendingGroupStatus.Created;
                sendingGroup.SendingType = SendingGroupType.Instant;
                // 解析总数
                sendingGroup.TotalCount = sendingGroup.Inboxes.Count;

                ctx.SendingGroups.Add(sendingGroup);
                await ctx.SaveChangesAsync();

                // 将数据组装成 SendingItem 保存
                List<SendingItem> items = sendingGroup.GenerateSendingItems();
                ctx.SendingItems.AddRange(items);
                // 更新发件数量
                sendingGroup.TotalCount = items.Count;

                // 增加附件使用记录
                if (sendingGroup.Attachments != null && sendingGroup.Attachments.Count > 0)
                {
                    var attachmentObjectIds = sendingGroup.Attachments.Select(x => x.FileObjectId).ToList();
                    await ctx.FileObjects.UpdateAsync(x => attachmentObjectIds.Contains(x.Id), obj => obj.SetProperty(x => x.LinkCount, y => y.LinkCount + 1));
                }

                return await ctx.SaveChangesAsync();
            });


            return sendingGroup;
        }

        /// <summary>
        /// 立即发件
        /// </summary>
        /// <param name="sendingData"></param>
        /// <returns></returns>
        public async Task<bool> CreateAndSendNow(SendingGroup sendingData)
        {
            // 使用事务
            var sendingGroup = await CreateSendingGroup(sendingData);

            // 向 waitList 中添加数据
            await WaitList.AddSendingGroup(sendingGroup);

            return true;
        }
    }
}
