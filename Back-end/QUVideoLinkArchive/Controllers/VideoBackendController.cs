using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using QUVideoLinkArchive.Models;
using QUVideoLinkArchive.ViewModel;


namespace QUVideoLinkArchive.Controllers
{

	public class VideoBackendController : Controller
	{

		public static string loginerror = "";
		AccountManagerViewModel curUser = new AccountManagerViewModel();
		Error curPage = new Error();
		login login = new login();
		AccountManager AccountManager = new AccountManager();

		QUToolsUtilities myRequest = new QUToolsUtilities();
		string userdb = "QUTool_BSIC_Vid_Users";


		public ActionResult Index()
		{

			return View();
		}
		[HttpPost]
		public ActionResult Index(login data)
		{
			data.Username = data.Username.ToLower();

			if (data.Password == null)
			{
				TempData["error"] = "No password.";
				return View();
			}
			else if (data.Username == null)
			{
				TempData["error"] = "No Username.";
				return View();
			}

			login checkit = new login(data);

			if (checkit.valid.ToString() == "True")
			{
				return RedirectToAction("MainMenu", "VideoBackend");
			}


			TempData["error"] = "Error. '" + data.Username + "': " + checkit.connectionReturn + " " + loginerror;

			return View();


			//return Content(msg.connectionReturn);//msg.valid.ToString());

		}


		public ActionResult MainMenu()
		{

			//checks if the session id is valid
			bool result = login.checkSession();

			if (result == false)
			{
				TempData["error"] = login.connectionReturn;
				return RedirectToAction("Index", "VideoBackend");
			}

			curUser.username = login.returnUsername();
			ViewData["user"] = curUser.username;
			curUser.userTable = userdb;

			if (result == false) return RedirectToAction("Index", "VideoBackend");
			string error = QUToolsUtilities.QueryString("errors");
			if (error != null) ViewBag.Error = error;
			AccountManagerViewModel sqlStuff = new AccountManagerViewModel();

			sqlStuff = AccountManager.checkUser(curUser);


			if (sqlStuff.userlevel == null)
			{
				curPage.errorMsg = "You do not appear to have rights to access this Tool. ";
				ViewBag.errorMsg = curUser.returnMsg;

				return RedirectToAction("Index", "VideoBackend", curPage);
			}
			if (curUser.Admin != "Yes")

			{
				curPage.errorMsg = "You do not appear to be an Administrator. ";
				//AccountManager curUser = new AccountManager();
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			return View();

		}

		public IActionResult UserPage()
		{
			ViewData["user"] = login.returnUsername();
			// Check Session ID & (eventually all databases for users)
			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				TempData["error"] = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin == "Yes")
			{

				curUser = AccountManager.getUsers(curUser);
				return View(curUser.userlist);
			}
			else
			{
				TempData["error"] = "You do not appear to be an Administrator. ";
				return RedirectToAction("Index", "VideoBackend");
			}
		}

		[HttpPost]
		public IActionResult UserPage(string QU_User, string QUID, string Admin, string PlacementLang, string PlacementMath, string PlacementMathCounts, string PlacementMathQuery)
		{
			ViewData["user"] = login.returnUsername();
			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				ViewBag.Msg = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);

			if (QU_User == "")
			{
				ViewBag.Msg = "Please enter QU Username";
				return View();
			}

			if (QUID == "")
			{
				ViewBag.Msg = "Please enter QU ID";
				return View();
			}
			AccountManager newuser = new AccountManager();



			curUser.username = QU_User;
			curUser.quID = QUID;
			curUser.Admin = Admin;
			//curUser.userlevel = PX_Tools.PX_UserLevelReverse(PX_Attributes);

			Console.Write("d");

			curUser = AccountManager.addUser(curUser);

			curUser = AccountManager.getUsers(curUser);
			ViewBag.Msg = curUser.returnMsg;
			return View(curUser.userlist);
		}
		public IActionResult Delete()
		{
			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				ViewBag.error = "Still A viewbag CHANGE";
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin == "Yes")
			{

				ViewBag.username = QUToolsUtilities.QueryString("uid");
				return View();
			}
			else
			{
				curPage.errorMsg = "You do not appear to be an Administrator. ";
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

		}

