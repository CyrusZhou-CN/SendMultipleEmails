﻿using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json.Linq;
using ServerLibrary.Config;
using ServerLibrary.Database.Extensions;
using ServerLibrary.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerLibrary.Http.Controller
{
    /// <summary>
    /// 统计报表相关的接口
    /// </summary>
    class Ctrler_Report : BaseControllerAsync
    {
        /// <summary>
        /// 邮件总体到达率
        /// </summary>
        [Route(HttpVerbs.Get, "/report/success-rate")]
        public async Task GetAllSuccessRate()
        {
            // 找到当前的用户名
            var userId = Token.UserId;
            // 获取用户发送的历史组
            var historyGroups = SqlDb.Fetch<HistoryGroup>(g => g.userId == userId).ToList();

            if (historyGroups.Count < 1)
            {
                // 返回1
                await ResponseSuccessAsync(1);
                return;
            };

            List<string> historyIds = historyGroups.Select(hg => hg._id).ToList();

            // 查找历史组下面的所有的发件
            var sendItems = SqlDb.Queryable<SendItem>().In(it=>it.historyId, historyIds);
            if (sendItems.Count() < 1)
            {
                // 返回1
                await ResponseSuccessAsync(1);
                return;
            }

            // 计算比例
            var successItems = sendItems.FindAll(item => item.isSent);
            await ResponseSuccessAsync(successItems.Count() * 1.0 / sendItems.Count());
        }

        /// <summary>
        /// 收件箱种类和数量
        /// </summary>
        [Route(HttpVerbs.Get, "/report/inbox-type-count")]
        public async Task GetINboxTypeAndCount()
        {
            // 找到当前的用户名
            var userId = Token.UserId;
            // 获取用户发送的历史组
            var historyGroups = SqlDb.Fetch<HistoryGroup>(g => g.userId == userId).ToList();
            var defaultResults = new JArray()
                {
                   new JObject(){ { "name", "notSent" },{ "value",0} }
                };
            if (historyGroups.Count < 1)
            {
                // 返回默认值
                try
                {
                    await ResponseSuccessAsync(defaultResults);
                }
                catch (Exception e)
                {
                    ;
                }
                return;
            };

            List<string> historyIds = historyGroups.Select(hg => hg._id).ToList();

            // 查找历史组下面的所有的发件
            var sendItems = SqlDb.Queryable<SendItem>().In(it => it.historyId, historyIds).ToList();               
            if (sendItems.Count() < 1)
            {
                // 返回1
                await ResponseSuccessAsync(defaultResults);
                return;
            }

            // 计算每个邮箱对应的值
            Dictionary<string, int> resultDic = new Dictionary<string, int>();
            var regex = new Regex(@"@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            foreach (var sendItem in sendItems)
            {

                var emailType = regex.Match(sendItem.receiverEmail);
                if (!emailType.Success) continue;

                var typeKey = emailType.Value;
                if (resultDic.ContainsKey(typeKey))
                {
                    resultDic[typeKey] = resultDic[typeKey] + 1;
                }
                else
                {
                    resultDic.Add(typeKey, 1);
                }
            }

            await ResponseSuccessAsync(resultDic.ToList().ConvertAll(item =>
            {
                return new JObject(new JProperty(Fields.name, item.Key), new JProperty(Fields.value, item.Value));
            }));
        }
    }
}
