using Server.Database.Definitions;
using Server.Database.Models;
using Server.Http.Definitions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server.Database.Extensions
{
    internal static class Ex_SqlSugarRepository
    {
        public static int Count<T>(this ISqlSugarClient db, Expression<Func<T, bool>> filter)
        {
            return db.Queryable<T>().Count(filter);
        }
        public static T SingleById<T>(this ISqlSugarClient db, string id)
        {
            return db.Queryable<T>().InSingle(id);
        }
        public static T FindById<T>(this ISqlSugarClient db, string id)
        {
            return db.Queryable<T>().InSingle(id);
        }
        public static ISugarQueryable<T> Fetch<T>(this ISqlSugarClient db, Expression<Func<T, bool>> filter)
        {
            return db.Queryable<T>().Where(filter);
        }
        public static T SingleOrDefault<T>(this ISqlSugarClient db, Expression<Func<T, bool>> filter)
        {
            return db.Queryable<T>().First(filter);
        }
        public static T FirstOrDefault<T>(this ISqlSugarClient db, Expression<Func<T, bool>> filter)
        {
            return db.Queryable<T>().First(filter);
        }
        public static T Insert<T>(this ISqlSugarClient db, T data) where T : class, new()
        {
            return db.Insertable(data).ExecuteReturnEntity();
        }
        public static bool InsertBulk<T>(this ISqlSugarClient db, List<T> data) where T : class, new()
        {
            return db.Fastest<T>().BulkCopy(data) > 0;
        }
        public static bool Delete<T>(this ISqlSugarClient db, string id) where T : class, new()
        {
            return db.Deleteable<T>(id).ExecuteCommand() > 0;
        }
        public static bool DeleteMany<T>(this ISqlSugarClient db, Expression<Func<T, bool>> filter) where T : class, new()
        {
            return db.Deleteable<T>().Where(filter).ExecuteCommand() > 0;
        }
        public static bool Update<T>(this ISqlSugarClient db, T data) where T : class, new()
        {
            return db.Updateable<T>(data).ExecuteCommand() > 0;
        }
        /// <summary>
        /// 插入或者根据_id更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool Upsert<T>(this ISqlSugarClient db, T data) where T : AutoObjectId, new()
        {
            if (db.FindById<T>(data._id) == null)
            {
                db.Insert<T>(data);
            }
            return db.Update<T>(data);
        }
        public static T Upsert2<T>(this ISqlSugarClient db, Expression<Func<T, bool>> filter, T data, UpdateOptions options = null) where T : class, new()
        {
            T exist = db.Queryable<T>().First(filter);
            if (exist == null)
            {
                db.Insertable(data).ExecuteCommand();
                return data;
            }
            if (options == null)
            {
                db.Updateable(data).Where(filter).ExecuteCommand();
            }
            else if (options.IsExclude)
            {
                db.Updateable(data).IgnoreColumns(options.ToArray()).Where(filter).ExecuteCommand();
            }
            else
            {
                db.Updateable(data).UpdateColumns(options.ToArray()).Where(filter).ExecuteCommand();
            }
            return db.Queryable<T>().First(filter);
        }

        public static ISugarQueryable<T> FindAll<T>(this ISugarQueryable<T> source, Expression<Func<T, bool>> filter)
        {
            return source.Where(filter);
        }
        public static int GetPageDatasCount<T>(this ISugarQueryable<T> source, Filter filter) where T : AutoObjectId
        {
            var regex = new Regex(filter.filter);
            var results = source.ToList().Where(h => regex.IsMatch(h.GetFilterString()));
            return results.Count();
        }
        public static IEnumerable<T> GetPageDatas<T>(this ISugarQueryable<T> source, Filter filter, Pagination pagination) where T : AutoObjectId
        {
            var regex = new Regex(filter.filter);

            // 进行筛选
            var results = source.ToList().Where(h => regex.IsMatch(h.GetFilterString()));

            if (pagination.descending)
            {
                results = results.OrderByDescending(item => item.GetValue(pagination.sortBy));
            }
            else
            {
                results = results.OrderBy(item => item.GetValue(pagination.sortBy));
            }

            return results.Skip(pagination.skip).Take(pagination.limit).ToList();
        }

    }
}
