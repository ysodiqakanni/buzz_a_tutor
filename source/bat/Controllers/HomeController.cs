using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using bat.logic.Constants;
using Elmah;
using Microsoft.AspNet.Identity;
using bat.data;
using bat.logic.ViewModels.Homepage;

namespace bat.Controllers
{
    public class HomeController : Controller
    {
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
                model.Initialise(user.ID);
                model.Load();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            switch (model.accountType)
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
                model.Signup(Convert.ToInt32(type), firstname, lastname, email, password);

                var auth = new logic.Rules.Authentication(Request.GetOwinContext());
                auth.Login(auth.GetUser(email, password));
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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
                ErrorSignal.FromCurrentContext().Raise(ex);
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
                var model = new bat.logic.ViewModels.Homepage.ForgotPassword();
                model.SetToken(txtUsername, Request.Url.AbsoluteUri.ToLower().Replace("forgotpassword", "resetpassword"));
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

                model.VerifyToken(t);
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
                var account = model.ChangePassword(token, txtPassword1, txtPassword2);

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
                ErrorSignal.FromCurrentContext().Raise(ex);
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
        public ActionResult TermsAndConditions()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ListLessons(string subject)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            var model = new bat.logic.ViewModels.Homepage.SubjectList();

            try
            {
                if (user != null)
                    model.Initialise(user.ID);

                model.Load(subject, user?.ID);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult SelectSubject()
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            var model = new bat.logic.ViewModels.Master();

            try
            {
                if (user != null)
                    model.Initialise(user.ID);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(string type, string firstname, string lastname, string email, string password, string returnUrl)
        {
            var model = new bat.logic.ViewModels.Homepage.Register();

            try
            {
                ViewBag.firstName = firstname;
                ViewBag.lastName = lastname;
                ViewBag.email = email;
                model.Signup(Convert.ToInt32(type), firstname, lastname, email, password);

                var auth = new logic.Rules.Authentication(Request.GetOwinContext());
                auth.Login(auth.GetUser(email, password));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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

        [AllowAnonymous]
        public ActionResult ListTeachers(string subject)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            var model = new bat.logic.ViewModels.Homepage.TeacherList();

            try
            {
                if (user != null)
                    model.Initialise(user.ID);

                model.Load(subject);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }
    }
}