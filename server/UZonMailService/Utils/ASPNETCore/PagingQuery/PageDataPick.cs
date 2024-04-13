﻿using Uamazing.Utils.Web.RequestModel;

namespace UZonMailService.Utils.ASPNETCore.PagingQuery
{
    /// <summary>
    /// 获取分页数据
    /// </summary>
    public class PageDataPick : PaginationModel
    {
        public PageDataPick()
        {
            // 修改默认值
            SortBy = "id";
        }

        /// <summary>
        /// 执行分页数据
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public IQueryable<T> Run<T>(IQueryable<T> values)
        {
            if (!string.IsNullOrEmpty(SortBy))
            {
                values = values.OrderBy(SortBy, Descending);
            }
            if (Skip > 0)
            {
                values = values.Skip(Skip);
            }
            if (Limit > 0)
            {
                values = values.Take(Limit);
            }
            return values;
        }
    }
}
