using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using ServerLibrary.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ServerLibrary.Http.Controller
{
    public class Home : BaseController
    {
        [Route(HttpVerbs.Get, "/home")]
        public string HomeIndex()
        {
            return "欢迎使用C#-JS系统";
        }
    }
}
