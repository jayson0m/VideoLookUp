using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using QUVideoLinkArchive.ViewModel;
using System.Data.SqlClient;
using System.Data;

namespace QUVideoLinkArchive.Models
{
	public class VideoBackEnd
	{
		/* you may or may not need this */
		public static IConfigurationRoot Configuration { get; set; }
		public static string GetConnectionString(string str)
		{

			var builder = new ConfigurationBuilder()
				 .SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json");
			Configuration = builder.Build();
			return Configuration.GetConnectionString(str);
		}

		/* 
		 * 
		 * 
		 * QUTool_BSIC_Vid_VideoList
				VidID int,
				VideoName varchar(250),
				VideoDescription varchar(2000),
				URL varchar(100),
				DateCompleted date,
				Duration int,
				VideoType varchar(50),
		* 
		*TABLES IN USE
		* All the following will have VidID and tagID 
			QUTool_BSIC_Vid_ProgramVids
			QUTool_BSIC_Vid_SchoolVids
			QUTool_BSIC_Vid_EventsVids
			QUTool_BSIC_Vid_PillarVids
			QUTool_BSIC_Vid_OtherTagsVids
			QUTool_BSIC_Vid_GeoVids
			QUTool_BSIC_Vid_AthleticsVids

		* All will have tagID and tagname. 
			QUTool_BSIC_Vid_Program
			QUTool_BSIC_Vid_School
			QUTool_BSIC_Vid_Events
			QUTool_BSIC_Vid_Pillars
			QUTool_BSIC_Vid_Geo
			QUTool_BSIC_Vid_Athletics
			QUTool_BSIC_Vid_OtherTags
			*/


		public VideoBackendViewModel getTagsForVideo(VideoBackendViewModel tagList)
		{

			tagList.GeneralTagList = new List<VideoBackendViewModel>();

			string sqlCmd = @"select sv.VidID,s.tagID, s.tagname, 'school'  as 'group' from QUTool_BSIC_Vid_SchoolVids sv
						left join QUTool_BSIC_Vid_School s on s.tagID = sv.tagID
						where VidID = @vidID
					union
						select pv.VidID, p.tagID, p.tagname, 'Program' as 'group'  from QUTool_BSIC_Vid_ProgramVids pv 
						left join QUTool_BSIC_Vid_Program p on p.tagID = pv.tagID 
						where VidID = @vidID
					union
						select plv.VidID, pl.tagID, pl.tagname, 'Pillars' as 'group' from QUTool_BSIC_Vid_PillarVids plv
						left join QUTool_BSIC_Vid_Pillars pl  on pl.tagID = plv.tagID
						 where VidID = @vidID
					union
						select ov.VidID, o.tagID, o.tagname, 'OtherTags'  as 'group' from QUTool_BSIC_Vid_OtherTagsVids ov
						left join QUTool_BSIC_Vid_OtherTags o  on o.tagID = ov.tagID 
						where VidID = @vidID
					union
						select gv.VidID, g.tagID, g.tagname, 'Geographical'  as 'group' from QUTool_BSIC_Vid_GeoVids gv 
						left join QUTool_BSIC_Vid_Geo g on g.tagID = gv.tagID 
						where VidID =@vidID
					union
						select ev.VidID, e.tagID, e.tagname, 'Events' as 'group' from QUTool_BSIC_Vid_EventsVids ev
						left join QUTool_BSIC_Vid_Events e  on e.tagID = ev.tagID 
						where VidID = @vidID
					union
						select av.VidID, a.tagID, a.tagname av, 'Athletics' as 'group' from QUTool_BSIC_Vid_AthleticsVids av
						left join QUTool_BSIC_Vid_Athletics a  on a.tagID = av.tagID 
						where VidID = @vidID";

			try
			{
				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
				{
					connection.Open();
					SqlCommand command = new SqlCommand(sqlCmd, connection);

					command.Parameters.Add("@vidID", SqlDbType.VarChar).Value = tagList.vidID;

					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{

							tagList.GeneralTagList.Add(new VideoBackendViewModel
							{
								Tag_ID = reader["tagID"].ToString(),
								Tag = reader["tagname"].ToString(),
								Tag_Group = reader["group"].ToString()
							});

						}
					}

					connection.Close();
				}
			}
			catch (Exception e)
			{
				tagList.returnMsg = e.Message.ToString();
				return tagList;
			}
			return tagList;


		}

