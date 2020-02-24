using System;
using System.Collections.Generic;
using System.IO;
using LinkArchiveToolFrontEnd.ViewModel;
using System.Data.SqlClient;
using System.Net;

namespace LinkArchiveToolFrontEnd.Models
{
    public class Home
    {
        public HomeViewModel GetAllTags(HomeViewModel tagList)
        {
            tagList.ProgramTags = new List<HomeViewModel>();
            tagList.OtherTags = new List<HomeViewModel>();
            tagList.PillarsTags = new List<HomeViewModel>();
            tagList.SchoolTags = new List<HomeViewModel>();

            tagList.EventsTags = new List<HomeViewModel>();
            tagList.AthleticsTags = new List<HomeViewModel>();
            tagList.GeoTags = new List<HomeViewModel>();

            tagList.SchoolTags = GetTags("QUTool_BSIC_Vid_School");
            tagList.ProgramTags = GetTags("QUTool_BSIC_Vid_Program");
            tagList.OtherTags = GetTags("QUTool_BSIC_Vid_OtherTags");
            tagList.PillarsTags = GetTags("QUTool_BSIC_Vid_Pillars");


            tagList.EventsTags = GetTags("QUTool_BSIC_Vid_Events");
            tagList.AthleticsTags = GetTags("QUTool_BSIC_Vid_Athletics");
            tagList.GeoTags = GetTags("QUTool_BSIC_Vid_Geo");

            return tagList;
        }

        private static List<HomeViewModel> GetTags(string category)
        {
            List<HomeViewModel> generalTagList = new List<HomeViewModel>();
            string tagGroup = category.Replace("QUTool_BSIC_Vid_", "");
            try
            {
                using (SqlConnection connection = new SqlConnection(QuToolsUtilities.GetConnectionString("internet")))
                {
                    connection.Open();

                    string sqlCmd = "select * from {0} order by tagname";
                    sqlCmd = String.Format(sqlCmd, category);
                    SqlCommand command = new SqlCommand(sqlCmd, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            generalTagList.Add(new HomeViewModel
                            {
                                TagId = reader["tagID"].ToString(),
                                Tag = reader["tagname"].ToString(),
                                TagGroup = tagGroup

                            });
                        }
                    }

                    connection.Close();
                }
            }
            catch
            {
                
                return generalTagList;
            }

            return generalTagList;
        }


        public HomeViewModel ParseTags(HomeViewModel tagList)
        {
            string[] tagArray = tagList.SearchRawTags.Split(',');

            tagList.ProgramTags = new List<HomeViewModel>();
            tagList.OtherTags = new List<HomeViewModel>();
            tagList.PillarsTags = new List<HomeViewModel>();
            tagList.SchoolTags = new List<HomeViewModel>();

            tagList.EventsTags = new List<HomeViewModel>();
            tagList.AthleticsTags = new List<HomeViewModel>();
            tagList.GeoTags = new List<HomeViewModel>();

            foreach (var i in tagArray)
            {

                string[] thisTag = i.Split('-');

                switch (thisTag[0])
                {
                    case "school":
                        tagList.SchoolTags.Add(new HomeViewModel
                        {
                            TagGroup = thisTag[0],
                            TagId = thisTag[1]
                        });

                        break;
                    case "Program":
                        tagList.ProgramTags.Add(new HomeViewModel
                        {
                            TagGroup = thisTag[0],
                            TagId = thisTag[1]
                        });
                        break;
                    case "Pillars":
                        tagList.PillarsTags.Add(new HomeViewModel
                        {
                            TagGroup = thisTag[0],
                            TagId = thisTag[1]
                        });
                        break;
                    case "Events":
                        tagList.EventsTags.Add(new HomeViewModel
                        {
                            TagGroup = thisTag[0],
                            TagId = thisTag[1]
                        });
                        break;
                    case "Geographical":
                        tagList.GeoTags.Add(new HomeViewModel
                        {
                            TagGroup = thisTag[0],
                            TagId = thisTag[1]
                        });
                        break;
                    case "Athletics":
                        tagList.AthleticsTags.Add(new HomeViewModel
                        {
                            TagGroup = thisTag[0],
                            TagId = thisTag[1]
                        });
                        break;
                    case "OtherTags":
                        tagList.OtherTags.Add(new HomeViewModel
                        {
                            TagGroup = thisTag[0],
                            TagId = thisTag[1]
                        });
                        break;
                }

            }
            return tagList;

        }



