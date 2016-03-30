﻿using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bat.Controllers
{
    public class LessonsController : Controller
    {
        [Authorize]
        public ActionResult Index(int id)
        {
            var model = new bat.logic.Models.Lessons.View();

            try
            {
                var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model.Initialise(user.ID);
                //if (!model.IsTeacher) return RedirectToRoute("home");
                model.Load(id);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return View("View", model); // TokBox view
            //return View("ViewZoom", model); // Zoom view
        }

        [Authorize]
        public ActionResult New()
        {
            var model = new bat.logic.Models.Lessons.New();

            try
            {
                var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model.Initialise(user.ID);
                if (!model.IsTeacher) return RedirectToRoute("home");
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
            var model = new bat.logic.Models.Lessons.New();

            try
            {
                var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model.Initialise(user.ID);
                if (!model.IsTeacher) return RedirectToRoute("home");
                model.Save(frm);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return RedirectToAction("Index", new { id = model.lesson.ID });
        }

        [Authorize]
        public ActionResult Join(int id)
        {
            var model = new bat.logic.Models.Lessons.View();

            try
            {
                var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model.Initialise(user.ID);
                model.Load(id);
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
        public ActionResult Join(int id, FormCollection frm)
        {
            var model = new bat.logic.Models.Lessons.View();

            try
            {
                var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model.Initialise(user.ID);
                model.Load(id);
                model.Save(frm);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return RedirectToAction("Index", new { id = model.lesson.ID });
        }


        [Authorize]
        [HttpPost]
        public ActionResult Upload(int id, HttpPostedFileBase data, string title)
        {
            var model = new bat.logic.Models.Lessons.View();

            try
            {
                var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model.Initialise(user.ID);
                model.UploadImage(title, data);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return RedirectToAction("Index", new { id = id });
        }
    }
}