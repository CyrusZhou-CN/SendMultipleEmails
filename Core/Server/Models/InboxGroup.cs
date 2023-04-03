using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uamazing.Utils.Database.Attributes;

namespace Uamazing.SME.Server.Models
{
    /// <summary>
    /// 收件箱组
    /// </summary>
    [CollectionName("EmailGroup")]
    public class InboxGroup : EmailBoxGroup
    {
        public InboxGroup()
        {
            GroupType = GroupType.InboxGroup;
        }
    }
}
