using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

namespace bat.Controllers.api
{
    public class DashBoardApiController : ApiController
    {
        [Authorize]
        [System.Web.Http.HttpPost]
        public string Getevents(FormDataCollection formData)
        {
            var user = new logic.Rules.Authentication(HttpContext.Current.Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) throw new Exception("Unauthorised access.");

            return bat.logic.ApiModels.DashBoard.GetEvents.GetLessons(Convert.ToInt32(formData["userid"]));
        }

        [Authorize]
        [System.Web.Http.HttpPost]
        public string Getday(FormDataCollection formData)
        {
            var user = new logic.Rules.Authentication(HttpContext.Current.Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) throw new Exception("Unauthorised access.");

            return bat.logic.ApiModels.DashBoard.GetDay.GDay(Convert.ToInt32(formData["userid"]), Convert.ToDateTime(formData["date"]));
        }
    }
}
