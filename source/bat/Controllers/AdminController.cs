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
        private readonly logic.Services.Admin _adminService;

        public AdminController(logic.Services.Admin adminService)
        {
            _adminService = adminService;
        }

        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult Edit(string subject)
        {
            var model = new logic.ViewModels.Admin.Edit();
            
            try
            {
                model = _adminService.LoadEditViewModel(subject);

                return View(model);
            }
            catch (Exception ex)
            {
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
                _adminService.Save(subject, txtDescription);
                model = _adminService.LoadEditViewModel(subject);

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult UploadExamPaper(int id, HttpPostedFileBase ExamPaper)
        {
            var model = _adminService.LoadEditViewModel(id);
            _adminService.UploadPaper(model.subjectDescription.ID, ExamPaper);

            return RedirectToAction("Edit", new { subject = model.subjectDescription.Subject });
        }

        public ActionResult DeleteExamPaper(int id)
        {
            var model = new logic.ViewModels.Admin.Edit();
            _adminService.DeletePaper(id);

            return RedirectToAction("Edit", new { subject = model.subjectDescription.Subject });
        }

        public ActionResult Tutors()
        {
            var model = new logic.ViewModels.Admin.Tutors();

            try
            {
                model = _adminService.LoadTutors();

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult TutorStatus(int id, bool status)
        {
            var model = new logic.ViewModels.Admin.Tutors();

            try
            {
                _adminService.SetAccountStatus(id, status);
                return RedirectToAction("TeacherProfile", "Admin", new { id = id });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult Approve(int id, bool status)
        {
            var model = new logic.ViewModels.Admin.Tutors();

            try
            {
                _adminService.ApproveAccount(id, status);
                return RedirectToAction("TeacherProfile", "Admin", new { id = id });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }


        public ActionResult Lessons()
        {
            var model = new logic.ViewModels.Admin.Lessons();

            try
            {
                model = _adminService.LoadLessons();

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult Lesson(int id)
        {
            var model = new logic.ViewModels.Admin.Lesson();

            try
            {
                model = _adminService.LoadLesson(id);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult LessonVisibility(int id, bool status)
        {
            var model = new logic.ViewModels.Admin.Lesson();

            try
            {
                _adminService.SetLessonVisibility(id, status);
                return RedirectToAction("Lesson", "Admin", new { id = id });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult Students()
        {
            var model = new logic.ViewModels.Admin.Students();

            try
            {
                model = _adminService.LoadStudents();

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult StudentStatus(int id, bool status)
        {
            var model = new logic.ViewModels.Admin.Students();

            try
            {
                _adminService.SetAccountStatus(id, status);
                return RedirectToAction("Students", "Admin");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        public ActionResult TeacherProfile(int id)
        {
            var model = new logic.ViewModels.Admin.TeacherProfile();

            try
            {
                model = _adminService.LoadTeacherProfile(id);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }
    }
}