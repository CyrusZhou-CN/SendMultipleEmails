using EmbedIO;
using EmbedIO.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Config;
using Server.Database;
using Server.Http.Extensions;
using Server.Http.Headers;
using SqlSugar;
using StyletIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Controller
{
    public abstract class BaseController : WebApiController,IDisposable
    {
        protected IContainer IoC { get; private set; }
        protected ISqlSugarClient SqlDb { get; private set; }
        protected string FileStorage => "";

        protected JwtToken Token { get; private set; }

        private JToken _body;
        protected JToken Body
        {
            get
            {
                if (_body == null) _body = GetBody();
                return _body;
            }
        }

        /// <summary>
        /// 获取控制数据库操作类
        /// </summary>
        protected override void OnBeforeHandler()
        {
            IoC = HttpContext.GetIoCScope();

            var databaseConfig=  IoC.Get<DatabaseConfig>();
            SqlDb = new SqlSugarDatabaseManager(databaseConfig).Db;

            UserConfig userConfig = IoC.Get<UserConfig>();
            string token = HttpContext.Request.Headers["X-Token"];
            Token = new JwtToken(userConfig.TokenSecret, token);

            base.OnBeforeHandler();
        }

        /// <summary>
        /// 发送成功的回应
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected void ResponseSuccess(object data)
        {
            var jobj = new HttpResponse()
            {
                data = data,
            };

            SendJObject(jobj);
        }

        /// <summary>
        /// 发送失败回应
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        protected void ResponseError(string errorMsg)
        {
            var jobj = new HttpResponse()
            {
                code = 204,
                message = errorMsg,
            };
            SendJObject(jobj);
        }

        private JToken GetBody()
        {
            Task<string> task = HttpContext.GetRequestBodyAsStringAsync();
            string json = task.Result;
            if (string.IsNullOrEmpty(json)) return new JObject();

            return JToken.Parse(json);
        }

        private void SendJObject(object obj)
        {
            Response.ContentType = MimeType.Json;
            using (var writer = HttpContext.OpenResponseText(Encoding.UTF8, true))
            {
                writer.Write(JsonConvert.SerializeObject(obj));
            }
        }

        public void Dispose()
        {
            this.SqlDb?.Dispose();
        }
    }

    class HttpResponse
    {
        public int code { get; set; } = 200;
        public string message { get; set; }
        public object data { get; set; }
    }
}
