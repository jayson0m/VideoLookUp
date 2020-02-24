using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUVideoLinkArchive.ViewModel
{
    public class AccountManagerViewModel
    {
		public string username { set; get; }
		public string quID { get; set; }
		public string userTable { get; set; }
		public string userComments { get; set; }
		public string returnMsg { get; set; }
		public string Admin { get; set; }
		public string userlevel { get; set; }
		public List<AccountManagerViewModel> userlist { get; set; }
		// for the list of users 
		public string list_user { get; set; }
		public string list_QUID { get; set; }
		public string list_admin { get; set; }
		public string list_userlevel { get; set; }
	}
}
