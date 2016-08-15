using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bat.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInAdminUser();
                if (user == null) return RedirectToRoute("home");

                return View();
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult Edit(string subject)
        {
            var model = new logic.ViewModels.Admin.Edit();
            
            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInAdminUser();
                if (user == null) return RedirectToRoute("home");

                model.Load(subject);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(string subject, string txtDescription)
        {
            var model = new logic.ViewModels.Admin.Edit();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInAdminUser();
                if (user == null) return RedirectToRoute("home");

                model.Load(subject);
                model.Save(txtDescription);
                model.Load(subject);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadExamPaper(int id, HttpPostedFileBase ExamPaper)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInAdminUser();
            if (user == null) return RedirectToRoute("home");

            var model = new logic.ViewModels.Admin.Edit();
            model.Load(id);
            model.UploadPaper(ExamPaper);

            return RedirectToAction("Edit", new { subject = model.subjectDescription.Subject });
        }

        public ActionResult DeleteExamPaper(int id)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInAdminUser();
            if (user == null) return RedirectToRoute("home");

            var model = new logic.ViewModels.Admin.Edit();
            model.DeletePaper(id);

            return RedirectToAction("Edit", new { subject = model.subjectDescription.Subject });
        }

        public ActionResult DownloadExamPaper(int id, string name)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInAdminUser();
            if (user == null) return RedirectToRoute("home");

            var model = new logic.ViewModels.Admin.Edit();

            return File(model.DownloadPaper(id).ToArray(), "application", name);
        }
    }
}