using bat.data;
using bat.logic.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace bat.logic.Services
{
    public class Chat : _ServiceClassBaseMarker
    {
        public List<ApiViewModels.Chat.Chat.message> GetChatHistory(int lessonId)
        {
            var chatHistory = new List<ApiViewModels.Chat.Chat.message>();
            using (var conn = new dbEntities())
            {
                var chatRecord = conn.ChatRecords.Where(c => c.Lesson_ID == lessonId)
                    .OrderBy(c => c.DateTime)
                    .ToList();
                foreach (var message in chatRecord)
                {
                    chatHistory.Add(new ApiViewModels.Chat.Chat.message
                    {
                        name = message.Chat_User,
                        msg = message.Char_Message
                    });
                }
                return chatHistory;
            }
        }
    }
}