		[HttpPost]
		public IActionResult Delete(string deluser)
		{
			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				TempData["error"] = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin == "Yes")
			{

				curPage.errorMsg = AccountManager.delUser(deluser, curUser.userTable);
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("UserPage", "VideoBackend", curPage);
			}
			else
			{
				curPage.errorMsg = "You do not appear to be an Administrator. ";
				//AccountManager curUser = new AccountManager();
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}
		}


		public IActionResult AddLink()
		{
			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				TempData["error"] = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin != "Yes")
			{
				curPage.errorMsg = "An error has occured, please try again. ";
				//AccountManager curUser = new AccountManager();
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			VideoBackendViewModel taglist = new VideoBackendViewModel();
			VideoBackEnd VBM = new VideoBackEnd();

			taglist = VBM.getAllTags(taglist);


			return View(taglist);
		}

		[HttpPost]
		public IActionResult AddLink(string alltags, string title, string URL, string duration, DateTime datecompleted, string videotype, string description)
		{

			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				TempData["error"] = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			bool _test1 = String.IsNullOrEmpty(title);
			bool _test2 = String.IsNullOrEmpty(URL);
			bool _test3 = String.IsNullOrEmpty(duration);
			bool _test4 = String.IsNullOrEmpty(videotype);
			bool _test5 = String.IsNullOrEmpty(description);
				
			if (_test1 || _test2 || _test3 || _test4 || _test5)
			{
				TempData["error"] = "It appears you missed a field.  Make sure all fields are filled out";
				return RedirectToAction("AddLink", "VideoBackend");
			}
			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin != "Yes")
			{
				curPage.errorMsg = "An error has occured, please try again. ";
				//AccountManager curUser = new AccountManager();
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}


			VideoBackendViewModel videoMetaData = new VideoBackendViewModel();
			VideoBackEnd VBM = new VideoBackEnd();
			string[] tagArray;
			videoMetaData.VideoName = title;
			videoMetaData.URL = URL;
			videoMetaData.Duration = duration;
			videoMetaData.DateCompleted = datecompleted;
			videoMetaData.VideoType = videotype;
			videoMetaData.VideoDescription = description;
			tagArray = VBM.parseTags(alltags);

			string x = VBM.processEntry(videoMetaData, tagArray, curUser.username);

			TempData["error"] = x;


			return RedirectToAction("AddLink", "VideoBackend", curPage);
		}

		public IActionResult ListLinks()
		{

			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				TempData["error"] = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin != "Yes")
			{
				curPage.errorMsg = "An error has occured, please try again. ";
				//AccountManager curUser = new AccountManager();
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			VideoBackendViewModel videodata = new VideoBackendViewModel();
			VideoBackEnd videomethods = new VideoBackEnd();

			videodata = videomethods.listVideos("all");
			TempData["error"] = videodata.returnMsg;
			return View(videodata);

		}

		[HttpGet]
		public IActionResult EditLink(string id)
		{

			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				TempData["error"] = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin != "Yes")
			{
				curPage.errorMsg = "An error has occured, please try again. ";
				//AccountManager curUser = new AccountManager();
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			VideoBackendViewModel videodata = new VideoBackendViewModel();
			VideoBackEnd videomethods = new VideoBackEnd();


			videodata = videomethods.listVideos(id);
			videodata = videomethods.getAllTags(videodata);
			videodata.vidID = id;
			videodata = videomethods.getTagsForVideo(videodata);
			TempData["error"] = videodata.returnMsg;
			return View(videodata);
		}

		[HttpPost]
		public IActionResult EditLink(string alltags, string vidID, string title, string URL, string duration, DateTime datecompleted, string videotype, string description, string delete)
		{


			bool result = login.checkSession();

			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				TempData["error"] = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin != "Yes")
			{
				curPage.errorMsg = "An error has occured, please try again. ";
				//AccountManager curUser = new AccountManager();
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			VideoBackendViewModel videoMetaData = new VideoBackendViewModel();
			VideoBackEnd VBM = new VideoBackEnd();
			try
			{

				string[] tagArray;
				videoMetaData.VideoName = title;
				videoMetaData.URL = URL;
				videoMetaData.Duration = duration;
				videoMetaData.DateCompleted = datecompleted;
				videoMetaData.VideoType = videotype;
				videoMetaData.VideoDescription = description;
				videoMetaData.vidID = vidID;
				tagArray = VBM.parseTags(alltags);



				string y = VBM.cleanLinkEntrys(vidID);

				if (delete == "1")
				{
					TempData["error"] = y;
					return RedirectToAction("ListLinks", "VideoBackend");
				}

				if (y == "Success")
				{
					string x = VBM.processEntry(videoMetaData, tagArray, curUser.username);

			
						TempData["error"] = x;
				
				}
				return RedirectToAction("ListLinks", "VideoBackend", curPage);
			}
			catch (Exception e)
			{
				TempData["error"] = e.Message;
				return RedirectToAction("ListLinks", "VideoBackend", curPage);
			}
		}

		public IActionResult addTag()
		{
			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				TempData["error"] = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin != "Yes")
			{
				curPage.errorMsg = "An error has occured, please try again. ";
				//AccountManager curUser = new AccountManager();
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			VideoBackendViewModel videodata = new VideoBackendViewModel();
			VideoBackEnd videomethods = new VideoBackEnd();

			videodata = videomethods.getAllTags(videodata);

			return View(videodata);
		}

		[HttpPost]
		public IActionResult addTag(string alltags, string delTags)
		{
			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				TempData["error"] = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin != "Yes")
			{
				curPage.errorMsg = "An error has occured, please try again. ";
				//AccountManager curUser = new AccountManager();
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			VideoBackendViewModel videoMetaData = new VideoBackendViewModel();
			VideoBackEnd VBM = new VideoBackEnd();


			if (delTags != "na")
			{

				string[] tags = delTags.Split('|');

				foreach (var t in tags)
				{
					if (t != "")
						VBM.deletetag(t);

				}
				TempData["error"] = "Deleted Tags";
				return RedirectToAction("MainMenu", "VideoBackend", curPage);
			}


			if (alltags == "na")
			{
				TempData["error"] = "No tags entered...";
				return RedirectToAction("MainMenu", "VideoBackend", curPage);
			}


			string[] tagArray;

			tagArray = VBM.parseTags(alltags);
			string x = VBM.processNewTags(tagArray);
			TempData["error"] = "Success";
			if (x != "Success")
			{
				TempData["error"] = "Unable to add. " + x;
			}

			return RedirectToAction("MainMenu", "VideoBackend", curPage);
		}

		public IActionResult RenameTags()
		{
			
			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				TempData["error"] = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin != "Yes")
			{
				curPage.errorMsg = "An error has occured, please try again. ";
				//AccountManager curUser = new AccountManager();
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}


			VideoBackendViewModel videodata = new VideoBackendViewModel();
			VideoBackEnd videomethods = new VideoBackEnd();

			videodata = videomethods.jayAllTags(videodata);

			return View(videodata);
		}

		[HttpPost]
		public IActionResult RenameTags(string renameID, string rename_tag)
		{
			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				TempData["error"] = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin != "Yes")
			{
				curPage.errorMsg = "An error has occured, please try again. ";
				//AccountManager curUser = new AccountManager();
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}


			VideoBackendViewModel videodata = new VideoBackendViewModel();
			VideoBackEnd videomethods = new VideoBackEnd();
			
			videomethods.renametag(renameID, rename_tag);
			videodata = videomethods.jayAllTags(videodata);
			return View(videodata);
		}
		// JAY Tools //////////////////////////////////////////////////////////////////////////////////////////////////
		public IActionResult jayAdmin()
		{

			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				TempData["error"] = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin != "Yes" && curUser.username == "jamunro")
			{
				curPage.errorMsg = "An error has occured, please try again. ";
				//AccountManager curUser = new AccountManager();
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}


			VideoBackendViewModel videodata = new VideoBackendViewModel();
			VideoBackEnd videomethods = new VideoBackEnd();

			videodata = videomethods.jayAllTags(videodata);
			
			return View(videodata);

		}

		[HttpPost]
		public IActionResult jayAdmin(string deleteThese)
		{
			#region login

			bool result = login.checkSession();
			if (result == false)
			{
				curPage.errorMsg = "Either you do not have access to these tools or your session has timed out...";
				TempData["error"] = curUser.returnMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}

			AccountManager sqlStuff = new AccountManager();

			login userInformation = new login();
			curUser.username = login.returnUsername();
			curUser.userTable = userdb;
			curUser = AccountManager.checkAdmin(curUser);



			if (curUser.Admin != "Yes" && curUser.username == "jamunro")
			{
				curPage.errorMsg = "An error has occured, please try again. ";
				//AccountManager curUser = new AccountManager();
				TempData["error"] = curPage.errorMsg;
				return RedirectToAction("Index", "VideoBackend", curPage);
			}


			#endregion


			VideoBackendViewModel videodata = new VideoBackendViewModel();
			VideoBackEnd videomethods = new VideoBackEnd();



			string[] tags = deleteThese.Split('|');

			foreach (var t in tags)
			{
				if (t != "")
					videomethods.deletetag(t);

			}
			TempData["error"] = " Deleted " + deleteThese;

			videodata = videomethods.jayAllTags(videodata);
			return View(videodata);

		}

	}
}