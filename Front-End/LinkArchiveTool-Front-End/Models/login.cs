using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LinkArchiveToolFrontEnd.Models;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;
using Novell.Directory.Ldap;
using System.Security.Cryptography;

namespace LinkArchiveToolFrontEnd.Models
{
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
*/
	public class Login
	{
		//TODO: REMOVE ME
		public string Jp203()
		{
			int[] my = { 164, 101, 157, 122, 184, 178, 199, 177, 146, 188, 151, 179, 162, 98 };
			string str2 = Environment.MachineName;
			string str3 = "";

			str2 = str2.PadRight(my.Length, 'A');
			for (int i = 0; i < str2.Length; i++)
			{
				int y = my[i];
				int z = Convert.ToByte(Convert.ToChar(str2.Substring(i, 1)));
				int x = (y - z);
				str3 += (Convert.ToChar(x));
			}

			return str3;
		}

		public bool ValidateUser(string userName, string password)
		{
			bool validation;


			using (var cn = new LdapConnection())
			{
				cn.Connect("authenticate.server.net", 389);
				try
				{
					cn.Bind("SERVER\\" + userName, password);
					validation = true;
					ConnectionReturn = "Connected.";
					Valid = true;
					return validation;
				}
				catch (Exception e)
				{

					validation = false;
					ConnectionReturn = "Invalid Login... Please try again.";
					Valid = false;
					return validation;
				}


			}

		}

		public string Username { get; set; }
		public string Password { get; set; }
		public bool Valid { get; set; }
		public string ConnectionReturn { get; set; }
		public string Hashstring { get; set; }
		public string Unhashstring { get; set; }

		private readonly IHttpContextAccessor _httpContextAccessor;

		public Login(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public Login()
		{ }

		//QUToolsUtilities QUToolsUtilities = new QUToolsUtilities();


		public Login(Login data)
		{

			if (data.Username != null && data.Password != null)
			{
				data.Valid = ValidateUser(data.Username, data.Password);
				data.Hashstring = HashIt(data.Username, "xx44aaAAddDDffDDeeEEe");
				if (data.Valid != false)
				{

					QuToolsUtilities.SetSession("QTools", data.Hashstring);
					QuToolsUtilities.SetSession("QToolsUser", data.Username);

				}
			}
			else { data.Valid = false; }
		}

		public bool CheckSession()
		{
			try
			{
				if (QuToolsUtilities.GetSession("QTools") != HashIt(QuToolsUtilities.GetSession("QTools"), "xx44aaAAddDDffDDeeEEe"))

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
		public string HashIt(string input, string key)
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
		public string ReturnUsername()
		{
			try
			{
				return QuToolsUtilities.GetSession("QToolsUser");
			}
			catch
			{
				return "None";
			}
		}
	}
}