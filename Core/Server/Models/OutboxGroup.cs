using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uamazing.Utils.Database.Attributes;

namespace Uamazing.UZonEmail.Server.Models
{
    /// <summary>
    /// 发件箱
    /// </summary>
    [CollectionName("EmailGroup")]
    public class OutboxGroup:EmailBoxGroup
    {
        public OutboxGroup()
        {
            GroupType = GroupType.OutboxGroup;
        }
    }
}
