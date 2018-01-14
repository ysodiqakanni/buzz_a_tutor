using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace bat.Controllers.api
{
    public class DashBoardApiController : ApiController
    {
        private readonly logic.Services.HomePage _homeService;

        public DashBoardApiController(
            logic.Services.HomePage homeService)
        {
            _homeService = homeService;
        }

        [Authorize]
        [System.Web.Http.HttpPost]
        public string Getevents(FormDataCollection formData)
        {
            var user = new logic.Rules.Authentication(HttpContext.Current.Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) throw new Exception("Unauthorised access.");

            var json = new JavaScriptSerializer();
            return json.Serialize(_homeService.GetLessons(Convert.ToInt32(formData["userid"])));
        }

        [Authorize]
        [System.Web.Http.HttpPost]
        public string Getday(FormDataCollection formData)
        {
            var user = new logic.Rules.Authentication(HttpContext.Current.Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) throw new Exception("Unauthorised access.");

            var json = new JavaScriptSerializer();
            return json.Serialize(_homeService.GDay(Convert.ToInt32(formData["userid"]), formData["date"]));
        }
    }
}
