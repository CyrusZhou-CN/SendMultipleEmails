using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Reflection.Metadata;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Models.SqlLite.EntityTypeConfigs;
using UZonMailService.Models.SqlLite.Templates;

namespace UZonMailService.Models.SqlLite
{
    /// <summary>
    /// Sql 上下文
    /// </summary>
    public class SqlContext : DbContext
    {        
        #region 初始化
        /// <summary>
        /// 配置数据库
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new EmailTypeConfig().Configure(modelBuilder);
        }
        #endregion

        #region 构造函数
        private readonly string _dbPath;
        public SqlContext()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _dbPath = Path.Join(path, "UZonMail\\uzon-mail.db");
            Directory.CreateDirectory(Path.GetDirectoryName(_dbPath));

            Database.EnsureCreated();
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

        public DbSet<Files.FileBucket> FileBuckets { get; set; }
        public DbSet<Files.FileObject> FileObjects { get; set; }
        public DbSet<Files.FileUsage> FileUsages { get; set; }

        public DbSet<EmailGroup> EmailGroups { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<Inbox> Inboxes { get; set; }
        /// <summary>
        /// 可以通过 Inbox 进行查找
        /// </summary>
        public DbSet<Outbox> Outboxes { get; set; } 

        //public DbSet<SendingGroup> SendingTasks { get; set; }
        //public DbSet<SendingItem> SendingItems { get; set; }
        #endregion

        #region 通用方法
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<T> RunTransaction<T>(Func<SqlContext, Task<T>> func)
        {
            using var transaction = await Database.BeginTransactionAsync();
            try
            {
                // 执行一些数据库操作
                var result = await func(this);
                // 如果所有操作都成功，那么提交事务
                await transaction.CommitAsync();
                return result;
            }
            catch (Exception)
            {
                // 如果有任何操作失败，那么回滚事务
                await transaction.RollbackAsync();

                // 向外抛出异常
                throw;
            }
        }
        #endregion
    }
}
