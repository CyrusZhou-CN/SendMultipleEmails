namespace UZonMailService.Utils.ASPNETCore.PagingQuery
{
    /// <summary>
    /// 获取分页数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageDataPick<T>
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public string? SortBy { get; set; }
        /// <summary>
        /// 是否降序
        /// </summary>
        public bool Descending { get; set; } = false;
        /// <summary>
        /// 跳过的数量
        /// </summary>
        public int Skip { get; set; }
        /// <summary>
        /// 返回的数量
        /// 若为0,则返回全部
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// 执行分页数据
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public IQueryable<T> Run(IQueryable<T> values)
        {
            if (!string.IsNullOrEmpty(SortBy))
            {
                values = values.OrderBy(SortBy, Descending);
            }
            if (Skip > 0)
            {
                values = values.Skip(Skip);
            }
            if(Limit > 0)
            {
                values = values.Take(Limit);
            }
            return values;
        }
    }
}