		public VideoBackendViewModel jayAllTags(VideoBackendViewModel TagList)
		{
			//VideoBackendViewModel TagList = new VideoBackendViewModel();

			TagList.GeneralTagList = new List<VideoBackendViewModel>();


			TagList.GeneralTagList.AddRange(getTags("QUTool_BSIC_Vid_School"));
			TagList.GeneralTagList.AddRange(getTags("QUTool_BSIC_Vid_Program"));
			TagList.GeneralTagList.AddRange(getTags("QUTool_BSIC_Vid_OtherTags"));
			TagList.GeneralTagList.AddRange(getTags("QUTool_BSIC_Vid_Pillars"));


			TagList.GeneralTagList.AddRange(getTags("QUTool_BSIC_Vid_Events"));
			TagList.GeneralTagList.AddRange(getTags("QUTool_BSIC_Vid_Athletics"));
			TagList.GeneralTagList.AddRange(getTags("QUTool_BSIC_Vid_Geo"));

			return TagList;
		}


		public VideoBackendViewModel getAllTags(VideoBackendViewModel TagList)
		{
			//VideoBackendViewModel TagList = new VideoBackendViewModel();

			TagList.ProgramTags = new List<VideoBackendViewModel>();
			TagList.OtherTags = new List<VideoBackendViewModel>();
			TagList.PillarsTags = new List<VideoBackendViewModel>();
			TagList.SchoolTags = new List<VideoBackendViewModel>();

			TagList.EventsTags = new List<VideoBackendViewModel>();
			TagList.AthleticsTags = new List<VideoBackendViewModel>();
			TagList.GeoTags = new List<VideoBackendViewModel>();

			TagList.SchoolTags = getTags("QUTool_BSIC_Vid_School");
			TagList.ProgramTags = getTags("QUTool_BSIC_Vid_Program");
			TagList.OtherTags = getTags("QUTool_BSIC_Vid_OtherTags");
			TagList.PillarsTags = getTags("QUTool_BSIC_Vid_Pillars");


			TagList.EventsTags = getTags("QUTool_BSIC_Vid_Events");
			TagList.AthleticsTags = getTags("QUTool_BSIC_Vid_Athletics");
			TagList.GeoTags = getTags("QUTool_BSIC_Vid_Geo");

			return TagList;
		}
		public List<VideoBackendViewModel> getTags(string category)
		{
			List<VideoBackendViewModel> GeneralTagList = new List<VideoBackendViewModel>();
			string tag_group = category.Replace("QUTool_BSIC_Vid_", "");
			try
			{
				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
				{
					connection.Open();

					string sqlCmd = "select * from {0} order by tagname";
					sqlCmd = String.Format(sqlCmd, category);
					SqlCommand command = new SqlCommand(sqlCmd, connection);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{

							GeneralTagList.Add(new VideoBackendViewModel
							{
								Tag_ID = reader["tagID"].ToString(),
								Tag = QUToolsUtilities.HtmlDecode(reader["tagname"].ToString()),
								Tag_Group = tag_group

							});
						}
					}

					connection.Close();
				}
			}
			catch (Exception e)
			{

			}

