using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using App.Web;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
//using System.DirectoryServices; 

namespace LinkArchiveToolFrontEnd.Models
{
    public class QuToolsUtilities
    {
        public QuToolsUtilities()
        {
        }

        public static void SetSession(string name, string value)
        {

            AppHttpContext.Current.Session.SetString(name, value);
        }

        public static string GetSession(string name)
        {
            return AppHttpContext.Current.Session.GetString(name);

        }
        public static string QueryString(string value) // request.QueryString
        {
            return AppHttpContext.Current.Request.Query[value];
        }


        public static Microsoft.Extensions.Configuration.IConfigurationRoot Configuration { get; set; }
        public static string GetConnectionString(string str)
        {

            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            return Configuration.GetConnectionString(str);

        }
    }
}


/// <summary>
/// From stack.  httpcontext home brew
/// </summary>
/// 

namespace App.Web
{

    public static class AppHttpContext
    {
        static IServiceProvider _services = null;

        /// <summary>
        /// Provides static access to the framework's services provider
        /// </summary>
        public static IServiceProvider Services
        {
            get { return _services; }
            set
            {
                if (_services != null)
                {
                    throw new Exception("Can't set once a value has already been set.");
                }
                _services = value;
            }
        }

        /// <summary>
        /// Provides static access to the current HttpContext
        /// </summary>
        public static HttpContext Current
        {
            get
            {
                IHttpContextAccessor httpContextAccessor = _services.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                return httpContextAccessor?.HttpContext;
            }
        }

    }
}
