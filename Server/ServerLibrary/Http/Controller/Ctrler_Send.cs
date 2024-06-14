using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json.Linq;
using ServerLibrary.Database;
using ServerLibrary.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ServerLibrary.Http.Definitions;
using Newtonsoft.Json;
using ServerLibrary.Http.Modules.SendEmail;
using Microsoft.Web.WebView2.Wpf;
using ServerLibrary.Database.Extensions;

namespace ServerLibrary.Http.Controller
{
    /// <summary>
    /// 本类主要用于发送，一次只能一个发送任务
    /// </summary>
    public class Ctrler_Send : BaseControllerAsync
    {
        // 获取发送状态，用于是否恢复界面的发送情况
        [Route(HttpVerbs.Get, "/send/status")]
        public async Task GetSendStatus()
        {
            SendStatus sendStatus = SendStatus.SendFinish;
            if (InstanceCenter.SendTasks[Token.UserId] != null)
            {
                sendStatus = InstanceCenter.SendTasks[Token.UserId].SendStatus;
            }

            await ResponseSuccessAsync(sendStatus);
        }

        // 新建预览
        [Route(HttpVerbs.Post, "/send/preview")]
        public async Task CreatePreview()
        {
            bool createResult = EmailPreview.CreateEmailPreview(Token.UserId, Body, SqlDb, out string message);
            if (createResult)
            {
                InstanceCenter.EmailPreview[Token.UserId].Generate();
                await ResponseSuccessAsync(createResult);
            }
            else await ResponseErrorAsync(message);
        }

        // 获取预览内容
        [Route(HttpVerbs.Get, "/send/preview/{key}")]
        public async Task GetSendPreview(string key)
        {
            SendItem item = InstanceCenter.EmailPreview[Token.UserId].GetPreviewHtml(key);
            while (item == null)
            {
                System.Threading.Thread.Sleep(1000);
                item = InstanceCenter.EmailPreview[Token.UserId].GetPreviewHtml(key);
            }

            if (item == null)
            {
                await ResponseErrorAsync("没有可预览项，请检查收件箱是否为空");
                return;
            }

            await ResponseSuccessAsync(item);
        }

        // 新建发送准备
        [Route(HttpVerbs.Post, "/send/task")]
        public async Task CreateTask()
        {
            bool createResult = EmailReady.CreateEmailReady(Token.UserId, Body, SqlDb, out string message);
            if (!createResult) await ResponseErrorAsync(message);

            // 在后台执行邮件生成和发送任务
            _ = Task.Run(async () =>
            {
                try
                {
                    InstanceCenter.EmailReady[Token.UserId].Generate();
                    await InstanceCenter.SendTasks[Token.UserId].StartSending();
                }
                catch (Exception ex)
                {
                    // 处理后台任务中的异常
                    // 可以记录日志或者发送通知
                    Console.WriteLine($"后台任务执行出错: {ex.Message}");
                }
            });

            var _info = new GenerateInfo
            {
                ok = true
            };
            await ResponseSuccessAsync(_info);
        }


        // 开始发送邮件
        // 也包括重新发件
        [Route(HttpVerbs.Post, "/send/tasks/{historyGroupId}")]
        public async Task StartSending(string historyGroupId)
        {
            // 因为创建 history 的时候，检查了发送模块是否进行，所以此处新建发送模块不会造成冲突
            if (!SendTask.CreateSendTask(historyGroupId, Token.UserId, SqlDb, out string message))
            {
                await ResponseErrorAsync(message);
                return;
            }

            await InstanceCenter.SendTasks[Token.UserId].StartSending();

            await ResponseSuccessAsync(historyGroupId);
        }

        // 获取发件状态
        [Route(HttpVerbs.Get, "/send/info")]
        public async Task GetSendingInfo()
        {
            await ResponseSuccessAsync(InstanceCenter.SendTasks[Token.UserId] == null ? new SendingProgressInfo() : InstanceCenter.SendTasks[Token.UserId].SendingProgressInfo);
        }

        // 获取发件状态
        [Route(HttpVerbs.Get, "/send/history/{id}/result")]
        public async Task GetHistoryResult(string id)
        {
            HistoryGroup historyGroup = SqlDb.SingleById<HistoryGroup>(id);
            // 获取成功的数量
            int successCount = SqlDb.Fetch<SendItem>(s => s.historyId == id && s.isSent).Count();

            JObject result;
            if (successCount == historyGroup.receiverIds.Count)
            {
                string msg = $"发送成功！共发送：{successCount}/{historyGroup.receiverIds.Count}";
                result = new JObject(new JProperty("message", msg), new JProperty("ok", true));
            }
            else
            {
                string msg = $"未完全发送，共发送：{successCount}/{historyGroup.receiverIds.Count}。请在发件历史中查询重发";
                result = new JObject(new JProperty("message", msg), new JProperty("ok", false));
            }
            await ResponseSuccessAsync(result);
        }
    }
}
