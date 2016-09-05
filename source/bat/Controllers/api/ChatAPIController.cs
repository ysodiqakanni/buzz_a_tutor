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
    public class ChatApiController : ApiController
    {
        public string GetChatHistory(int lessonID)
        {
            return bat.logic.ApiModels.Chat.Chat.GetChatHistory(lessonID);
        }
    }
}