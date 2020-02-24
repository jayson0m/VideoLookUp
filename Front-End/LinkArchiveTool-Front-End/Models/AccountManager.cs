using System;
using System.Collections.Generic;
//using System.Web;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using LinkArchiveToolFrontEnd.Models;
using Microsoft.Extensions.DependencyInjection;
using LinkArchiveToolFrontEnd.ViewModel;

namespace LinkArchiveToolFrontEnd.Models
{
    public class AccountManager
    {

		/* ############################################
           ##           SQL Table set up:            ##
           ##   QUID, toolAdmin, userlevel, QU_User  ##
           ############################################         
        */



		public IConfiguration ConfigurationString;


		public AccountManager(IConfiguration config)
		{//public void ConfigureServices(IServiceCollection services)
			this.ConfigurationString = config;
		}
		public AccountManager()
		{ }

		//QUToolsUtilities QUHelper = new QUToolsUtilities();



		public static bool IsDigitsOnly(string str)
		{
			foreach (char c in str)
			{
				if (c < '0' || c > '9')
					return false;
			}

			return true;
		}
		public AccountManagerViewModel AddUser(AccountManagerViewModel user)
		{
			//Add the user to DB
			AccountManagerViewModel returnMsg = user;
			string username = user.Username.ToString();
			username = username.ToLower();
			string userTable = user.UserTable.ToString();
			string quId = user.QuId.ToString();
			string userlevel = "Z";

			if (user.Userlevel != null)
			{
				userlevel = user.Userlevel.ToString();
			}




			string adminUser;

			if (user.Admin == "Yes")
			{
				adminUser = "Y";
			}
			else
			{
				adminUser = "N";
			}


			// Check to see if username is in datatel 
			try
			{

				using (SqlConnection connection = new SqlConnection(QuToolsUtilities.GetConnectionString("datatel")))      
				{
					connection.Open();


					string sqlCmd = "select * from person where NTLoginID = @un";
					SqlCommand command = new SqlCommand(sqlCmd, connection);
					command.Parameters.AddWithValue("@un", username);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (!reader.HasRows)
						{
							returnMsg.ReturnMsg = "Invalid User";
							return returnMsg;
						}
					}
					connection.Close();
				}
			}
			catch (Exception e)
			{
				returnMsg.ReturnMsg = e.ToString();
				return returnMsg;
			}

			// Check if user is already in database
			try
			{
				using (SqlConnection connection = new SqlConnection(QuToolsUtilities.GetConnectionString("internet")))
				{
					connection.Open();

					string sqlCmd = "select * from {0} where QU_User = @un";
					sqlCmd = String.Format(sqlCmd, userTable);
					SqlCommand command = new SqlCommand(sqlCmd, connection);
					command.Parameters.AddWithValue("@un", username);


					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{

							if (reader["QU_User"].ToString() == user.Username)
							{
								connection.Close();
								returnMsg.ReturnMsg = "User already Exists.";
								return returnMsg;
							}
						}
					}



					connection.Close();
				}
			}
			catch (Exception e)
			{
				returnMsg.ReturnMsg = e.Message;
				return returnMsg;
			}



			if (adminUser == "Y" && userlevel != "Z")
				userlevel = "1111";


			// All went well... Lets add the user into the table.  
			string template = "INSERT INTO {0} (QUID, toolAdmin, userlevel, QU_User) VALUES (@qID, @admin, @userlevel, @un)";
			template = String.Format(template, user.UserTable);

			try
			{
				using (SqlConnection connection = new SqlConnection(QuToolsUtilities.GetConnectionString("internet")))
				{

					connection.Open();

					using (SqlCommand command = new SqlCommand(template, connection))
					{
						command.Parameters.AddWithValue("@qID", quId);
						command.Parameters.AddWithValue("@admin", adminUser);
						command.Parameters.AddWithValue("@userlevel", userlevel);
						command.Parameters.AddWithValue("@un", username);


						using (SqlDataReader reader = command.ExecuteReader())
							connection.Close();
					}
				}
			}
			catch (Exception e)
			{
				returnMsg.ReturnMsg = e.Message;
				return returnMsg;
			}

