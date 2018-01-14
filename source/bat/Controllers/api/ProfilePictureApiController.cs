using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace bat.Controllers.api
{
    public class ProfilePictureApiController : Controller
    {
        public ActionResult DownloadResource(int id, string name)
        {
            var image = logic.Rules.ResourceManagement.DownloadProfilePicture(id);
            if (image != null)
            {
                return File(image.ToArray(), "application", name);
            } else
            {
                var dir = Server.MapPath("~/assets/img/");
                var defaultImage = "default-avatar.png";
                var path = Path.Combine(dir + defaultImage);
                return base.File(path, "image/png", "Default Image");
            }
        }
    }
}