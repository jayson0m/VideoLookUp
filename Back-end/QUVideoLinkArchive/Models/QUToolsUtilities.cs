using System;
using Microsoft.AspNetCore.Http;
using App.Web;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;


namespace QUVideoLinkArchive.Models
{
	public class QUToolsUtilities
	{
		public QUToolsUtilities()
		{ }

		public static void SetSession(string Name, string Value)
		{

			AppHttpContext.Current.Session.SetString(Name, Value);
		}

		public static string GetSession(string Name)
		{
			return AppHttpContext.Current.Session.GetString(Name);

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

		public static string HtmlEncode(string s)
		{
			string b = WebUtility.HtmlEncode(s);
			/*
				StringBuilder b = new StringBuilder(s.Length * 3 / 2);
				foreach (char c in s)
					// characters above ~ are converted to a hexadecimal entity
					if (c > '~') b.AppendFormat("&#x{0:x};", (long)c);
					else switch (c)
						{
							case '"': b.Append("&quot;"); break; // double quote
							case '&': b.Append("&amp;"); break;
							case '<': b.Append("&lt;"); break;
							case '>': b.Append("&gt;"); break;
							default: b.Append(c); break;
						}*/
			return b.ToString(); 
		}

		public static string HtmlDecode(string s)
		{
			string b = WebUtility.HtmlDecode(s); 
			/* s.Replace("&quot;", "\"");
			s.Replace("&amp;", "&");
			s.Replace("&lt;", "<");
			s.Replace("&gt;", ">"); */

			return b.ToString();
		}
	}
}


namespace App.Web
{

	public static class AppHttpContext
	{
		static IServiceProvider services = null;

		/// <summary>
		/// Provides static access to the framework's services provider
		/// </summary>
		public static IServiceProvider Services
		{
			get { return services; }
			set
			{
				if (services != null)
				{
					throw new Exception("Can't set once a value has already been set.");
				}
				services = value;
			}
		}

		/// <summary>
		/// Provides static access to the current HttpContext
		/// </summary>
		public static HttpContext Current
		{
			get
			{
				IHttpContextAccessor httpContextAccessor = services.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
				return httpContextAccessor?.HttpContext;
			}
		}

	}
}