			returnMsg.ReturnMsg = "Success";
			return returnMsg;
		}



		public AccountManagerViewModel CheckAdmin(AccountManagerViewModel user)
		{
			AccountManagerViewModel myConn = user; //new AccountManager();

			Login myLogin = new Login();

			string sqlCmd;

		

			using (SqlConnection connection = new SqlConnection(QuToolsUtilities.GetConnectionString("internet")))
			{
				connection.Open();
				string username = myLogin.ReturnUsername();

				sqlCmd = "select * from {0} where QU_User = @qun";
				sqlCmd = String.Format(sqlCmd, user.UserTable); //, User.username);
				SqlCommand command = new SqlCommand(sqlCmd, connection);
				command.Parameters.AddWithValue("@qun", user.Username);
				using (SqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{

						if (reader["QU_User"].ToString() == username && reader["toolAdmin"].ToString() == "Y")
						{
							myConn.Admin = "Yes";
							myConn.Userlevel = reader["userlevel"].ToString();
							return myConn;
						}
					}
				}
				connection.Close();
			}

			return myConn;
		} //end of checkAdmin()


		public AccountManagerViewModel CheckUser(AccountManagerViewModel user)
		{
			AccountManagerViewModel myConn = user;
			string sqlCmd;

			try
			{
				using (SqlConnection connection = new SqlConnection(QuToolsUtilities.GetConnectionString("internet"))) //ConfigurationExtensions.GetConnectionString(ConfigurationString, "internet")))
				{
					connection.Open();
					string username = QuToolsUtilities.GetSession("QUToolsUser"); // HttpContext.Session.GetString("QUToolsUser");   // HttpContext.Current.Session["QUToolsUser"].ToString();
					sqlCmd = "select * from {0} where QU_User = @qun";
					sqlCmd = String.Format(sqlCmd, user.UserTable);
					SqlCommand command = new SqlCommand(sqlCmd, connection);
					command.Parameters.AddWithValue("@qun", user.Username);
					//command.Parameters.AddWithValue("@db", User.userTable);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							if (reader["QU_User"].ToString() == username && reader["toolAdmin"].ToString() == "N")
							{
								myConn.Admin = "No";
								myConn.Username = username;
								myConn.Userlevel = reader["userlevel"].ToString();
								return myConn;
							}
							else
							{
								myConn.Admin = "Yes";
								myConn.Username = username;
								myConn.Userlevel = reader["userlevel"].ToString();
								return myConn;
							}
						}
					}
					connection.Close();
				}
			}
			catch (Exception e)
			{
				myConn.ReturnMsg = e.Message;
				return myConn;
			}
			return myConn;
		} //end of checkUser()

		public string DelUser(string deluser, string userTable)
		{

			try
			{


				using (SqlConnection con = new SqlConnection(QuToolsUtilities.GetConnectionString("internet")))
				{
					con.Open();
					string sqlstr = "DELETE from {0} where QU_User=@un";
					sqlstr = String.Format(sqlstr, userTable);
					SqlCommand da = new SqlCommand(sqlstr, con);
					da.Parameters.AddWithValue("@un", deluser);
					da.ExecuteNonQuery();

				}
				string returnmsg = "Deleted {0}";
				returnmsg = String.Format(returnmsg, deluser);
				return returnmsg;

			}
			catch (Exception e)
			{
				return e.Message;
			}

		}
		public AccountManagerViewModel SetUserLevel(AccountManagerViewModel user)
		{
			AccountManagerViewModel myConn = user;
			try
			{


				using (SqlConnection con = new SqlConnection(QuToolsUtilities.GetConnectionString("internet")))
				{
					con.Open();
					string sqlstr = "UPDATE {0} SET userlevel=@ul";
					sqlstr = String.Format(sqlstr, user.UserTable);
					SqlCommand da = new SqlCommand(sqlstr, con);
					da.Parameters.AddWithValue("@ul", user.Userlevel);
					da.ExecuteNonQuery();

				}
				myConn.ReturnMsg = "User Level updated";

				return myConn;

			}
			catch (Exception e)
			{
				myConn.ReturnMsg = e.Message;
				return myConn;
			}

		}

		public AccountManagerViewModel GetUsers(AccountManagerViewModel user)
		{
			AccountManagerViewModel users = user;

			users.Userlist = new List<AccountManagerViewModel>();
			try
			{



				using (SqlConnection con = new SqlConnection(QuToolsUtilities.GetConnectionString("internet")))
				{
					con.Open();
					string sqlcmd = "SELECT * FROM {0}";
					sqlcmd = String.Format(sqlcmd, user.UserTable);
					SqlCommand da = new SqlCommand(sqlcmd, con);

					using (SqlDataReader reader = da.ExecuteReader())
					{
						while (reader.Read())
						{
							users.Userlist.Add(new AccountManagerViewModel
							{  /// Note: Not sure why i'm using List_user quid etc... 
								ListUser = reader["QU_User"].ToString(),
								ListQuid = reader["QUID"].ToString(),
								ListAdmin = reader["toolAdmin"].ToString(),
								ListUserlevel = reader["userlevel"].ToString()
							});
						}
						return users;
					}
				}
			}
			catch (Exception e)
			{
				users.ReturnMsg = e.Message;
				return users;


			}

		}

		public int[] UserLevelArray(string userlevel)
		{
			int[] rtnUserLevel = { 0, 0, 0, 0 };

			if (userlevel != null || userlevel.Length != 4 || AccountManager.IsDigitsOnly(userlevel) == false)
			{
				return rtnUserLevel;
			}

			rtnUserLevel[0] = Convert.ToInt32(userlevel.Substring(0, 1));
			rtnUserLevel[1] = Convert.ToInt32(userlevel.Substring(1, 1));
			rtnUserLevel[2] = Convert.ToInt32(userlevel.Substring(2, 1));
			rtnUserLevel[3] = Convert.ToInt32(userlevel.Substring(3, 1));

			return rtnUserLevel;
		}


	}
}