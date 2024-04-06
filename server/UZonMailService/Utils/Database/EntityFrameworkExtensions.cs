using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UZonMailService.Models.SqlLite.Base;

namespace UZonMailService.Utils.Database
{
    public static class EntityFrameworkExtensions
    {
        /// <summary>
        /// 更新一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="predicate"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        async public static Task<T?> UpdateOneAscyn<T>(this IQueryable<T> entities, Expression<Func<T, bool>> predicate, Action<T> action) where T : SqlId
        {
            var entity = await entities.FirstOrDefaultAsync(predicate);
            if (entity == null) return null;

            action?.Invoke(entity);
            return entity;
        }
    }
}
