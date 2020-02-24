using System;
using System.Collections.Generic;

namespace LinkArchiveToolFrontEnd.ViewModel
{
    public class AccountManagerViewModel
    {

        public string Username { set; get; }
        public string QuId { get; set; }
        public string UserTable { get; set; }
        public string UserComments { get; set; }
        public string ReturnMsg { get; set; }
        public string Admin { get; set; }
        public string Userlevel { get; set; }
        public List<AccountManagerViewModel> Userlist { get; set; }
        // for the list of users 
        public string ListUser { get; set; }
        public string ListQuid { get; set; }
        public string ListAdmin { get; set; }
        public string ListUserlevel { get; set; }

    }
}