        public HomeViewModel Search(HomeViewModel searchInfo)
        {
            if (searchInfo.SearchRawTags != null)
            {
                searchInfo = ParseTags(searchInfo);
            }
            else
            {
                searchInfo.SearchRawTags = "0";
            }


            string sqlTmpDb = @"
                IF Exists (Select * From tempdb..sysobjects Where name like '#QUToolsTemp') DROP TABLE #QUToolsTemp 
                
                select * INTO #QUToolsTemp
                from (
                    select sv.VidID,s.tagID, s.tagname, 'school'  as 'grouper' from QUTool_BSIC_Vid_SchoolVids sv
                    left join QUTool_BSIC_Vid_School s on s.tagID = sv.tagID
                    union
                    select pv.VidID, p.tagID, p.tagname, 'program' as 'grouper'  from QUTool_BSIC_Vid_ProgramVids pv 
                    left join QUTool_BSIC_Vid_Program p on p.tagID = pv.tagID 
                    union
                    select plv.VidID, pl.tagID, pl.tagname, 'pillar' as 'grouper' from QUTool_BSIC_Vid_PillarVids plv
                    left join QUTool_BSIC_Vid_Pillars pl  on pl.tagID = plv.tagID
                    union
                    select ov.VidID, o.tagID, o.tagname, 'othertags'  as 'grouper' from QUTool_BSIC_Vid_OtherTagsVids ov
                    left join QUTool_BSIC_Vid_OtherTags o  on o.tagID = ov.tagID 
                    union
                    select gv.VidID, g.tagID, g.tagname, 'geo'  as 'grouper' from QUTool_BSIC_Vid_GeoVids gv 
                    left join QUTool_BSIC_Vid_Geo g on g.tagID = gv.tagID 
                    union
                    select ev.VidID, e.tagID, e.tagname, 'events' as 'grouper' from QUTool_BSIC_Vid_EventsVids ev
                    left join QUTool_BSIC_Vid_Events e  on e.tagID = ev.tagID 
                    union
                    select av.VidID, a.tagID, a.tagname av, 'athletics' as 'grouper' from QUTool_BSIC_Vid_AthleticsVids av
                    left join QUTool_BSIC_Vid_Athletics a  on a.tagID = av.tagID 
                ) data";

            string sqlstr = BuildSqlStr(searchInfo);


            try
            {
                using (SqlConnection connection = new SqlConnection(QuToolsUtilities.GetConnectionString("internet")))
                {
                    connection.Open();

                    SqlCommand command1 = new SqlCommand(sqlTmpDb, connection);
                    command1.ExecuteNonQuery();

                    SqlCommand command = new SqlCommand(sqlstr, connection);
                    searchInfo.SearchResults = new List<HomeViewModel>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            searchInfo.SearchResults.Add(new HomeViewModel
                            {
                                VideoName = WebUtility.HtmlDecode(reader["VideoName"].ToString()),
                                VideoDescription = WebUtility.HtmlDecode(reader["VideoDescription"].ToString()),
                                Url = reader["URL"].ToString(),
                                DateCompleted = Convert.ToDateTime(reader["DateCompleted"]),
                                Duration = reader["Duration"].ToString(),
                                VideoType = reader["VideoType"].ToString(),
                                VidId = reader["VidID"].ToString()
                            });
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                searchInfo.ReturnMsg = e.Message;
                return searchInfo;
            }

            return searchInfo;
        }


        private string BuildSqlStr(HomeViewModel searchInfo)
        {
            string schoolStr = "", programStr = "", pillarStr = "", othertagStr = "", geoStr = "", eventsStr = "", athleticsStr = "", searchStr = "", vidType = "", sqlStr = "";
            DateTime dateF, dateT;
            searchStr = searchInfo.SearchString;
            vidType = searchInfo.SearchVideoType;
            dateF = searchInfo.SearchDateFrom;
            dateT = searchInfo.SearchDateTo;

            int x = 0;
            int tick = 0;
            if (searchInfo.SearchRawTags != "0")
            {
                if (searchInfo.SchoolTags.Count != 0)
                {
                    schoolStr = "(t.grouper = 'school' and ";
                    foreach (var i in searchInfo.SchoolTags)
                    {
                        string tagstr;
                        if (x != 0)
                            tagstr = " or ";
                        x++;
                        tagstr = " t.tagID = '{0}'";
                        schoolStr += String.Format(tagstr, WebUtility.HtmlEncode(i.TagId));
                    }
                    schoolStr += ")";
                    x = 0;
                    tick++;
                }
                if (searchInfo.ProgramTags.Count != 0)
                {
                    if (tick != 0)
                        programStr = "or (t.grouper = 'program' and ";
                    else
                        programStr = "(t.grouper = 'program' and ";

                    foreach (var i in searchInfo.ProgramTags)
                    {
                        string tagstr;
                        if (x != 0)
                            tagstr = " or ";
                        x++;
                        tagstr = " t.tagID = '{0}'";
                        programStr += String.Format(tagstr, WebUtility.HtmlEncode(i.TagId));
                    }
                    programStr += ")";
                    x = 0;
                    tick++;
                }
                if (searchInfo.PillarsTags.Count != 0)
                {
                    if (tick != 0)
                        pillarStr = "or (t.grouper = 'pillar' and ";
                    else
                        pillarStr = "(t.grouper = 'pillar' and ";

                    foreach (var i in searchInfo.PillarsTags)
                    {
                        string tagstr;
                        if (x != 0)
                            tagstr = " or ";
                        x++;
                        tagstr = " t.tagID = '{0}'  ";
                        pillarStr += String.Format(tagstr, WebUtility.HtmlEncode(i.TagId));
                    }
                    pillarStr += ")";
                    x = 0;
                    tick++;
                }
                if (searchInfo.OtherTags.Count != 0)
                {
                    if (tick != 0)
                        othertagStr = " or (t.grouper = 'othertags' and ";
                    else
                        othertagStr = "(t.grouper = 'othertags' and ";

                    foreach (var i in searchInfo.OtherTags)
                    {
                        string tagstr;
                        if (x != 0)
                            tagstr = " or ";
                        x++;
                        tagstr = " t.tagID = '{0}'  ";
                        othertagStr += String.Format(tagstr, WebUtility.HtmlEncode(i.TagId));
                    }
                    othertagStr += ")";
                    x = 0;
                    tick++;
                }
                if (searchInfo.GeoTags.Count != 0)
                {
                    if (tick != 0)
                        geoStr = " or (t.grouper = 'geo' and ";
                    else
                        geoStr = "(t.grouper = 'geo' and ";

                    foreach (var i in searchInfo.GeoTags)
                    {
                        string tagstr;
                        if (x != 0)
                            tagstr = " or ";
                        x++;
                        tagstr = " t.tagID = '{0}'  ";
                        geoStr += String.Format(tagstr, WebUtility.HtmlEncode(i.TagId));
                    }
                    geoStr += ")";
                    x = 0;
                    tick++;
                }

                if (searchInfo.EventsTags.Count != 0)
                {
                    if (tick != 0)
                        eventsStr = " or (t.grouper = 'events' and ";
                    else
                        eventsStr = "(t.grouper = 'events' and ";

                    foreach (var i in searchInfo.EventsTags)
                    {
                        string tagstr;
                        if (x != 0)
                            tagstr = " or ";
                        x++;
                        tagstr = " t.tagID = '{0}'  ";
                        eventsStr += String.Format(tagstr, WebUtility.HtmlEncode(i.TagId));
                    }
                    eventsStr += ")";
                    x = 0;
                    tick++;
                }
                if (searchInfo.AthleticsTags.Count != 0)
                {
                    if (tick != 0)
                        athleticsStr = " or (t.grouper = 'athletics' and ";
                    else
                        athleticsStr = "(t.grouper = 'athletics' and ";
                    foreach (var i in searchInfo.AthleticsTags)
                    {
                        string tagstr;
                        if (x != 0)
                            tagstr = " or ";
                        x++;
                        tagstr = " t.tagID = '{0}'  ";
                        athleticsStr += String.Format(tagstr, WebUtility.HtmlEncode(i.TagId));

                    }
                    athleticsStr += ")";
                    x = 0;
                    tick++;
                }
            }

            sqlStr = @"select distinct * from #QUToolsTemp t left join QUTool_BSIC_Vid_VideoList vl on vl.VidID = t.VidID where {0} {1} {2} {3} {4} {5} {6} {8} (vl.videoname like '%{7}%' or  vl.VideoDescription like '%{7}%' or vl.URL like '%{7}%')";
            
            string datesearch = "";
            if (dateF.Year > 2000)
            {
                datesearch = "and vl.DateCompleted >= '" + dateF.ToString() + "'";
                sqlStr += datesearch;
            }
            if (dateT.Year > 2000)
            {
                datesearch += "and vl.DateCompleted <= '" + dateT.ToString() + "'";
                sqlStr += datesearch;
            }

            if (vidType != null)
            {
                string typesearch = "and vl.VideoType like '%{0}%'";
                typesearch = String.Format(typesearch, vidType);
                sqlStr += typesearch;
            }
            string attachit = "";
            if (searchInfo.SearchRawTags != "0")
                attachit = "and";

            sqlStr += " order by t.vidID";
            sqlStr = String.Format(sqlStr, schoolStr, programStr, pillarStr, othertagStr, geoStr, eventsStr, athleticsStr, searchStr, attachit);
            return sqlStr;

        }
        public  string Convert2Csv(HomeViewModel searchResults)
        {
            StringWriter csv = new StringWriter();
            string lastVidId = "";
            foreach (var items in searchResults.SearchResults)
            {
                if (items.VidId != lastVidId) {
                    csv.WriteLine(String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"", items.VideoName, items.VideoDescription, items.Url, items.VideoType, items.Duration, items.DateCompleted.ToString("MM-dd-yyyy")));

                }
                lastVidId = items.VidId;

            }
            return csv.ToString();

        }


    }
}