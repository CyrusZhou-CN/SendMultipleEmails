using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Database.Models
{
    public abstract class AutoObjectId
    {

        [SugarColumn(IsPrimaryKey = true)]
        public string _id { get; set; }
        public AutoObjectId()
        {
            _id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 获取过滤字符串
        /// </summary>
        /// <returns></returns>
        public virtual string GetFilterString()
        {
            return _id;
        }

        /// <summary>
        /// 通过字段名称获取值
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public object GetValue(string fieldName)
        {
            var propertyInfo = GetType().GetProperty(fieldName);
            if (propertyInfo == null) return string.Empty;

            return propertyInfo.GetValue(this, null);
        }
    }
}
