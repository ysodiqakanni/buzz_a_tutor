using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bat.logic.Constants;

namespace bat.Controllers
{
    public class HomeController : Controller
    {
        private readonly logic.Services.AccountInfo _accService;
        private readonly logic.Services.HomePage _homeService;
        private readonly logic.Services.Password _passwdService;

        public HomeController(
            logic.Services.AccountInfo accService,
            logic.Services.HomePage homeService,
            logic.Services.Password passwdService)
        {
            _accService = accService;
            _homeService = homeService;
            _passwdService = passwdService;
        }

        [AllowAnonymous]
        public ActionResult Landing()
        {
            return View(new bat.logic.ViewModels.Homepage.Landing());
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null)
            {
                user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInAdminUser();
                if (user == null)
                    return RedirectToAction("Landing");

                return RedirectToAction("Index", "Admin");
            }

            var model = new bat.logic.ViewModels.Homepage.Dashboard();

            try
            {
                var accInfo = _accService.Get(user.ID);

                if (accInfo.IsTeacher && 
                    (string.IsNullOrEmpty(accInfo.account.Description) || string.IsNullOrEmpty(accInfo.account.Qualifications)))
                {
                    return RedirectToAction("Complete", "Profile");
                }

                model = _homeService.LoadDashboard(accInfo.accountType, accInfo.account.ID);
                model.AccInfo = accInfo;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }

            switch (model.AccInfo.accountType)
            {
                case Types.AccountTypes.Student:
                    return View("DashStudent", model);

                case Types.AccountTypes.Teacher:
                    return View("DashTeacher", model);
            }

            // maybe for admin later
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(
            string type, string firstname, string lastname, string email, string password)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user != null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Homepage.Landing();

            try
            {
                ViewBag.firstName = firstname;
                ViewBag.lastName = lastname;
                ViewBag.email = email;
                model = _homeService.Signup(Convert.ToInt32(type), firstname, lastname, email, password);

                var auth = new logic.Rules.Authentication(Request.GetOwinContext());
                auth.Login(auth.GetUser(email, password));
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Landing", model);
            }
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();

            if (user != null)
                return RedirectToRoute("home");

            return View();
        }

        [AllowAnonymous]
        public ActionResult LoginSignup(string returnUrl)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();

            //So that the user can be referred back to where they were when they click logon
            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);
            if (user != null)
                return RedirectToRoute("home");

            else if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
                ViewBag.ReturnURL = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult login(string txtUsername, string txtPassword,string returnUrl)
        {
            // not already logged in, so continue
            Session.Clear();
            System.Threading.Thread.Sleep(250); // slight delay to deter bots

            try
            {
                var auth = new logic.Rules.Authentication(Request.GetOwinContext());
                auth.Login(auth.GetUser(txtUsername, txtPassword));
            }
            catch (Exception ex)
            {
                ViewBag.LoginErrMsg = ex.Message;
                return View();
            }

            //returnURL needs to be decoded
            string decodedUrl = "";
            if (!string.IsNullOrEmpty(returnUrl))
                decodedUrl = Server.UrlDecode(returnUrl);

            //Login logic...

            if (Url.IsLocalUrl(decodedUrl))
            {
                return Redirect(decodedUrl);
            }
            else
            {
                return RedirectToRoute("home");
            }
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewData.Clear();
            TempData.Clear();
            Session.Clear();
            var auth = new logic.Rules.Authentication(Request.GetOwinContext());
            auth.Logout();

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgotPassword(string txtUsername)
        {
            try
            {
                _passwdService.SetForgotPasswordToken(txtUsername, Request.Url.AbsoluteUri.ToLower().Replace("forgotpassword", "resetpassword"));
                ViewBag.Response = "Please check your email for your password reset link.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrMsg = ex.Message;
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string t)
        {
            var model = new bat.logic.ViewModels.Homepage.ResetPassword();

            try
            {
                ViewData.Clear();
                TempData.Clear();
                Session.Clear();
                var auth = new logic.Rules.Authentication(Request.GetOwinContext());
                auth.Logout();

                model = _passwdService.VerifyToken(t);
            }
            catch (Exception ex)
            {
                ViewBag.Response = ex.Message;
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(string token, string txtPassword1, string txtPassword2)
        {
            var model = new bat.logic.ViewModels.Homepage.ResetPassword();

            try
            {
                model = _passwdService.VerifyToken(token);
                var account = _passwdService.ChangePassword(token, txtPassword1, txtPassword2);

                var auth = new logic.Rules.Authentication(Request.GetOwinContext());
                auth.Login(account);

                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ViewBag.ErrMsg = ex.Message;
            }
            return View(model);
        }

        [Authorize]
        public ActionResult Swap(int id)
        {
            var model = new logic.Rules.Swap();
            
            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model.Load(user, id);
                var auth = new logic.Rules.Authentication(Request.GetOwinContext());
                auth.Login(model.account);
            }
            catch (Exception ex)
            {
                ViewBag.LoginErrMsg = ex.Message;
            }

            return RedirectToRoute("home");
        }

        [Authorize]
        public ActionResult logout()
        {
            ViewData.Clear();
            TempData.Clear();
            Session.Clear();
            var auth = new logic.Rules.Authentication(Request.GetOwinContext());
            auth.Logout();

            return RedirectToRoute("home");
        }

        [AllowAnonymous]
        public ActionResult ListLessons(string subject)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            var model = new bat.logic.ViewModels.Homepage.SubjectList();

            try
            {
                model = _homeService.LoadSubjectList(subject, user?.ID);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult SelectSubject()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ListTeachers(string subject)
        {
            var model = new bat.logic.ViewModels.Homepage.TeacherList();

            try
            {
                model = _homeService.LoadTeacherList(subject);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ListTeacherLessons(int id)
        {
            var model = new bat.logic.ViewModels.Homepage.SubjectList();

            try
            {
                model = _homeService.LoadSubjectListByTeacher(id);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(string type, string firstname, string lastname, string email, string password, string returnUrl)
        {
            try
            {
                ViewBag.firstName = firstname;
                ViewBag.lastName = lastname;
                ViewBag.email = email;
                var rs = _homeService.Signup(Convert.ToInt32(type), firstname, lastname, email, password);

                var auth = new logic.Rules.Authentication(Request.GetOwinContext());
                auth.Login(auth.GetUser(email, password));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("LoginSignup");
            }

            //returnURL needs to be decoded
            string decodedUrl = "";
            if (!string.IsNullOrEmpty(returnUrl))
                decodedUrl = Server.UrlDecode(returnUrl);

            //Login logic...

            if (Url.IsLocalUrl(decodedUrl))
            {
                return Redirect(decodedUrl);
            }
            else
            {
                return RedirectToRoute("home");
            }
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Terms()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Privacy()
        {
            return View();
        }
    }
}