﻿using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bat.logic.Exceptions;

namespace bat.Controllers
{
    public class ProfileController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.Profile();

            try
            {
                model.Initialise(user.ID);
                model.Load(user.ID);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [Authorize]
        public ActionResult Complete()
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.Edit();

            try
            {
                model.Initialise(user.ID);
                model.Load(user.ID);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [Authorize]
        public ActionResult Edit()
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.Edit();

            try
            {
                model.Initialise(user.ID);
                model.Load(user.ID);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [Authorize]
        [HttpPost] 
         public ActionResult Edit(string FirstName, string LastName, string Description, string Qualifications, int? Rate, HttpPostedFileBase Picture)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.Edit();
            try
            {
                model.Initialise(user.ID);
                model.Load(user.ID);
                model.Save(user.ID, FirstName, LastName, Description, Qualifications, Rate, Picture);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }
            return View(model);
        }

        [Authorize]
        public ActionResult New()
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.New();

            try
            {
                model.Initialise(user.ID);
                if (model.IsTeacher) return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult New(FormCollection frm)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.New();
            try
            {
                model.Initialise(user.ID);
                if (model.IsTeacher) return RedirectToAction("Index");

                model.Save(user.ID, frm);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }
            return View(model);
        }

        [Authorize]
        public ActionResult EditMember(int id)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.EditMember();

            try
            {
                model.Initialise(user.ID);
                if (model.IsTeacher) return RedirectToAction("Index");

                model.load(id);
            }
            catch (WrongAccountException)
            {
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditMember(FormCollection frm)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.EditMember();
            try
            {
                model.Initialise(user.ID);
                if (model.IsTeacher) RedirectToAction("Index");

                model.Save(frm);
                return RedirectToAction("Index");
            }
            catch (WrongAccountException)
            {
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }
            return View(model);
        }

        [Authorize]
        public ActionResult DeleteMember(int id)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.EditMember();

            try
            {
                model.Initialise(user.ID);
                if (model.IsTeacher) return RedirectToAction("Index");

                model.Delete(id);
            }
            catch (WrongAccountException)
            {
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return RedirectToAction("Index");
        }

        public ActionResult ProfilePicture(int id)
        {
            var imageData = logic.ViewModels.Profile.Profile.GetProfilePicture(id);

            return File(imageData, "image/jpg");
        }

        [Authorize]
        public ActionResult Lessons()
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.Lessons();

            try
            {
                model.Initialise(user.ID);
                model.Load(user.ID);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [Authorize]
        public ActionResult LessonDetails(int id)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.LessonDetails();

            try
            {
                model.Initialise(user.ID);
                model.Load(id);
            }
            catch (WrongAccountException)
            {
                return RedirectToRoute("home");
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
