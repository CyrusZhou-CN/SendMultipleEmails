using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json.Linq;
using Server.Database.Extensions;
using Server.Database.Models;
using Server.Http.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server.Http.Controller
{
    /// <summary>
    /// 发件历史
    /// </summary>
    public class Ctrler_History : BaseControllerAsync
    {
        // 获取发件历史数量
        [Route(HttpVerbs.Post, "/histories/count")]
        public async Task GetHistoriesCount()
        {
            var data = Body.ToObject<PageQuery>();
            // 进行筛选
            var histories = LiteDb.Fetch<SendTask>(h => h.UserId == Token.UserId);

            var count = histories.GetPageDatasCount(data.filter);

            // 获取状态
            await ResponseSuccessAsync(histories.Count());
        }

        // 获取发件历史列表
        [Route(HttpVerbs.Post, "/histories/list")]
        public async Task GetHistoriesList()
        {
            var data = Body.ToObject<PageQuery>();
            var regex = new Regex(data.filter.filter);

            // 进行筛选
            var histories = LiteDb.Fetch<SendTask>(h => h.UserId == Token.UserId);

            var results = histories.GetPageDatas(data.filter, data.pagination);

            // 获取状态
            await ResponseSuccessAsync(results);
        }

        // 获取单个发件历史
        [Route(HttpVerbs.Get, "/histories/{historyId}")]
        public async Task GetHistory(string historyId)
        {
            var history = LiteDb.SingleById<SendTask>(historyId);

            // 获取成功的数量
            history.SuccessCount = LiteDb.Fetch<SendItem>(s => s.TaskId == history.Id && s.IsSent).Count;


            // 获取状态
            await ResponseSuccessAsync(history);
        }

        // 获取历史中的数量
        [Route(HttpVerbs.Post, "/histories/{historyId}/items/count")]
        public async Task GetSendItemsCount(string historyId)
        {
            var data = Body.ToObject<PageQuery>();

            // 进行筛选
            var sendItems = LiteDb.Fetch<SendItem>(s => s.TaskId == historyId);

            var count = sendItems.GetPageDatasCount(data.filter);

            // 获取状态
            await ResponseSuccessAsync(count);
        }

        // 获取发件历史中的数据
        [Route(HttpVerbs.Post, "/histories/{historyId}/items/list")]
        public async Task GetSendItemsList(string historyId)
        {
            var data = Body.ToObject<PageQuery>();

            // 进行筛选
            var sendItems = LiteDb.Fetch<SendItem>(s => s.TaskId == historyId);
               
            var results = sendItems.GetPageDatas(data.filter,data.pagination);

            // 获取状态
            await ResponseSuccessAsync(results);
        }

        // 删除发件历史 
        [Route(HttpVerbs.Delete, "/histories/{historyId}")]
        public async Task DeleteHistoryGroup(string historyId)
        {
            // 删除发送记录
            LiteDb.DeleteMany<SendItem>(item => item.TaskId == historyId);

            // 删除组
            LiteDb.Delete<SendTask>(historyId);

            // 获取状态
            await ResponseSuccessAsync(true);
        }
    }
}