			return GeneralTagList;
		}


		public string VidIDMaker(string videoname)
		{
			string compname = Environment.MachineName + videoname;
			int seed = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			Random number = new Random(seed);

			string myNum;

			myNum = "";

			for (var r = 0; r <= 7; r++)
			{
				myNum += number.Next(9).ToString();

			}

			if (compname.Length > 8)
			{
				compname = compname.Substring(0, 8);
			}
			else if (compname.Length < 8)
			{
				compname += DateTime.Now.ToString("mmhhssdd");
				compname = compname.Substring(0, 8);
			}

			byte[] compbytes = Encoding.ASCII.GetBytes(compname);
			byte[] timebytes = Encoding.ASCII.GetBytes(myNum);
			string outputstr = "";
			int buff;
			int x = 0;

			foreach (byte i in compbytes)
			{
				do
				{
					buff = (compbytes[x] * timebytes[x]) % number.Next(20, 240);
				} while (buff > 250);
				outputstr += String.Format("{0:X}", buff);
			}
			return outputstr;
		}

		public string[] parseTags(string tags)
		{
			string[] tagArray = tags.Split('|');
			return tagArray;

		}

		public string processEntry(VideoBackendViewModel newVideolink, string[] tags, string username)
		{
			string vidID;
			if (newVideolink.vidID == null)
				vidID = VidIDMaker(newVideolink.VideoName);
			else
				vidID = newVideolink.vidID;

			string y = addVideo2db(newVideolink, vidID, username);

			if (y != "true")
				return "280: " + y;

			foreach (string i in tags)
			{
				if (i != "")
				{
					string[] tagArray = processRawTag(i);
					bool x = addTag2db(tagArray, vidID);
					if (x == false)
					{
						break;
					}
				}


			}

			return "Success";

		}

		public string[] processRawTag(string tag)   // Tag looks like {category},{vidID}
		{
			string[] tagArray = tag.Split('_');

			switch (tagArray[0].ToLower())
			{
				case "school":
					tagArray[0] = "QUTool_BSIC_Vid_SchoolVids";
					break;
				case "program":
					tagArray[0] = "QUTool_BSIC_Vid_ProgramVids";
					break;
				case "pillars":
					tagArray[0] = "QUTool_BSIC_Vid_PillarVids";
					break;
				case "events":
					tagArray[0] = "QUTool_BSIC_Vid_EventsVids";
					break;
				case "geographical":
					tagArray[0] = "QUTool_BSIC_Vid_GeoVids";
					break;
				case "geo":
					tagArray[0] = "QUTool_BSIC_Vid_GeoVids";
					break;
				case "athletics":
					tagArray[0] = "QUTool_BSIC_Vid_AthleticsVids";
					break;
				case "othertags":
					tagArray[0] = "QUTool_BSIC_Vid_OtherTagsVids";
					break;
				default:

					break;
			}
			return tagArray;
		}

		public string processNewTags(string[] tags)
		{

			foreach (string i in tags)
			{
				if (i != "")
				{
					string[] tagArray = processRawTagForNew(i);
					if (tagArray[0] == "1")
					{
						string x = addNewTag2db(tagArray);
						if (x != "Success")
						{
							return x;
						}
					}
				}


			}

			return "Success";

		}

		public string[] processRawTagForNew(string tag)   // Tag looks like {new}-{category}-{name}
		{
			string[] tagArray = tag.Split('_');

			if (tagArray[0] == "1")
			{
				switch (tagArray[1].ToLower())
				{
					case "school":
						tagArray[1] = "QUTool_BSIC_Vid_School";
						break;
					case "program":
						tagArray[1] = "QUTool_BSIC_Vid_Program";
						break;
					case "pillars":
						tagArray[1] = "QUTool_BSIC_Vid_Pillars";
						break;
					case "events":
						tagArray[1] = "QUTool_BSIC_Vid_Events";
						break;
					case "geographical":
						tagArray[1] = "QUTool_BSIC_Vid_Geo";
						break;
					case "geo":
						tagArray[1] = "QUTool_BSIC_Vid_Geo";
						break;
					case "athletics":
						tagArray[1] = "QUTool_BSIC_Vid_Athletics";
						break;
					case "othertags":
						tagArray[1] = "QUTool_BSIC_Vid_OtherTags";
						break;
					default:

						break;
				}
				return tagArray;
			}
			string[] bad = { "No New Tags" };
			return bad;

		}

		public string addVideo2db(VideoBackendViewModel videoInfo, string vidID, string username)
		{
			//try
			//{
				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
				{
					connection.Open();

					string sqlCmd = "INSERT INTO QUTool_BSIC_Vid_VideoList (VidID, VideoName, VideoDescription, URL, DateCompleted, Duration, VideoType, AddedDate, AddedBy)" +
																	 "VALUES (@vidID, @VideoName, @VideoDescription, @URL, @DateCompleted, " +
																			 "@Duration, @VideoType, @AddedDate, @username);";
					SqlCommand command = new SqlCommand(sqlCmd, connection);

					command.Parameters.Add("@vidID", SqlDbType.VarChar).Value = vidID;
					command.Parameters.Add("@VideoName", SqlDbType.VarChar).Value = videoInfo.VideoName;
					command.Parameters.Add("@VideoDescription", SqlDbType.VarChar, 9999).Value = videoInfo.VideoDescription;
					command.Parameters.Add("@URL", SqlDbType.VarChar).Value = videoInfo.URL;
					command.Parameters.Add("@DateCompleted", SqlDbType.DateTime).Value = videoInfo.DateCompleted;
					command.Parameters.Add("@Duration", SqlDbType.VarChar).Value = videoInfo.Duration;
					command.Parameters.Add("@VideoType", SqlDbType.VarChar).Value = videoInfo.VideoType;
					command.Parameters.Add("@AddedDate", SqlDbType.DateTime).Value = DateTime.Now;
					command.Parameters.Add("@username", SqlDbType.VarChar).Value = username;


					command.ExecuteNonQuery();

					connection.Close();
				}
			//}
			//catch (Exception e)
			//{
			//	return e.Message.ToString();
			//}


			return "true";
		}

		public bool addTag2db(string[] tag, string vidID)
		{
			VideoBackendViewModel allTags = new VideoBackendViewModel();
			allTags = getAllTags(allTags);

			/*var theTag = allTags.SchoolTags.First(x => x.Tag_ID == tag[1]);

			if (theTag.Tag_ID == null)
				return false;
			*/

			try
			{
				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
				{
					connection.Open();

					string sqlCmd = @"INSERT INTO {0} (vidID, tagID)
													  VALUES (@vidID, @tagID);";

					sqlCmd = String.Format(sqlCmd, tag[0]);
					SqlCommand command = new SqlCommand(sqlCmd, connection);

					int tagID = Convert.ToInt16(tag[1]);


					command.Parameters.Add("@vidID", SqlDbType.VarChar).Value = vidID;
					command.Parameters.Add("@tagID", SqlDbType.Int).Value = tagID;
					command.ExecuteNonQuery();

					connection.Close();
				}
			}
			catch (Exception e)
			{
				return false;
			}


			return true;
		}

		public string addNewTag2db(string[] tag)
		{
			VideoBackendViewModel allTags = new VideoBackendViewModel();
			allTags = getAllTags(allTags);

			try
			{
				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
				{
					connection.Open();

					string sqlCmd = @"INSERT INTO {0} (tagname)
											   VALUES (@tagname);";

					sqlCmd = String.Format(sqlCmd, tag[1]);
					SqlCommand command = new SqlCommand(sqlCmd, connection);

					command.Parameters.Add("@tagname", SqlDbType.VarChar).Value = tag[2];
					command.ExecuteNonQuery();

					connection.Close();
				}
			}
			catch (Exception e)
			{
				return e.Message;
			}


			return "Success";
		}

		public VideoBackendViewModel getVideoTags(string vidID)
		{
			VideoBackendViewModel VidTags = new VideoBackendViewModel();



			return VidTags;

		}

		public VideoBackendViewModel listVideos(string searchParam)
		{

			// Maybe date? 
			VideoBackendViewModel vidObj = new VideoBackendViewModel();
			vidObj.VideoInfo = new List<VideoBackendViewModel>();
			string sqlCmd = "select * from QUTool_BSIC_Vid_VideoList";
			bool search = false;

			if (searchParam != "all")
			{
				sqlCmd = "select * from QUTool_BSIC_Vid_VideoList where vidid = @vidid";
				search = true;
			}

			try
			{
				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
				{
					connection.Open();

					///sqlCmd = String.Format(sqlCmd, category);
					SqlCommand command = new SqlCommand(sqlCmd, connection);
					if (search == true)
						command.Parameters.Add("@vidid", SqlDbType.VarChar).Value = searchParam;


					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{

							vidObj.VideoInfo.Add(new VideoBackendViewModel
							{
								VideoName = QUToolsUtilities.HtmlDecode(reader["Videoname"].ToString()),
								VideoDescription = QUToolsUtilities.HtmlDecode(reader["VideoDescription"].ToString()),
								URL = QUToolsUtilities.HtmlDecode(reader["URL"].ToString()),
								DateCompleted = Convert.ToDateTime(reader["DateCompleted"]),
								Duration = QUToolsUtilities.HtmlDecode(reader["Duration"].ToString()),
								VideoType = QUToolsUtilities.HtmlDecode(reader["VideoType"].ToString()),
								Addedby = reader["AddedBy"].ToString(),
								UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"]),
								AddedDate = Convert.ToDateTime(reader["AddedDate"]),
								vidID = reader["VidID"].ToString()
							});
						}
					}

					connection.Close();
				}
			}
			catch (Exception e)
			{
				vidObj.returnMsg = e.Message.ToString();
				return vidObj;
			}


			return vidObj;

		}

		public string cleanLinkEntrys(string vidID)
		{
			string sqlCmd = @"Delete from QUTool_BSIC_Vid_SchoolVids where vidid = @vidid;
						Delete from QUTool_BSIC_Vid_ProgramVids where vidid = @vidid;
						Delete from QUTool_BSIC_Vid_PillarVids where vidid = @vidid;
						Delete from QUTool_BSIC_Vid_EventsVids where vidid = @vidid;
						Delete from QUTool_BSIC_Vid_GeoVids where vidid = @vidid;
						Delete from QUTool_BSIC_Vid_AthleticsVids where vidid = @vidid;
						Delete from QUTool_BSIC_Vid_OtherTagsVids where vidid = @vidid;
						Delete from QUTool_BSIC_Vid_VideoList where vidid = @vidid;";

			try
			{
				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
				{
					connection.Open();


					SqlCommand command = new SqlCommand(sqlCmd, connection);




					command.Parameters.Add("@vidid", SqlDbType.VarChar).Value = vidID;
					command.ExecuteNonQuery();

					connection.Close();
				}
			}
			catch (Exception e)
			{
				return e.Message.ToString();
			}


			return "Success";
		}

		public string deletetag(string tag)
		{
			string[] tagArray = tag.Split('_');
			string db, tagid;
			if (tagArray[0] != "0")
			{
				db = tagArray[0];
				tagid = tagArray[1];
			}
			else
			{
				db = tagArray[1];
				tagid = tagArray[2];
			}

			switch (db.ToLower())
			{
				case "school":
					db = "QUTool_BSIC_Vid_School";
					break;
				case "program":
					db = "QUTool_BSIC_Vid_Program";
					break;
				case "pillars":
					db = "QUTool_BSIC_Vid_Pillars";
					break;
				case "events":
					db = "QUTool_BSIC_Vid_Events";
					break;
				case "geographical":
					db = "QUTool_BSIC_Vid_Geo";
					break;
				case "geo":
					db = "QUTool_BSIC_Vid_Geo";
					break;
				case "athletics":
					db = "QUTool_BSIC_Vid_Athletics";
					break;
				case "othertags":
					db = "QUTool_BSIC_Vid_OtherTags";
					break;
				default:

					break;
			}


			string sqlCmd = @"Delete from {0} where tagid = @tagid;";

			sqlCmd = String.Format(sqlCmd, db);
			try
			{
				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
			{
				connection.Open();


				SqlCommand command = new SqlCommand(sqlCmd, connection);




				
				command.Parameters.Add("@tagid", SqlDbType.VarChar).Value = tagid;
				command.ExecuteNonQuery();

				connection.Close();
			}
			}
			catch (Exception e)
			{
				return e.Message.ToString();
			}


			return "Success";

		}
		
		public string renametag(string tag, string newname)
		{
			string[] tagArray = tag.Split('_');
			string db, tagid;
			if (tagArray[0] != "0")
			{
				db = tagArray[0];
				tagid = tagArray[1];
			}
			else
			{
				db = tagArray[1];
				tagid = tagArray[2];
			}

			switch (db.ToLower())
			{
				case "school":
					db = "QUTool_BSIC_Vid_School";
					break;
				case "program":
					db = "QUTool_BSIC_Vid_Program";
					break;
				case "pillars":
					db = "QUTool_BSIC_Vid_Pillars";
					break;
				case "events":
					db = "QUTool_BSIC_Vid_Events";
					break;
				case "geographical":
					db = "QUTool_BSIC_Vid_Geo";
					break;
				case "geo":
					db = "QUTool_BSIC_Vid_Geo";
					break;
				case "athletics":
					db = "QUTool_BSIC_Vid_Athletics";
					break;
				case "othertags":
					db = "QUTool_BSIC_Vid_OtherTags";
					break;
				default:

					break;
			}

			string sqlCmd = @"update {0} set tagname = @newname where tagid = @tagid;";

			sqlCmd = String.Format(sqlCmd, db);
			try
			{
				using (SqlConnection connection = new SqlConnection(QUToolsUtilities.GetConnectionString("internet")))
				{
					connection.Open();


					SqlCommand command = new SqlCommand(sqlCmd, connection);
					command.Parameters.Add("@tagid", SqlDbType.VarChar).Value = tagid;
					command.Parameters.Add("@newname", SqlDbType.VarChar).Value = newname;
					command.ExecuteNonQuery();

					connection.Close();
				}
			}
			catch (Exception e)
			{
				return e.Message;
			}


			return "Success";

		}
		
	}
}
