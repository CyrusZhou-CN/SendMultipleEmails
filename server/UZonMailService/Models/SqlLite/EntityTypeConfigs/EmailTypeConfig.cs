using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection.Emit;
using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.EmailSending;

namespace UZonMailService.Models.SqlLite.EntityTypeConfigs
{
    // 指定 outbox 外键
    internal class EmailTypeConfig : IEntityTypeConfig
    {
        public void Configure(ModelBuilder builder)
        {
            // 取消级联删除
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientNoAction;
            }

            // 对 SendingTask 进行配置
            builder.Entity<SendingGroup>()
                .Property(x => x.Data)
                .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JArray.Parse(v).ToList());
        }
    }
}
