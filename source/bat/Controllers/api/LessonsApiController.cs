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
        [HttpPost]
        public string Upload(FormDataCollection formData)
        {
            var user = new logic.Rules.Authentication(HttpContext.Current.Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) throw new Exception("Unauthorised access.");

            return bat.logic.ApiModels.Lessons.Upload.UploadImage(Convert.ToInt32(formData["lessonid"]), user.ID, formData["title"], formData["data"]);
        }

        [HttpPost]
        public string UploadFile()
        {
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                // Get the uploaded image from the Files collection
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];

                if (httpPostedFile != null)
                {
                    // Validate the uploaded image(optional)
                    return "success";
                }
                return "fail";
            }
            return "fail";
        }

        [Authorize]
        [System.Web.Http.HttpPost]
        public string UploadtoCloud(FormDataCollection formData)
        {
            var user = new logic.Rules.Authentication(HttpContext.Current.Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) throw new Exception("Unauthorised access.");

            return logic.Rules.ResourceManagement.UploadResourceImage(formData);
        }

        public string DownloadFromCloud(FormDataCollection formData)
        {
            return logic.Rules.ResourceManagement.DownloadLessonImage(Convert.ToInt32(formData["attachmentid"]));
        }

        [Authorize]
        [System.Web.Http.HttpPost]
        public string Getattachment(FormDataCollection formData)
        {
            var user = new logic.Rules.Authentication(HttpContext.Current.Request.GetOwinContext()).GetLoggedInUser();
            if (user == null) throw new Exception("Unauthorised access.");

            return bat.logic.ApiModels.Lessons.GetAttachment.GetImageStream(Convert.ToInt32(formData["attachmentid"]));
        }

        [Authorize]
        [System.Web.Http.HttpPost]
        public string GetBlackboardImages(FormDataCollection formData)
        {
            return bat.logic.ApiModels.Lessons.GetAttachment.GetBlackboardImages(Convert.ToInt32(formData["lessonId"]));
        }
    }
}
