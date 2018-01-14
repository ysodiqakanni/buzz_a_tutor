using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bat.logic.Constants;
using bat.logic.Exceptions;

namespace bat.Controllers
{
    public class LessonsController : Controller
    {
        private readonly logic.Services.Lessons _lessonsService;

        public LessonsController(
            logic.Services.Lessons lessonsService)
        {
            _lessonsService = lessonsService;
        }

        [Authorize]
        public ActionResult Index(int id)
        {
            var model = new bat.logic.ViewModels.Lessons.View();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model = _lessonsService.LoadView(id, user.ID);

                if (model.lesson.CancelledDate.HasValue)
                {
                    return View("CancelledLesson", model);
                }
                
                if (logic.Helpers.WebRTC.WebRTCAvailable())
                {
                    model = _lessonsService.GenerateTokBoxToken(model, user.ID, user.Email);
                    return View("ViewTokBox", model);
                }
                else
                {
                    return View("WebRTCUnavailable", model);
                }

                // no longer using zoom
                //if (!model.CurrentlyAZoomUser)
                //{
                //    model.CheckZoomUser();
                //    return View("NewZoomUserAccount", model);
                //}

                //switch (model.accountType)
                //{
                //    case Types.AccountTypes.Student:
                //        if (!model.LessonReady)
                //            return View("ZoomLessonHostNotReady", model);
                //        break;

                //    case Types.AccountTypes.Teacher:
                //        model.CreateZoomMeeting();
                //        return View("ViewZoom", model);

                //    default:
                //        throw new Exception("Invalid account type.");
                //}
            }
            //catch (ZoomException ex) when (ex.Code == bat.logic.Constants.Zoom.ErrorCode_CannotCreateMeeting)
            //{
            //    return View("NewZoomUserAccount", model);
            //}
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error", model);
            }
        }

        [Authorize]
        public ActionResult New(string date)
        {
            var model = new bat.logic.ViewModels.Lessons.New();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                var accInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
                if (!accInfo.IsTeacher || !accInfo.IsEnabled) return RedirectToRoute("home");

                model = _lessonsService.LoadNew(date);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            
            return View(model);
        }

        [Authorize]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult New(FormCollection frm)
        {
            var model = new bat.logic.ViewModels.Lessons.New();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                var accInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
                if (!accInfo.IsTeacher || !accInfo.IsEnabled) return RedirectToRoute("home");

                model = _lessonsService.SaveNew(frm, user.ID);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index", new { id = model.lesson.ID });
        }

        [Authorize]
        public ActionResult Join(int id)
        {
            var model = new bat.logic.ViewModels.Lessons.Join();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model = _lessonsService.LoadJoin(id, user.ID);
                
                if (!model.CanContinue)
                    return RedirectToAction("Index", "Lessons", new { id = id });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Join(int id, FormCollection frm)
        {
            var model = new bat.logic.ViewModels.Lessons.Join();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model = _lessonsService.LoadJoin(id, user.ID);

                if (!model.CanContinue)
                    return RedirectToAction("Index", "Lessons", new { id = id });

                _lessonsService.SaveJoin(id, user.ID);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return RedirectToAction("Index", new { id = model.lesson.ID });
        }

        [Authorize]
        public ActionResult Leave(int lessonid, int studentid)
        {
            var model = new bat.logic.ViewModels.Lessons.Leave();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model = _lessonsService.LoadLeave(lessonid, user.ID);

                if (!model.CanContinue)
                    return RedirectToAction("Index", "Lessons", new { id = lessonid });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(model);
        }


        [Authorize]
        [HttpPost]
        public ActionResult Leave(int lessonid, int studentid, FormCollection frm)
        {
            var model = new bat.logic.ViewModels.Lessons.Leave();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model = _lessonsService.LoadLeave(lessonid, user.ID);

                if (!model.CanContinue)
                    return RedirectToAction("Index", "Lessons", new { id = lessonid });

                _lessonsService.SaveLeave(lessonid, studentid);

                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return RedirectToRoute("home");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Upload(int id, HttpPostedFileBase data, string title)
        {
            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                _lessonsService.UploadImage(id, title, data, user.ID);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return RedirectToAction("Index", new { id = id });
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var model = new logic.ViewModels.Lessons.Edit();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                var accInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
                if (!accInfo.IsTeacher) return RedirectToRoute("home");

                model = _lessonsService.LoadEdit(id, user.ID);

                if (model.lesson.CancelledDate.HasValue)
                {
                    return RedirectToAction("Index", new {id = id});
                }

                return View(model);
            }
            catch (WrongAccountException)
            {
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error", model);
            }
        }

        [Authorize]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Edit(int id, FormCollection frm)
        {
            var model = new logic.ViewModels.Lessons.Edit();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                var accInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
                if (!accInfo.IsTeacher) return RedirectToRoute("home");

                model = _lessonsService.LoadEdit(id, user.ID);
                _lessonsService.SaveEdit(id, user.ID, frm);
            }
            catch (WrongAccountException)
            {
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error", model);
            }

            return RedirectToAction("Index", new { id = model.lesson.ID });
        }

        [Authorize]
        public ActionResult Reschedule(int id)
        {
            var model = new logic.ViewModels.Lessons.Reschedule();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                var accInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
                if (!accInfo.IsTeacher) return RedirectToRoute("home");

                model = _lessonsService.LoadReschedule(id, user.ID);

                if (model.lesson.CancelledDate.HasValue)
                {
                    return RedirectToAction("Index", new { id = id });
                }

                return View(model);
            }
            catch (WrongAccountException)
            {
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error", model);
            }
        }

        [Authorize]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Reschedule(int id, FormCollection frm)
        {
            var model = new logic.ViewModels.Lessons.Reschedule();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                var accInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
                if (!accInfo.IsTeacher) return RedirectToRoute("home");

                model = _lessonsService.SaveReschedule(id, user.ID, frm);
            }
            catch (WrongAccountException)
            {
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error", model);
            }

            return RedirectToAction("Index", new { id = model.lesson.ID });
        }

        [Authorize]
        public ActionResult Cancel(int id)
        {
            var model = new logic.ViewModels.Lessons.Cancel();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                var accInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
                if (!accInfo.IsTeacher) return RedirectToRoute("home");

                model = _lessonsService.LoadCancel(id, user.ID);

                if (model.lesson.CancelledDate.HasValue)
                {
                    return RedirectToAction("Index", new { id = id });
                }

                return View(model);
            }
            catch (WrongAccountException)
            {
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error", model);
            }
        }

        [Authorize]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Cancel(int id, FormCollection frm)
        {
            var model = new logic.ViewModels.Lessons.Cancel();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                var accInfo = logic.Helpers.UserAccountInfo.Get(user.ID);
                if (!accInfo.IsTeacher) return RedirectToRoute("home");

                model = _lessonsService.LoadCancel(id, user.ID);
                _lessonsService.SaveCancel(id, user.ID, frm);
            }
            catch (WrongAccountException)
            {
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error", model);
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult UploadResource(int id, HttpPostedFileBase ClassResource)
        {
            logic.Rules.ResourceManagement.UploadResource(id, ClassResource);
            return RedirectToAction("Edit", new { id = id });
        }

        public ActionResult DownloadResource(int id, string name)
        {
            return File(logic.Rules.ResourceManagement.DownloadLessonResource(id).ToArray(), "application", name);
        }

        public ActionResult DeleteResource(int id, int resourceId)
        {
            logic.Rules.ResourceManagement.DeleteResource(resourceId);
            return RedirectToAction("Index", new { id = id });
        }

        [AllowAnonymous]
        public ActionResult DownloadExamPaper(int id, string name)
        {
            return File(bat.logic.Rules.ExamPapers.DownloadPaper(id).ToArray(), "application", name);
        }
    }
}