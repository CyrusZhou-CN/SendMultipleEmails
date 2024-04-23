﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uamazing.ConfValidatation.Core.Entrance;
using Uamazing.ConfValidatation.Core.Validators;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.Templates;
using UZonMailService.Services.Settings;
using UZonMailService.Utils.ASPNETCore.PagingQuery;
using UZonMailService.Utils.Database;
using UZonMailService.Utils.DotNETCore.Exceptions;

namespace UZonMailService.Controllers.Emails
{
    /// <summary>
    /// 邮箱模板
    /// </summary>
    public class EmailTemplateController(SqlContext db, TokenService tokenService) : ControllerBaseV1
    {
        /// <summary>
        /// 通过 id 获取邮件模板
        /// </summary>
        /// <param name="emailTemplateId"></param>
        /// <returns></returns>
        [HttpGet("{emailTemplateId:int}")]
        public async Task<ResponseResult<EmailTemplate?>> GetEmailTemplateById(int emailTemplateId)
        {
            int userId = tokenService.GetIntUserId();
            var result = await db.EmailTemplates.FirstOrDefaultAsync(x => x.Id == emailTemplateId && x.UserId == userId);
            return result.ToSmartResponse();
        }

        /// <summary>
        /// 新增或修改邮件模板
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult<EmailTemplate>> Upsert([FromBody] EmailTemplate entity)
        {
            // 添加当前用户名
            entity.UserId = tokenService.GetIntUserId();

            // 数据验证
            entity.Validate(new VdObj()
            {
                { ()=>entity.Name,new IsString("模板名不能为空")},
                { ()=>entity.Content,new IsString("模板内容不能为空") }
            }, ValidateOption.ThrowError);

            // 判断是否存在同名的模板
            var existOne = await db.EmailTemplates.FirstOrDefaultAsync(x => x.Name == entity.Name && x.UserId == entity.UserId);
            // 如果有 Id,则说明是修改
            if (entity.Id > 0)
            {
                if (existOne != null && existOne.Id != entity.Id)
                    throw new KnownException("已存在同名的模板");
                await db.UpdateById(entity, [nameof(EmailTemplate.Name), nameof(EmailTemplate.Content)]);
            }
            else
            {
                if (existOne != null)
                    throw new KnownException("已存在同名的模板");

                db.Add(entity);
                await db.SaveChangesAsync();
                // 更新缩略图
                entity.Thumbnail = $"public/{entity.UserId}/template-thumbnails/{entity.Id}.png";
                await db.SaveChangesAsync();
            }

            return entity.ToSuccessResponse();
        }

        /// <summary>
        /// 删除邮件模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<ResponseResult<bool>> Delete(int id)
        {
            // 通过条件删除
            int userId = tokenService.GetIntUserId();
            var email = await db.EmailTemplates.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId) ?? throw new KnownException("模板不存在");
            db.Remove(email);
            await db.SaveChangesAsync();
            return true.ToSuccessResponse();
        }

        /// <summary>
        /// 获取邮件模板数量
        /// </summary>
        /// <returns></returns>
        [HttpGet("filtered-count")]
        public async Task<ResponseResult<int>> GetEmailTemplatesCount(string filter)
        {
            int userId = tokenService.GetIntUserId();
            var dbSet = db.EmailTemplates.Where(x => x.UserId == userId);
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.Name.Contains(filter) || x.Content.Contains(filter));
            }
            var count = await dbSet.CountAsync();
            return count.ToSuccessResponse();
        }

        /// <summary>
        /// 获取邮件模板数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpPost("filtered-data")]
        public async Task<ResponseResult<List<EmailTemplate>>> GetEmailTemplatesData(string filter, Pagination pagination)
        {
            int userId = tokenService.GetIntUserId();
            var dbSet = db.EmailTemplates.Where(x => x.UserId == userId);
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.Name.Contains(filter) || x.Content.Contains(filter));
            }

            var results = await dbSet.Page(pagination).ToListAsync();
            return results.ToSuccessResponse();
        }
    }
}
