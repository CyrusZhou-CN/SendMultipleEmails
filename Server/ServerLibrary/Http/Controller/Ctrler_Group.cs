﻿using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Newtonsoft.Json.Linq;
using ServerLibrary.Database.Definitions;
using ServerLibrary.Database.Extensions;
using ServerLibrary.Database.Models;
using ServerLibrary.Http.Definitions;
using ServerLibrary.SDK.Extension;
using Swan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Http.Controller
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

            var results = SqlDb.Fetch<Group>(g => g.groupType == groupType).ToList();
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
                userId = Token.UserId,
                parentId = parentId,
                name = name,
                groupType = groupType,
                description = description,
            };

            SqlDb.Insert(newGroup);

            await ResponseSuccessAsync(newGroup);
        }

        [Route(HttpVerbs.Delete, "/groups")]
        public async Task DeleteGroup()
        {
            List<string> ids = Body["groupIds"].ToObject<List<string>>();
            SqlDb.DeleteMany<Group>(g => ids.Contains(g._id));
            await ResponseSuccessAsync(ids);
        }

        // 更新group
        [Route(HttpVerbs.Put, "/groups/{id}")]
        public async Task UpdateGroup(string id)
        {
            // 获取所有待更新的key
            List<string> keys = (Body as JObject).Properties().ToList().ConvertAll(p => p.Name);
            Group group = Body.ToObject<Group>();
            var res = SqlDb.Upsert2(g => g._id == id, group, new Database.Definitions.UpdateOptions(keys));
            await ResponseSuccessAsync(res);
        }

        // 新建邮件
        [Route(HttpVerbs.Post, "/groups/{id}/email")]
        public async Task NewEmail(string id)
        {
            // 根据id获取组
            var group = SqlDb.SingleOrDefault<Group>(g => g._id == id);
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
                return;
            }
            // 根据key来进行实例化
            EmailInfo res;
            if (group.groupType == "send")
            {
                var emailInfo = Body.ToObject<SendBox>();
                res = SqlDb.Upsert2(g => g.email == emailInfo.email, emailInfo);
            }
            else
            {
                var emailInfo = Body.ToObject<ReceiveBox>();
                res = SqlDb.Upsert2(g => g.email == emailInfo.email, emailInfo);
            }

            await ResponseSuccessAsync(res);
        }

        // 新建多个邮件
        [Route(HttpVerbs.Post, "/groups/{id}/emails")]
        public async Task NewEmails(string id)
        {
            // 获取所有待更新的key
            var group = SqlDb.SingleOrDefault<Group>(g => g._id == id);
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
                return;
            }
            // 根据key来进行实例化
            if (group.groupType == "send")
            {
                var emailInfos = Body.ToObject<List<SendBox>>();
                emailInfos.ForEach(e => e.groupId = id);
                SqlDb.InsertBulk(emailInfos);
                await ResponseSuccessAsync(emailInfos);
            }
            else
            {
                var emailInfos = Body.ToObject<List<ReceiveBox>>();
                emailInfos.ForEach(e => e.groupId = id);
                SqlDb.InsertBulk(emailInfos);
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
            var group = SqlDb.SingleOrDefault<Group>(g => g._id == id);
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
                return;
            }

            var data = Body.ToObject<PageQuery>();
            var regex = new System.Text.RegularExpressions.Regex(data.filter.filter);
            int count = 0;
            if (group.groupType == "send")
            {
                count = SqlDb.Fetch<SendBox>(e => e.groupId == id).ToList().Where(item => regex.IsMatch(item.GetFilterString())).Count();
            }
            else
            {
                count = SqlDb.Fetch<ReceiveBox>(e => e.groupId == id).ToList().Where(item => regex.IsMatch(item.GetFilterString())).Count(); ;
            }

            await ResponseSuccessAsync(count);
        }

        // 获取多个邮件
        [Route(HttpVerbs.Post, "/groups/{id}/emails/list")]
        public async Task GetEmails(string id)
        {
            var group = SqlDb.SingleOrDefault<Group>(g => g._id == id);
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
                return;
            }

            List<EmailInfo> results = new List<EmailInfo>();
            var data = Body.ToObject<PageQuery>();
            var regex = new System.Text.RegularExpressions.Regex(data.filter.filter);
            if (group.groupType == "send")
            {
                //var emails = SqlDb.Fetch<SendBox>(e => e.groupId == id).ToList()
                //    .Where(item => regex.IsMatch(item.GetFilterString()));
                var emails = SqlDb.Fetch<SendBox>(e => e.groupId == id);
                if (data.pagination.descending)
                {
                    emails = emails.OrderBy($"{data.pagination.sortBy} desc");
                    //emails = emails.OrderByDescending(item => item.GetValue(data.pagination.sortBy));
                }
                else
                {
                    //emails = emails.OrderBy(item => item.GetValue(data.pagination.sortBy));
                    emails = emails.OrderBy(data.pagination.sortBy);
                }

                emails = emails.Skip(data.pagination.skip).Take(data.pagination.limit);

                results.AddRange(emails.ToList());
            }
            else
            {
                //var emails = SqlDb.Fetch<ReceiveBox>(e => e.groupId == id).ToList()
                //    .Where(item => regex.IsMatch(item.GetFilterString()));
                var emails = SqlDb.Fetch<ReceiveBox>(e => e.groupId == id);
                if (data.pagination.descending)
                {
                    // emails = emails.OrderByDescending(item => item.GetValue(data.pagination.sortBy));
                    emails = emails.OrderBy($"{data.pagination.sortBy} desc");
                }
                else
                {
                    //emails = emails.OrderBy(item => item.GetValue(data.pagination.sortBy));
                    emails = emails.OrderBy(data.pagination.sortBy);
                }

                emails = emails.Skip(data.pagination.skip).Take(data.pagination.limit);

                results.AddRange(emails.ToList());
            }

            await ResponseSuccessAsync(results);
        }

        // 删除单个邮箱
        [Route(HttpVerbs.Delete, "/emails/{id}")]
        public async Task DeleteEmail(string id)
        {
            // 获取所有待更新的key
            SqlDb.Delete<SendBox>(id);
            SqlDb.Delete<ReceiveBox>(id);

            await ResponseSuccessAsync("success");
        }

        // 删除多个邮箱
        [Route(HttpVerbs.Delete, "/groups/{id}/emails")]
        public async Task DeleteEmails(string id)
        {
            var group = SqlDb.SingleOrDefault<Group>(m => m._id == id);
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
                return;
            }

            // 删除组id对应的所有邮箱
            SqlDb.DeleteMany<SendBox>(m => m.groupId == id);
            SqlDb.DeleteMany<ReceiveBox>(m => m.groupId == id);

            await ResponseSuccessAsync("success");
        }

        // 修改邮箱
        [Route(HttpVerbs.Put, "/emails/{id}")]
        public async Task ModifyEmail(string id)
        {
            // 根据id判断属于发件还是收件
            var sendbox = SqlDb.FirstOrDefault<SendBox>(s => s._id == id);
            if (sendbox != null)
            {
                var updateData1 = Body.ToObject<SendBox>();
                var result1 = SqlDb.Upsert2(e => e._id == id, updateData1, new UpdateOptions(true) { "_id", "groupId", "settings" });
                await ResponseSuccessAsync(result1);
                return;
            }

            // 收件情况
            var receiveBox = SqlDb.FirstOrDefault<ReceiveBox>(r => r._id == id);
            if (receiveBox == null)
            {
                await ResponseErrorAsync($"未找到id:{id}对应的邮箱");
                return;
            }

            var updateData2 = Body.ToObject<ReceiveBox>();
            // 更新
            var result2 = SqlDb.Upsert2(e => e._id == id, updateData2, new UpdateOptions(true) { "_id", "groupId" });
            await ResponseSuccessAsync(result2);
        }

        // 修改发件箱设置
        [Route(HttpVerbs.Put, "/emails/{id}/settings")]
        public async Task UpdateSendEmailSettings(string id)
        {
            // 根据id判断属于发件还是收件
            var sendbox = SqlDb.FirstOrDefault<SendBox>(s => s._id == id);
            if (sendbox != null)
            {
                if (sendbox.settings == null)
                {
                    sendbox.settings = new SendBoxSetting();
                }

                sendbox.settings.UpdateObject(Body as JObject);
                SqlDb.Update(sendbox);
                await ResponseSuccessAsync(sendbox);
                return;
            }

            await ResponseErrorAsync($"未找到发件箱:{id}");
        }
    }
}
