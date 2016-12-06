using Elmah;
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
        [Authorize]
        public ActionResult Index(int id)
        {
            var model = new bat.logic.ViewModels.Lessons.View();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model.Initialise(user.ID);
                model.Load(id);

                // TokBox option, disable for now to try Zoom only
                //if (model.WebRTCAvailable)
                //{
                //    model.GenerateTokBoxToken();
                //    return View("ViewTokBox", model);
                //}

                if (!model.CurrentlyAZoomUser)
                {
                    model.CheckZoomUser();
                    return View("NewZoomUserAccount", model);
                }

                switch (model.accountType)
                {
                    case Types.AccountTypes.Student:
                        if (!model.LessonReady)
                            return View("ZoomLessonHostNotReady", model);
                        break;

                    case Types.AccountTypes.Teacher:
                        model.CreateZoomMeeting();
                        return View("ViewZoom", model);

                    default:
                        throw new Exception("Invalid account type.");
                }


                model.CreateZoomMeeting();
                return View("ViewZoom", model);
            }
            catch(ZoomException ex) when (ex.Code == bat.logic.Constants.Zoom.ErrorCode_CannotCreateMeeting)
            {
                return View("NewZoomUserAccount", model);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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

                model.Initialise(user.ID);
                if (!model.IsTeacher) return RedirectToRoute("home");
                model.SetDate(date);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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
            var model = new bat.logic.ViewModels.Lessons.Join();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model.Initialise(user.ID);
                if (!model.Load(id))
                    return RedirectToAction("Index", "Lessons", new { id = id });
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
            var model = new bat.logic.ViewModels.Lessons.Join();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model.Initialise(user.ID);
                model.Save(id);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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

                model.Initialise(user.ID);
                if (!model.Load(lessonid))
                    return RedirectToAction("Index", "Lessons", new { id = lessonid });
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
        public ActionResult Leave(int lessonid, int studentid, FormCollection frm)
        {
            var model = new bat.logic.ViewModels.Lessons.Leave();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model.Delete(lessonid, studentid);
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
            }
            return RedirectToRoute("home");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Upload(int id, HttpPostedFileBase data, string title)
        {
            var model = new bat.logic.ViewModels.Lessons.View();

            try
            {
                var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
                if (user == null) return RedirectToRoute("home");

                model.Initialise(user.ID);
                model.UploadImage(id, title, data);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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

                model.Initialise(user.ID);
                if (!model.IsTeacher) return RedirectToRoute("home");
                model.Load(id);
                
                return View(model);
            }
            catch (WrongAccountException)
            {
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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

                model.Initialise(user.ID);
                if (!model.IsTeacher) return RedirectToRoute("home");
                model.Save(id, frm);
            }
            catch (WrongAccountException)
            {
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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

                model.Initialise(user.ID);
                if (!model.IsTeacher) return RedirectToRoute("home");
                model.Load(id);

                return View(model);
            }
            catch (WrongAccountException)
            {
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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

                model.Initialise(user.ID);
                if (!model.IsTeacher) return RedirectToRoute("home");
                model.Save(id, frm);
            }
            catch (WrongAccountException)
            {
                return RedirectToRoute("home");
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ViewBag.Error = ex.Message;
                return View("Error", model);
            }

            return RedirectToAction("Index", new { id = model.lesson.ID });
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
            return RedirectToAction("Edit", new { id = id });
        }

        [AllowAnonymous]
        public ActionResult DownloadExamPaper(int id, string name)
        {
            return File(bat.logic.Rules.ExamPapers.DownloadPaper(id).ToArray(), "application", name);
        }
    }
}