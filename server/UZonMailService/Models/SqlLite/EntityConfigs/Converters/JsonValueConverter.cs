using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace UZonMailService.Models.SqlLite.EntityConfigs.Converters
{
    /// <summary>
    /// 将对象转换成 json 保存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonValueConverter : ValueConverter
    {
        /// <summary>
        /// Json 转换器
        /// </summary>
        /// <param name="modelType"></param>
        public JsonValueConverter(Type modelType)
            : base(() => "", () => "")
        {
            ModelClrType = modelType;
            ProviderClrType = typeof(string);

            // 生成转换器
            ConvertToProvider = value=> JsonConvert.SerializeObject(value);
            ConvertFromProvider = value => JsonConvert.DeserializeObject(value.ToString(), modelType);
        }

        public override Func<object?, object?> ConvertToProvider { get; }

        public override Func<object?, object?> ConvertFromProvider { get; }

        public override Type ModelClrType { get; }

        public override Type ProviderClrType { get; }
    }
}
