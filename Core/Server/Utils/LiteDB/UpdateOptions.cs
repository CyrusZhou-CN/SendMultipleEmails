using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.SME.Server.Utils.LiteDB
{
    public class UpdateOptions : List<string>
    {
        public UpdateOptions() { }

        public UpdateOptions(bool isExclude)
        {
            IsExclude = isExclude;
        }

        public UpdateOptions(IList<string> keys,bool isExclude=false):base(keys)
        {
            IsExclude = isExclude;
        }

        /// <summary>
        /// 是否不包含在集合中
        /// true：不在集合中的字段才更新
        /// </summary>
        public bool IsExclude { get; set; }

        /// <summary>
        /// 忽略大小写
        /// </summary>
        public bool IgnoreCase { get; set; } = true;

        public bool Validate(string value)
        {
            if (IgnoreCase)
            {
                var valueTemp = value.ToLower();
                bool isIncluding = this.Any(x => x.ToLower() == valueTemp);
                if (IsExclude) return !isIncluding;
                return isIncluding;
            }
            else
            {
                if (IsExclude) return !Contains(value);
                return Contains(value);
            }            
        }
    }
}
