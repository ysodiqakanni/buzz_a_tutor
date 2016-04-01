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
    public class LessonsApiController : ApiController
    {
        [Authorize]
        [System.Web.Http.HttpPost]
        public string Upload(FormDataCollection formData)
        {
            var user = new logic.Models.System.Authentication(HttpContext.Current.Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) throw new Exception("Unauthorised access.");

            return bat.logic.Models.Api.Lessons.Upload.UploadImage(Convert.ToInt32(formData["lessonid"]), user.ID, formData["title"], formData["data"]);
        }
    }
}
