using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using System.Web.Script.Serialization;

namespace bat.logic.ApiModels.Chat
{
    public class Chat
    {
        public class message
        {
            public string name { get; set; }
            public string msg { get; set; }
        }
        public List<message> chatHistory { get; set; }
        public static string GetChatHistory(int lessonID)
        {
            var json = new JavaScriptSerializer();
            var chatHistory = new List<message>();
            using (var conn = new dbEntities())
            {
                var chatRecord = conn.ChatRecords.Where(c => c.Lesson_ID == lessonID)
                    .OrderBy(c => c.DateTime)
                    .ToList();
                foreach(var message in chatRecord)
                {
                    chatHistory.Add(new message
                    {
                        name = message.Chat_User,
                        msg = message.Char_Message
                    });
                }
                return json.Serialize(chatHistory);
            }
        }
    }
}
