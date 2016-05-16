using bat.data;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace bat
{
    public class BlackboardHub : Hub
    {
        //Connect to Group
        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }

        public void UpdateModel(ChalkModel clientModel)
        {
            clientModel.LastUpdatedBy = Context.ConnectionId;
            // Update the Chalk model within our broadcaster
            Clients.Group(clientModel.Group, clientModel.LastUpdatedBy).UpdateChalk(clientModel);
        }
    }
    public class ChalkModel
    {
        // We declare Left and Top as lowercase with 
        // JsonProperty to sync the client and server models
        [JsonProperty("x")]
        public double x { get; set; }
        [JsonProperty("y")]
        public double y { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        [JsonProperty("group")]
        public string Group { get; set; }
        // We don't want the client to get the "LastUpdatedBy" property
        [JsonIgnore]
        public string LastUpdatedBy { get; set; }
    }

    public class ChatHub : Hub
    {
        public Lesson lesson { get; set; }
        public ChatRecord chatRecord { get; set; }
        //Connect to Group
        public void JoinGroup(string groupName, string name)
        {
            Groups.Add(Context.ConnectionId, groupName);
            Clients.Group(groupName).broadcastMessage("Classroom", name +" has joined.");          
        }


        public void Send(MyMessage message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.Group(message.GroupName).broadcastMessage(message.Name, message.Msg);

            // Save message to DB
            using (var conn = new dbEntities())
            {
                var id = Int32.Parse(message.GroupName);

                this.lesson = conn.Lessons.FirstOrDefault(a => a.ID == id);
                if (this.lesson == null) throw new Exception("Lesson does not exist.");

                this.chatRecord = new ChatRecord()
                {
                    Lesson_ID = id,
                    Chat_User = message.Name,
                    Char_Message = message.Msg,
                    DateTime = Shearnie.Net.OzTime.GetNowAEST(),
                };
                conn.ChatRecords.Add(this.chatRecord);
                conn.SaveChanges();
            }
        }
    }

    public class MyMessage
    {
        public string Name { get; set; }
        public string Msg { get; set; }
        public string GroupName { get; set; }
    }
}