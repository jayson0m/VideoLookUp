using System;
using Microsoft.AspNetCore.Http;
using System.Text;
using Novell.Directory.Ldap;
using QUVideoLinkArchive.Controllers;

namespace QUVideoLinkArchive.Models

/*
    Jay Munro - Created 2016 
    Updated 1/13/16 by JM
	
    Methods
    ---- Login Procedure
    ValidateUser(string userName, string password)  - validate user against Active Directory
    login(login data)                               - Main login method. Calls Validate User.  If user is valid sets a hashed session id.  
                                                        Sets Username as a session id as well - used across all tool utilities. 
    checkSession()                                  - checks if User has the correct hash in session.  If yes user is able continue.  If 
                                                        not user is thrown back to login screen.
    hashIt(string input, string key)                - Simple homebrew hash algorism. 

Error 100 == ldap authentication error 
101 = login or pw blank or null
*/

{
	public class login
	{

		public bool ValidateUser(string userName, string password)
		{
			bool validation;

			using (var cn = new LdapConnection())
			{
				cn.Connect("authenticate.server.net", 389);
				try
				{
					cn.Bind("DOMAIN\\" + userName, password);
					validation = true;
					connectionReturn = "Connected.";
					valid = true;
					return validation;
				}
				catch (Exception e)
				{

					validation = false;
					connectionReturn = "100";
					VideoBackendController.loginerror += connectionReturn;
					valid = false;
					return validation;
				}


			}

		}

		public string Username { get; set; }
		public string Password { get; set; }
		public bool valid { get; set; }
		public string connectionReturn { get; set; }
		public string hashstring { get; set; }
		public string unhashstring { get; set; }
		public string returnmsg { get; set; }
		private readonly IHttpContextAccessor _httpContextAccessor;

		public login(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public login()
		{ }

		//QUToolsUtilities QUToolsUtilities = new QUToolsUtilities();


		public login(login data)
		{

			if (data.Username != null && data.Password != null)
			{
				data.valid = ValidateUser(data.Username, data.Password);
				data.hashstring = hashIt(data.Username, "XXccXXddDDddDddddxXdd");
				
				
				if (data.valid != false)
				{

					QUToolsUtilities.SetSession("QTools", data.hashstring);
					QUToolsUtilities.SetSession("QToolsUser", data.Username);
				} 
			}
			else { data.valid = false;
				VideoBackendController.loginerror +=  "101";
				
			}
		}

		public bool checkSession()
		{
			try
			{
				if (QUToolsUtilities.GetSession("QTools") != hashIt(QUToolsUtilities.GetSession("QTools"), "XXccXXddDDddDddddxXdd"))
				{
					return true;
				}
				else { return false; }
			}
			catch
			{
				return false;
			}
		}
		public string hashIt(string input, string key)
		{
			int str1 = input.Length;
			int str2 = key.Length;

			StringBuilder hash = new StringBuilder();

			input = input.PadRight(30, 'j');
			key = key.PadRight(30, 'j');
			byte[] inputBytes = Encoding.ASCII.GetBytes(input);
			byte[] keyBytes = Encoding.ASCII.GetBytes(key);
			for (int x = 0; x <= 29; x++)
			{
				hash.Append(
						(
						Convert.ToInt16(inputBytes[x])
						^
						Convert.ToInt16(keyBytes[x])
						)
						*
						x + (x++)
					);
			}
			return hash.ToString();
		}
		public string returnUsername()
		{
			try
			{
				return QUToolsUtilities.GetSession("QToolsUser");
			}
			catch
			{
				return "None";
			}
		}
	}
}