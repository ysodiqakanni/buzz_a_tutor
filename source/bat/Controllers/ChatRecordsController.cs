using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bat.data;
using Elmah;

namespace bat.Controllers
{
    public class ChatRecordsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.ChatRecords.Index();

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
        public ActionResult Details(int id)
        {
            var user = new logic.Rules.Authentication(Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) return RedirectToRoute("home");

            var model = new bat.logic.ViewModels.ChatRecords.Details();

            try
            {
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
    }
}
