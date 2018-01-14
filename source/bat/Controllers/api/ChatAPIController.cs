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
    public class ChatApiController : ApiController
    {
        private readonly logic.Services.Chat _chatService;

        public ChatApiController(
            logic.Services.Chat chatService)
        {
            _chatService = chatService;
        }

        public string GetChatHistory(int lessonID)
        {
            var json = new JavaScriptSerializer();
            return json.Serialize(_chatService.GetChatHistory(lessonID));
        }
    }
}