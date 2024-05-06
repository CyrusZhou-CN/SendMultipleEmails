using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.EmailSending;
using UZonMailService.Models.SqlLite.EntityConfigs.Attributes;
using UZonMailService.Models.SqlLite.EntityConfigs.Converters;

namespace UZonMailService.Models.SqlLite.EntityConfigs
{
    // 指定 outbox 外键
    internal class EntityTypeConfig : IEntityTypeConfig
    {
        public void Configure(ModelBuilder builder)
        {
            // 取消级联删除
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientNoAction;
            }

            // 对所有的实体配置 json 转换
            ConfigureJsonMap(builder);
        }

        /// <summary>
        /// 配置 json 映射
        /// 根据 JsonMapAttribute 判断是否需要设置 json 映射
        /// 若有，则设置 json 映射
        /// </summary>
        /// <param name="builder"></param>
        private static void ConfigureJsonMap(ModelBuilder modelBuilder)
        {
            List<Type> ignoreTypes =
            [
                typeof(JToken),
                typeof(Dictionary<string, object>),
            ];
            // 获取所有的实体
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                // 忽略一些类型
                if (ignoreTypes.Contains(clrType)) continue;
                try
                {
                    var entityTypeBuilder = modelBuilder.Entity(clrType);
                    foreach (var propertyInfo in clrType.GetProperties())
                    {
                        if (propertyInfo.GetCustomAttribute<JsonMapAttribute>() != null)
                        {
                            entityTypeBuilder.Property(propertyInfo.Name).HasConversion(new JsonValueConverter(propertyInfo.PropertyType));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ConfigureJsonMap 函数报错：");
                    Console.WriteLine(e);
                }
            }
        }
    }
}
