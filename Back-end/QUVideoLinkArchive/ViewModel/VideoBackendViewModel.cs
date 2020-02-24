using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace QUVideoLinkArchive.ViewModel
{
	public class VideoBackendViewModel
	{
		public List<VideoBackendViewModel> ProgramTags { set; get; }
		public List<VideoBackendViewModel> SchoolTags { set; get; }
		public List<VideoBackendViewModel> EventsTags { set; get; }
		public List<VideoBackendViewModel> PillarsTags { set; get; }
		public List<VideoBackendViewModel> GeoTags { set; get; }
		public List<VideoBackendViewModel> AthleticsTags { set; get; }
		public List<VideoBackendViewModel> OtherTags { set; get; }

		public List<VideoBackendViewModel> GeneralTagList { set; get; }

		public string Tag_ID { set; get; }
		public string Tag { set; get; }
		public string Tag_Group { set; get; }

		public List<VideoBackendViewModel> VideoInfo { set; get; }
		public string vidID { set; get; }
		public string VideoName { set; get; }
		public string VideoDescription { set; get; }
		public string URL { set; get; }
		public DateTime DateCompleted { set; get; }
		public string Duration { set; get; }
		public string VideoType { set; get; }
		public DateTime UpdatedDate { set; get; }
		public string Addedby { get; set; }
		public DateTime AddedDate { get; set; }


		public string returnMsg { get; set; }




	}
}
