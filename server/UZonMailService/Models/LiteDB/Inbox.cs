using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uamazing.Utils.Database.Attributes;

namespace UZonMailService.Models.LiteDB
{
    /// <summary>
    /// 收件箱
    /// </summary>
    [CollectionName("EmailBox")]
    public class Inbox : EmailBox
    {
    }
}
