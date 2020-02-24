
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QUVideoLinkArchive.ViewModel;

namespace QUVideoLinkArchive.Models
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
		public AccountManagerViewModel addUser(AccountManagerViewModel User)
		{
			//Add the user to DB
			AccountManagerViewModel returnMsg = User;
			string username = User.username.ToString();
			username = username.ToLower();
			string userTable = User.userTable.ToString();
			string quID = User.quID.ToString();
			string userlevel = "Z";

			if (User.userlevel != null)
			{
				userlevel = User.userlevel.ToString();
			}




			string admin_user;

			if (User.Admin == "Yes")
			{
				admin_user = "Y";
			}
			else
			{
				admin_user = "N";
			}


			// Check to see if username is in datatel 
			try
			{

				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("datatel")))      
				{
					connection.Open();


					string sqlCmd = "select * from person where NTLoginID = @un";
					SqlCommand command = new SqlCommand(sqlCmd, connection);
					command.Parameters.AddWithValue("@un", username);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (!reader.HasRows)
						{
							returnMsg.returnMsg = "Invalid User";
							return returnMsg;
						}
					}
					connection.Close();
				}
			}
			catch (Exception e)
			{
				returnMsg.returnMsg = e.ToString();
				return returnMsg;
			}

			// Check if user is already in database
			try
			{
				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
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

							if (reader["QU_User"].ToString() == User.username)
							{
								connection.Close();
								returnMsg.returnMsg = "User already Exists.";
								return returnMsg;
							}
						}
					}



					connection.Close();
				}
			}
			catch (Exception e)
			{
				returnMsg.returnMsg = e.Message;
				return returnMsg;
			}



			if (admin_user == "Y" && userlevel != "Z")
				userlevel = "1111";


			// All went well... Lets add the user into the table.  
			string template = "INSERT INTO {0} (QUID, toolAdmin, userlevel, QU_User) VALUES (@qID, @admin, @userlevel, @un)";
			template = String.Format(template, User.userTable);

			try
			{
				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
				{

					connection.Open();

					using (SqlCommand command = new SqlCommand(template, connection))
					{
						command.Parameters.AddWithValue("@qID", quID);
						command.Parameters.AddWithValue("@admin", admin_user);
						command.Parameters.AddWithValue("@userlevel", userlevel);
						command.Parameters.AddWithValue("@un", username);


						using (SqlDataReader reader = command.ExecuteReader())
							connection.Close();
					}
				}
			}
			catch (Exception e)
			{
				returnMsg.returnMsg = e.Message;
				return returnMsg;
			}

			returnMsg.returnMsg = "Success";
			return returnMsg;
		}



		public AccountManagerViewModel checkAdmin(AccountManagerViewModel User)
		{
			AccountManagerViewModel myConn = User; //new AccountManager();

			login myLogin = new login();

			string sqlCmd;

			// Check to see if user is an admin. 
			try
			{

				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
				{

					connection.Open();
					string username = myLogin.returnUsername();

					sqlCmd = "select * from {0} where QU_User = @qun";
					sqlCmd = String.Format(sqlCmd, User.userTable); //, User.username);
					SqlCommand command = new SqlCommand(sqlCmd, connection);
					command.Parameters.AddWithValue("@qun", User.username);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{

							if (reader["QU_User"].ToString() == username && reader["toolAdmin"].ToString() == "Y")
							{
								myConn.Admin = "Yes";
								myConn.userlevel = reader["userlevel"].ToString();
								return myConn;
							}
						}
					}

					connection.Close();
				}
			}
			catch (Exception e)
			{
				myConn.returnMsg = e.Message;
			}

			return myConn;

		} //end of checkAdmin()


		public AccountManagerViewModel checkUser(AccountManagerViewModel User)
		{
			AccountManagerViewModel myConn = User;
			string sqlCmd;

			try
			{
				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
				{
					connection.Open();
					string username = QUToolsUtilities.GetSession("QToolsUser"); 
					sqlCmd = "select * from {0} where QU_User = @qun";
					sqlCmd = String.Format(sqlCmd, User.userTable);
					SqlCommand command = new SqlCommand(sqlCmd, connection);
					command.Parameters.AddWithValue("@qun", User.username);
					//command.Parameters.AddWithValue("@db", User.userTable);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							if (reader["QU_User"].ToString() == username && reader["toolAdmin"].ToString() == "N")
							{
								myConn.Admin = "No";
								myConn.username = username;
								myConn.userlevel = reader["userlevel"].ToString();
								return myConn;
							}
							else
							{
								myConn.Admin = "Yes";
								myConn.username = username;
								myConn.userlevel = reader["userlevel"].ToString();
								return myConn;
							}
						}
					}
					connection.Close();
				}
			}
			catch (Exception e)
			{
				myConn.returnMsg = e.Message;
				return myConn;
			}
			return myConn;
		} //end of checkUser()

		public string delUser(string deluser, string userTable)
		{

			try
			{


				using (SqlConnection con = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
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
		public AccountManagerViewModel setUserLevel(AccountManagerViewModel user)
		{
			AccountManagerViewModel myConn = user;
			try
			{


				using (SqlConnection con = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
				{
					con.Open();
					string sqlstr = "UPDATE {0} SET userlevel=@ul";
					sqlstr = String.Format(sqlstr, user.userTable);
					SqlCommand da = new SqlCommand(sqlstr, con);
					da.Parameters.AddWithValue("@ul", user.userlevel);
					da.ExecuteNonQuery();

				}
				myConn.returnMsg = "User Level updated";

				return myConn;

			}
			catch (Exception e)
			{
				myConn.returnMsg = e.Message;
				return myConn;
			}

		}

		public AccountManagerViewModel getUsers(AccountManagerViewModel user)
		{
			AccountManagerViewModel users = user;

			users.userlist = new List<AccountManagerViewModel>();
			try
			{



				using (SqlConnection con = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
				{
					con.Open();
					string sqlcmd = "SELECT * FROM {0}";
					sqlcmd = String.Format(sqlcmd, user.userTable);
					SqlCommand da = new SqlCommand(sqlcmd, con);

					using (SqlDataReader reader = da.ExecuteReader())
					{
						while (reader.Read())
						{
							users.userlist.Add(new AccountManagerViewModel
							{  /// Note: Not sure why i'm using List_user quid etc... 
								list_user = reader["QU_User"].ToString(),
								list_QUID = reader["QUID"].ToString(),
								list_admin = reader["toolAdmin"].ToString(),
								list_userlevel = reader["userlevel"].ToString()
							});
						}
						return users;
					}
				}
			}
			catch (Exception e)
			{
				users.returnMsg = e.Message;
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