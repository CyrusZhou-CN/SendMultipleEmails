using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using UZonMailService.Models.SqlLite;
using UZonMailService.Models.SqlLite.Base;

namespace UZonMailService.Utils.Database
{
    public static class EntityFrameworkExtensions
    {
        /// <summary>
        /// 更新匹配到的实体
        /// 该方法效率还需要优化，可能会请求数据库两次，不高效(未测试)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="predicate"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public async static Task<int> UpdateAsync<T>(this IQueryable<T> entities, Expression<Func<T, bool>> predicate, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> update) where T : SqlId
        {
            return await entities.Where(predicate).ExecuteUpdateAsync(update);
        }

        /// <summary>
        /// 通过 Id 更新实体
        /// 里面会调用 SaveChangesAsync 方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="entity"></param>
        /// <param name="modifiedPropertyNames"></param>
        /// <returns></returns>
        public async static Task<TEntity> UpdateById<TEntity>(this SqlContext ctx, TEntity entity, List<string> modifiedPropertyNames) where TEntity : SqlId
        {
            if (entity == null || entity.Id == 0)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var entry = ctx.Attach(entity);
            foreach (var propertyName in modifiedPropertyNames)
            {
                entry.Property(propertyName).IsModified = true;
            }

            await ctx.SaveChangesAsync();
            return entity;
        }
    }
}
