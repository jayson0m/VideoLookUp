using System;
using Microsoft.AspNetCore.Mvc;
using LinkArchiveToolFrontEnd.Models;
using LinkArchiveToolFrontEnd.ViewModel;

namespace LinkArchiveToolFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        readonly AccountManagerViewModel _curUser = new AccountManagerViewModel();
     
        Login _login = new Login();
        string _userdb = "QUTool_BSIC_Vid_Users";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Login data)
        {
            data.Username = data.Username.ToLower();

            if (data.Password == null)
            {
                TempData["error"] = "No password.";
                return View();
            }

            if (data.Username == null)
            {
                TempData["error"] = "No Username.";
                return View();
            }

            Login checkit = new Login(data);

            if (checkit.Valid.ToString() == "True")
            {
                return RedirectToAction($"SearchPage", $"Home");
            }

            TempData["error"] = "Error. " + checkit.ConnectionReturn;

            return View();
        }

        public IActionResult SearchPage()
        {
            //checks if the session id is valid
            bool result = _login.CheckSession();

            if (result == false)
            {
                TempData["error"] = _login.ConnectionReturn;
                return RedirectToAction($"Index", $"Home");
            }

            _curUser.Username = _login.ReturnUsername();
            ViewData["user"] = _curUser.Username;
            _curUser.UserTable = _userdb;

            string error = QuToolsUtilities.QueryString("errors");
            if (error != null) ViewBag.Error = error;

            HomeViewModel allTags = new HomeViewModel();
            Home tagMethods = new Home(); 

            allTags = tagMethods.GetAllTags(allTags);

            allTags.ReturnMsg = "";

            return View(allTags); 
        }

        [HttpPost]
        public IActionResult SearchPage(string videoType, DateTime dateFrom, DateTime dateTo, string alltags, string searchMe)
        {
            //checks if the session id is valid
            bool result = _login.CheckSession();

            if (result == false)
            {
                TempData["error"] = _login.ConnectionReturn;
                return RedirectToAction($"Index", $"Home");
            }

            _curUser.Username = _login.ReturnUsername();
            ViewData["user"] = _curUser.Username;
            _curUser.UserTable = _userdb;

            string error = QuToolsUtilities.QueryString("errors");
            if (error != null) ViewBag.Error = error;

            HomeViewModel searchInfo = new HomeViewModel();
            Home searchMethods = new Home();

            searchInfo.SearchVideoType = videoType;
            searchInfo.SearchDateFrom = dateFrom;
            searchInfo.SearchDateTo = dateTo;
            searchInfo.SearchRawTags = alltags;
            searchInfo.SearchString = searchMe;

            searchInfo = searchMethods.Search(searchInfo);

            searchInfo = searchMethods.GetAllTags(searchInfo);

            string csv = searchMethods.Convert2Csv(searchInfo);

            searchInfo.CsvResults = csv;

            searchInfo.ReturnMsg = "results";

            return View(searchInfo);
        }

        public IActionResult CsvForm()
        {
            return RedirectToAction($"SearchPage", $"Home");
        }

        [HttpPost]
        public IActionResult CsvForm (string rawdata)
        {
            //checks if the session id is valid
            bool result = _login.CheckSession();

            if (result == false)
            {
                TempData["error"] = _login.ConnectionReturn;
                return RedirectToAction($"Index", $"Home");
            }

            _curUser.Username = _login.ReturnUsername();
            ViewData["user"] = _curUser.Username;
            _curUser.UserTable = _userdb;

            string error = QuToolsUtilities.QueryString("errors");
            if (error != null) ViewBag.Error = error;

            string csv = rawdata;
            string filename = "Results_{0}.csv";

            filename = String.Format(filename, DateTime.Now.ToString("HHmmMMddyy"));
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", filename);
        }
    }
}