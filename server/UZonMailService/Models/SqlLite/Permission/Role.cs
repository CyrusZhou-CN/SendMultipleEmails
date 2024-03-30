using UZonMailService.Models.LiteDB;
using UZonMailService.Models.SqlLite.Base;

namespace UZonMailService.Models.SqlLite.Permission
{
    /// <summary>
    /// 权限角色
    /// </summary>
    public class Role : SqlId
    {
        /// <summary>
        /// 角色名
        /// </summary>
        public string Name { get; set; }

        #region 导航属性
        /// <summary>
        /// 权限码
        /// </summary>
        public List<PermissionCode> Codes { get; set; }
        public List<User> Users { get; set; }
        #endregion
    }
}
