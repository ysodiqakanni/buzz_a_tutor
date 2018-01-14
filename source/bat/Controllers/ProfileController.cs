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
        private readonly logic.Services.Profile _profileService;

        public ProfileController(
            logic.Services.Profile profileService)
        {
            _profileService = profileService;
        }

        [Authorize]
        public ActionResult Index()
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.Profile.Profile();

            try
            {
                model.AccInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
            }
            catch (Exception ex)
            {
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
                model = _profileService.LoadEdit(user.ID);
            }
            catch (Exception ex)
            {
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
                model = _profileService.LoadEdit(user.ID);
            }
            catch (Exception ex)
            {
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
                model = _profileService.ProfileEditSave(
                    user.ID, 
                    FirstName, 
                    LastName, 
                    Description, 
                    Qualifications, 
                    Rate, 
                    Picture);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
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
                var accInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
                if (accInfo.IsTeacher) return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
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
                var accInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
                if (accInfo.IsTeacher) return RedirectToAction("Index");

                model = _profileService.SaveNew(user.ID, frm);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
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
                var accInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
                if (accInfo.IsTeacher) return RedirectToAction("Index");

                model = _profileService.LoadEditMember(id, user.ID);
            }
            catch (WrongAccountException)
            {
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
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
                var accInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
                if (accInfo.IsTeacher) RedirectToAction("Index");

                model = _profileService.SaveEditMember(frm, user.ID);
                return RedirectToAction("Index");
            }
            catch (WrongAccountException)
            {
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
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
                var accInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
                if (accInfo.IsTeacher) RedirectToAction("Index");

                model = _profileService.DeleteEditMember(id, user.ID);
            }
            catch (WrongAccountException)
            {
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return RedirectToAction("Index");
        }

        public ActionResult ProfilePicture(int id)
        {
            var imageData = _profileService.GetProfilePicture(id);

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
                model = _profileService.LoadLessons(user.ID);
            }
            catch (Exception ex)
            {
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
                model = _profileService.LoadLessonDetails(id, user.ID);
            }
            catch (WrongAccountException)
            {
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }
    }
}
