using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bat.Controllers.api
{
    public class ProfilePictureApiController : Controller
    {
        public ActionResult DownloadResource(int id, string name)
        {
            return File(logic.Rules.ResourceManagement.DownloadProfilePicture(id).ToArray(), "application", name);
        }
    }
}