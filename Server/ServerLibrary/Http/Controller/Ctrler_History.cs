﻿using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json.Linq;
using ServerLibrary.Database.Extensions;
using ServerLibrary.Database.Models;
using ServerLibrary.Http.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerLibrary.Http.Controller
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
            var histories = SqlDb.Fetch<HistoryGroup>(h => h.userId == Token.UserId);

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
            var histories = SqlDb.Fetch<HistoryGroup>(h => h.userId == Token.UserId);

            var results = histories.GetPageDatas(data.filter, data.pagination);
            foreach (var item in results)
            {
               item.successCount = SqlDb.Count<SendItem>(s => s.historyId == item._id && s.isSent);
            }
            // 获取状态
            await ResponseSuccessAsync(results);
        }

        // 获取单个发件历史
        [Route(HttpVerbs.Get, "/histories/{historyId}")]
        public async Task GetHistory(string historyId)
        {
            var history = SqlDb.SingleById<HistoryGroup>(historyId);

            // 获取成功的数量
            history.successCount = SqlDb.Count<SendItem>(s => s.historyId == history._id && s.isSent);


            // 获取状态
            await ResponseSuccessAsync(history);
        }

        // 获取历史中的数量
        [Route(HttpVerbs.Post, "/histories/{historyId}/items/count")]
        public async Task GetSendItemsCount(string historyId)
        {
            var data = Body.ToObject<PageQuery>();

            // 进行筛选
            var sendItems = SqlDb.Fetch<SendItem>(s => s.historyId == historyId);

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
            var sendItems = SqlDb.Fetch<SendItem>(s => s.historyId == historyId);
               
            var results = sendItems.GetPageDatas(data.filter,data.pagination);

            // 获取状态
            await ResponseSuccessAsync(results);
        }

        // 删除发件历史 
        [Route(HttpVerbs.Delete, "/histories/{historyId}")]
        public async Task DeleteHistoryGroup(string historyId)
        {
            // 删除发送记录
            SqlDb.DeleteMany<SendItem>(item => item.historyId == historyId);

            // 删除组
            SqlDb.Delete<HistoryGroup>(historyId);

            // 获取状态
            await ResponseSuccessAsync(true);
        }
    }
}
