using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using bat.logic.Constants;
using Elmah;
using Microsoft.AspNet.Identity;

namespace bat.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            var model = new bat.logic.Models.Homepage.Dashboard();

            try
            {
                var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return View("Landing");

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
        public ActionResult Login()
        {
            var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();

            if (user != null)
                return RedirectToRoute("home");
            else
                return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult login(string txtUsername, string txtPassword)
        {
            // not already logged in, so continue
            Session.Clear();
            System.Threading.Thread.Sleep(250); // slight delay to deter bots

            try
            {
                var auth = new logic.Models.System.Authentication(Request.GetOwinContext());
                auth.Login(auth.GetUser(txtUsername, txtPassword));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.LoginErrMsg = ex.Message;
                return View();
            }

            return RedirectToRoute("home");
        }

        [Authorize]
        public ActionResult logout()
        {
            ViewData.Clear();
            TempData.Clear();
            Session.Clear();
            var auth = new logic.Models.System.Authentication(Request.GetOwinContext());
            auth.Logout();

            return RedirectToRoute("home");
        }


        [AllowAnonymous]
        public ActionResult TermsAndConditions()
        {
            return View();
        }

        [Authorize]
        public ActionResult Meet()
        {

            var model = new bat.logic.Models.Meet();

            try
            {
                var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model.Initialise(user.ID);
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