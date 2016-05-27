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

namespace bat.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return View("Landing", new bat.logic.ViewModels.Homepage.Landing());

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
                model.Signup(type, firstname, lastname, email, password);

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
        public ActionResult Login(string returnUrl)
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
        public ActionResult login(string txtUsername, string txtPassword, string returnUrl)
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
        public ActionResult SubjectList(string subject)
        {
            var model = new bat.logic.ViewModels.Homepage.SubjectList();

            try
            {
                model.Load(subject);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            var model = new bat.logic.ViewModels.Homepage.Register();

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(string type, string firstname, string lastname, string email, string password, string returnUrl)
        {
            var model = new bat.logic.ViewModels.Homepage.Register();

            try
            {
                model.Signup(type, firstname, lastname, email, password);

                var auth = new logic.Rules.Authentication(Request.GetOwinContext());
                auth.Login(auth.GetUser(email, password));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
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

        //[HttpPost]
        //public ActionResult Join(int id)
        //{
        //Check if user is logged in
        //If logged they join the lessons

        //If not logged in - sent to log in
        //If user logs in they join the lesson

        //if not as register user - sent to register page
        //If user registers they are able to join that lesson.
        //}
    }
}