using System;
using System.Collections.Generic;

using System.IO;
namespace LinkArchiveToolFrontEnd.ViewModel
{
    public class HomeViewModel
    {
        
		public List<HomeViewModel> ProgramTags { set; get; }
		public List<HomeViewModel> SchoolTags { set; get; }
		public List<HomeViewModel> EventsTags { set; get; }
		public List<HomeViewModel> PillarsTags { set; get; }
		public List<HomeViewModel> GeoTags { set; get; }
		public List<HomeViewModel> AthleticsTags { set; get; }
		public List<HomeViewModel> OtherTags { set; get; }

		public List<HomeViewModel> GeneralTagList { set; get; }
		public string TagId { set; get; }
		public string Tag { set; get; }
		public string TagGroup { set; get; }

		public List<HomeViewModel> VideoInfo { set; get; }
		public DateTime DateCompleted { set; get; }
		public DateTime AddedDate { get; set; }
		public DateTime UpdatedDate { set; get; }
		public string VidId { set; get; }
		public string VideoName { set; get; }
		public string VideoDescription { set; get; }
		public string Url { set; get; }
		public string Duration { set; get; }
		public string VideoType { set; get; }
		public string Addedby { get; set; }
		

		public List<HomeViewModel> SearchResults { get; set; }
		public List<HomeViewModel> SearchTags { get; set; }

		public DateTime SearchDateFrom { get; set; }
		public DateTime SearchDateTo { get; set; }
		public string SearchVideoType { get; set; }
		public string SearchString { get; set; }
		public string SearchRawTags { get; set; }

		public string CsvResults { get; set; }
		public string ReturnMsg { get; set; }

    }
}
