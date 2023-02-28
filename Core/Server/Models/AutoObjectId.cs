using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.SME.Server.Models
{
    public abstract class AutoObjectId
    {
        [BsonId,BsonField("_id")]
        public string Id { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        public AutoObjectId()
        {
            // 必须要用字符串当 id,方便格式化给前端使用
            Id = ObjectId.NewObjectId().ToString();
        }

        /// <summary>
        /// 获取过滤字符串
        /// </summary>
        /// <returns></returns>
        public virtual string GetFilterString()
        {
            return Id;
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
