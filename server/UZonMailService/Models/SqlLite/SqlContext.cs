using Microsoft.EntityFrameworkCore;

namespace UZonMailService.Models.SqlLite
{
    /// <summary>
    /// Sql 上下文
    /// </summary>
    public class SqlContext : DbContext
    {
        #region 构造函数
        private readonly string _dbPath;
        public SqlContext()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _dbPath = Path.Join(path, "UZonMail/data.db");
        }
        /// <summary>
        /// The following configures EF to create a Sqlite database file in the
        /// special "local" folder for your platform.
        /// </summary>
        /// <param name="options"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={_dbPath}");
        #endregion

        #region 数据表定义
        public DbSet<UserInfos.User> Users { get; set; }
        public DbSet<Permission.PermissionCode> PermissionCodes { get; set; }
        public DbSet<Permission.Role> Roles { get; set; }
        public DbSet<Permission.RolePermissionCode> RolePermissionCodes { get; set; }
        public DbSet<Permission.UserRole> UserRoles { get; set; }
        #endregion
    }
}
