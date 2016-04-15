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
        // GET: Profile
        [AllowAnonymous]
        public ActionResult Index()
        {
            var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return View("Landing", new bat.logic.Models.Homepage.Landing());

            var model = new bat.logic.Models.Profile.Profile();

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

        [AllowAnonymous]
        public ActionResult Edit()
        {
            var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return View("Landing", new bat.logic.Models.Homepage.Landing());

            var model = new bat.logic.Models.Profile.Edit();

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
            if (user == null) return View("Landing", new bat.logic.Models.Homepage.Landing());

            var model = new bat.logic.Models.Profile.Edit();
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

        [AllowAnonymous]
        public ActionResult EditPassword()
        {
            var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return View("Landing", new bat.logic.Models.Homepage.Landing());

            var model = new bat.logic.Models.Profile.EditPassword();

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
        public ActionResult EditPassword(FormCollection frm)
        {
            var user = new logic.Models.System.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return View("Landing", new bat.logic.Models.Homepage.Landing());

            var model = new bat.logic.Models.Profile.EditPassword();
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
    }
}
