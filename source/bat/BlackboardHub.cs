using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;


namespace bat
{

    public class BlackboardHub : Hub
    {
        //Connect to Group
        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
            Clients.Group(groupName).addChatMessage(Context.User.Identity.Name + " joined.");
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
        //Connect to Group
        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
            Clients.Group(groupName).addChatMessage(Context.User.Identity.Name + " joined.");
        }

        //public void Send(string name, string message)
        //{
        //    // Call the broadcastMessage method to update clients.
        //    Clients.All.broadcastMessage(name, message);
        //}

        public void Send(MyMessage message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(message.Name, message.Msg);
            Clients.Group(message.GroupName).broadcastMessage(new MyMessage() { Name = message.Name, Msg = message.Msg });
        }

    }

    public class MyMessage
    {
        public string Name { get; set; }
        public string Msg { get; set; }
        public string GroupName { get; set; }
    }
}