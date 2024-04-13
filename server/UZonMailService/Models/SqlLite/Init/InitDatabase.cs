﻿using Microsoft.Extensions.Hosting;
using UZonMailService.Config;
using UZonMailService.Config.SubConfigs;
using Uamazing.Utils.Extensions;

namespace UZonMailService.Models.SqlLite.Init
{
    /// <summary>
    /// 初始化数据库
    /// </summary>
    public class InitDatabase
    {
        private readonly SqlContext _db;
        private IWebHostEnvironment _hostEnvironment;
        private AppConfig _appConfig;

        public InitDatabase(IWebHostEnvironment hostEnvironment, SqlContext sqlContext, AppConfig config)
        {
            _db = sqlContext;
            _hostEnvironment = hostEnvironment;
            _appConfig = config;
        }

        /// <summary>
        /// 开始执行初始化
        /// </summary>
        public void Init()
        {
            InitUser();
            InitPermission();

            _db.SaveChanges();
        }

        private void InitUser()
        {
            // 设置超管,防止其它账号权限被撤销
            var adminUser = _db.Users.FirstOrDefault(x => x.IsSuperAdmin);
            if (adminUser == null)
            {
                adminUser = new UserInfos.User
                {
                    UserName = "admin",
                    // 密码是进行了 Sha256 二次加密的
                    Password = "admin1234".Sha256(1),
                    IsSuperAdmin = true
                };

                // 从配置中读取超管的信息
                if(_appConfig?.User?.AdminUser != null)
                {
                    var adminUserConfig = _appConfig.User.AdminUser;
                    if(!string.IsNullOrEmpty(adminUserConfig.UserId))adminUser.UserId = adminUserConfig.UserId;
                    if (!string.IsNullOrEmpty(adminUserConfig.Password)) adminUser.Password = adminUserConfig.Password.Sha256(1);
                    if (!string.IsNullOrEmpty(adminUserConfig.Avatar)) adminUser.Avatar = adminUserConfig.Avatar;
                }

                _db.Add(adminUser);
            }
        }

        /// <summary>
        /// 初始化用户角色
        /// 1. 添加默认功能码,包含管理员
        /// 2. 生成一个管理员角色和一个普通用户角色
        /// 3. 为角色赋予默认功能码
        /// </summary>
        private void InitPermission()
        {
            // 检查是否已经有数据
            if (_db.Users.Any())
            {
                return;   // 如果已经有数据，直接返回
            }

            // 添加初始数据
            _db.Users.AddRange();
        }
    }
}
