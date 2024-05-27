using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Database
{
    public class ConfigHelper
    {
        public static DatabaseConfig GetDatabaseConfig()
        {
            var settings = (NameValueCollection)ConfigurationManager.GetSection("appSettings");

            return new DatabaseConfig
            {
                DatabaseType = settings["DatabaseType"],
                ConnectionString = settings["ConnectionString"]
            };
        }
    }
}
