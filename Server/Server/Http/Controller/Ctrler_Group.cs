﻿using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using LiteDB;
using Newtonsoft.Json.Linq;
using Server.Database.Definitions;
using Server.Database.Extensions;
using Server.Database.Models;
using Server.Http.Definitions;
using Server.SDK.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.SME.Server.Http.Controller
{
    public class Ctrler_Group : BaseControllerAsync
    {

        [Route(HttpVerbs.Get, "/group")]
        public async Task GetGroups([QueryField] string groupType)
        {
            if (string.IsNullOrEmpty(groupType))
            {
                await ResponseErrorAsync("请传递组的类型:[send,receive]");
            };

            var results = LiteDb.Fetch<Group>(g => g.GroupType == groupType).ToList();
            await ResponseSuccessAsync(results);
        }

        [Route(HttpVerbs.Post, "/group")]
        public async Task NewGroup()
        {
            var parentId = Body.Value<string>("parentId");
            var name = Body.Value<string>("name");
            var groupType = Body.Value<string>("groupType");
            var description = Body.Value<string>("description");

            var newGroup = new Group()
            {
                UserId = Token.UserId,
                ParentId = parentId,
                Name = name,
                GroupType = groupType,
                Description = description,
            };

            LiteDb.Insert(newGroup);

            await ResponseSuccessAsync(newGroup);
        }

        [Route(HttpVerbs.Delete, "/groups")]
        public async Task DeleteGroup()
        {
            List<string> ids = Body["groupIds"].ToObject<List<string>>();
            LiteDb.DeleteMany<Group>(g => ids.Contains(g.Id));
            await ResponseSuccessAsync(ids);
        }

        // 更新group
        [Route(HttpVerbs.Put, "/groups/{id}")]
        public async Task UpdateGroup(string id)
        {
            // 获取所有待更新的key
            List<string> keys = (Body as JObject).Properties().ToList().ConvertAll(p => p.Name);
            Group group = Body.ToObject<Group>();
            var res = LiteDb.Upsert2(g => g.Id == id, group, new Database.Definitions.UpdateOptions(keys));
            await ResponseSuccessAsync(res);
        }

        // 新建邮件
        [Route(HttpVerbs.Post, "/groups/{id}/email")]
        public async Task NewEmail(string id)
        {
            // 根据id获取组
            var group = LiteDb.SingleOrDefault<Group>(g => g.Id == id);
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
                return;
            }
            // 根据key来进行实例化
            EmailInfo res;
            if (group.GroupType == "send")
            {
                var emailInfo = Body.ToObject<SendBox>();
                res = LiteDb.Upsert2(g => g.Email == emailInfo.Email, emailInfo);
            }
            else
            {
                var emailInfo = Body.ToObject<ReceiveBox>();
                res = LiteDb.Upsert2(g => g.Email == emailInfo.Email, emailInfo);
            }

            await ResponseSuccessAsync(res);
        }

        // 新建多个邮件
        [Route(HttpVerbs.Post, "/groups/{id}/emails")]
        public async Task NewEmails(string id)
        {
            // 获取所有待更新的key
            var group = LiteDb.SingleOrDefault<Group>(g => g.Id == id);
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
                return;
            }
            // 根据key来进行实例化
            if (group.GroupType == "send")
            {
                var emailInfos = Body.ToObject<List<SendBox>>();
                emailInfos.ForEach(e => e.GroupId = id);
                LiteDb.Database.GetCollection<SendBox>().InsertBulk(emailInfos);
                await ResponseSuccessAsync(emailInfos);
            }
            else
            {
                var emailInfos = Body.ToObject<List<ReceiveBox>>();
                emailInfos.ForEach(e => e.GroupId = id);
                LiteDb.Database.GetCollection<ReceiveBox>().InsertBulk(emailInfos);
                await ResponseSuccessAsync(emailInfos);
            }
        }

        /// <summary>
        /// 获取邮件的数量
        /// </summary>
        /// <returns></returns>
        [Route(HttpVerbs.Post, "/groups/{id}/emails/count")]
        public async Task GetEmailsCount(string id)
        {
            var group = LiteDb.SingleOrDefault<Group>(g => g.Id == id);
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
                return;
            }

            var data = Body.ToObject<PageQuery>();

            int count = 0;
            var regex = new System.Text.RegularExpressions.Regex(data.filter.filter);
            if (group.GroupType == "send")
            {
                count = LiteDb.Fetch<SendBox>(e => e.GroupId == id).Where(item => regex.IsMatch(item.GetFilterString())).Count();
            }
            else
            {
                count = LiteDb.Fetch<ReceiveBox>(e => e.GroupId == id).Where(item => regex.IsMatch(item.GetFilterString())).Count();
            }

            await ResponseSuccessAsync(count);
        }

        // 获取多个邮件
        [Route(HttpVerbs.Post, "/groups/{id}/emails/list")]
        public async Task GetEmails(string id)
        {
            var group = LiteDb.SingleOrDefault<Group>(g => g.Id == id);
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
                return;
            }

            List<EmailInfo> results = new List<EmailInfo>();
            var data = Body.ToObject<PageQuery>();
            var regex = new System.Text.RegularExpressions.Regex(data.filter.filter);

            if (group.GroupType == "send")
            {
                var emails = LiteDb.Fetch<SendBox>(e => e.GroupId == id)
                    .Where(item => regex.IsMatch(item.GetFilterString()));
                if (data.pagination.descending)
                {
                    emails = emails.OrderByDescending(item => item.GetValue(data.pagination.sortBy));
                }
                else
                {
                    emails = emails.OrderBy(item => item.GetValue(data.pagination.sortBy));
                }

                emails = emails.Skip(data.pagination.skip).Take(data.pagination.limit);

                results.AddRange(emails);
            }
            else
            {
                var emails = LiteDb.Fetch<ReceiveBox>(e => e.GroupId == id)
                    .Where(item => regex.IsMatch(item.GetFilterString()));
                if (data.pagination.descending)
                {
                    emails = emails.OrderByDescending(item => item.GetValue(data.pagination.sortBy));
                }
                else
                {
                    emails = emails.OrderBy(item => item.GetValue(data.pagination.sortBy));
                }

                emails = emails.Skip(data.pagination.skip).Take(data.pagination.limit);

                results.AddRange(emails);
            }

            await ResponseSuccessAsync(results);
        }

        // 删除单个邮箱
        [Route(HttpVerbs.Delete, "/emails/{id}")]
        public async Task DeleteEmail(string id)
        {
            // 获取所有待更新的key
            LiteDb.Delete<SendBox>(id);
            LiteDb.Delete<ReceiveBox>(id);

            await ResponseSuccessAsync("success");
        }

        // 删除多个邮箱
        [Route(HttpVerbs.Delete, "/groups/{id}/emails")]
        public async Task DeleteEmails(string id)
        {
            var group = LiteDb.SingleOrDefault<Group>($"_id='{id}'");
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
                return;
            }

            // 删除组id对应的所有邮箱
            LiteDb.DeleteMany<SendBox>($"groupId='{id}'");
            LiteDb.DeleteMany<ReceiveBox>($"groupId='{id}'");

            await ResponseSuccessAsync("success");
        }

        // 修改邮箱
        [Route(HttpVerbs.Put, "/emails/{id}")]
        public async Task ModifyEmail(string id)
        {
            // 根据id判断属于发件还是收件
            var sendbox = LiteDb.FirstOrDefault<SendBox>(s => s.Id == id);
            if (sendbox != null)
            {
                var updateData1 = Body.ToObject<SendBox>();
                var result1 = LiteDb.Upsert2(e => e.Id == id, updateData1, new UpdateOptions(true) { "_id", "groupId" });
                await ResponseSuccessAsync(result1);
                return;
            }

            // 收件情况
            var receiveBox = LiteDb.FirstOrDefault<ReceiveBox>(r => r.Id == id);
            if (receiveBox == null)
            {
                await ResponseErrorAsync($"未找到id:{id}对应的邮箱");
                return;
            }

            var updateData2 = Body.ToObject<ReceiveBox>();
            // 更新
            var result2 = LiteDb.Upsert2(e => e.Id == id, updateData2, new UpdateOptions(true) { "_id", "groupId" });
            ResponseSuccessAsync(result2);
        }

        // 修改发件箱设置
        [Route(HttpVerbs.Put, "/emails/{id}/settings")]
        public async Task UpdateSendEmailSettings(string id)
        {
            // 根据id判断属于发件还是收件
            var sendbox = LiteDb.FirstOrDefault<SendBox>(s => s.Id == id);
            if (sendbox != null)
            {
                if (sendbox.Settings == null)
                {
                    sendbox.Settings = new SendBoxSetting();
                }

                sendbox.Settings.UpdateObject(Body as JObject);
                LiteDb.Update(sendbox);
                await ResponseSuccessAsync(sendbox);
                return;
            }

            await ResponseErrorAsync($"未找到发件箱:{id}");
        }
    }
}
