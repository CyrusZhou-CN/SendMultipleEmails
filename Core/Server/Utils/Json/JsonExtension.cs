using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uamazing.SME.Server.Utils.Json
{
    public static class JsonExtension
    {
        public static T ValueOrDefault<T>(this IEnumerable<JToken> jt, T default_)
        {
            if (jt == null) return default_;

            T value = jt.Value<T>();
            if (value == null) return default_;

            return value;
        }
    }
}
