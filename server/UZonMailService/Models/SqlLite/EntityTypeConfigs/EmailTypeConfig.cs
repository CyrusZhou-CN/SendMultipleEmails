using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.Emails;

namespace UZonMailService.Models.SqlLite.EntityTypeConfigs
{
    // 指定 outbox 外键
    internal class EmailTypeConfig : IEntityTypeConfig
    {
        public void Configure(ModelBuilder builder)
        {
            
        }
    }
}
