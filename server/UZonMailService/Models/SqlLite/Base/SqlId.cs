using System.ComponentModel.DataAnnotations;

namespace UZonMailService.Models.SqlLite.Base
{
    /// <summary>
    /// 所有数据库的基类
    /// </summary>
    public class SqlId
    {
        /// <summary>
        /// Id 值
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
