using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Json.Internal;
using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SqlLite;
using UZonMailService.Services.EmailSending.OutboxPool;
using UZonMailService.Services.EmailSending.Sender;
using UZonMailService.Services.EmailSending.WaitList;
using UZonMailService.SignalRHubs;

namespace UZonMailService.Services.EmailSending
{
    /// <summary>
    /// 单例级别的服务
    /// </summary>
    public class EmailSendingService : IScopedService
    {
        public static EmailSendingService Instance { get; private set; }

        public SqlContext DB { get; private set; }
        public IHubContext<UzonMailHub, IUzonMailClient> HubContext { get; private set; }
        public SystemSendingWaitListService WaitList { get; private set; }
        public SystemTasksService TasksService { get; private set; }
        public UserOutboxesPool OutboxPool { get; private set; }

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
                       , UserOutboxesPool outboxPool)
        {
            Instance = this;

            DB = db;
            HubContext = sendHub;
            WaitList = waitList;
            TasksService = tasksService;
            OutboxPool = outboxPool;
        }
    }
}
