using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bat.Controllers
{
    public class ProfileController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
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
        public ActionResult Edit()
        {
            var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
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
         public ActionResult Edit(FormCollection frm)
        {
            var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.Edit();
            try
            {
                model.Initialise(user.ID);
                model.Load(user.ID);
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
        public ActionResult New()
        {
            var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.New();

            try
            {
                model.Initialise(user.ID);
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
            var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.New();
            try
            {
                model.Initialise(user.ID);
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
            var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.EditMember();

            try
            {
                model.Initialise(user.ID);
                model.load(id);
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
            var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.EditMember();
            try
            {
                model.Initialise(user.ID);
                model.Save(frm);
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
            var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.EditMember();

            try
            {
                model.Initialise(user.ID);
                model.Delete(id);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
