using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;


namespace bat
{

    public class BlackboardHub : Hub
    {
        public void UpdateModel(ChalkModel clientModel)
        {
            clientModel.LastUpdatedBy = Context.ConnectionId;
            // Update the Chalk model within our broadcaster
            Clients.AllExcept(clientModel.LastUpdatedBy).UpdateChalk(clientModel);
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
        // We don't want the client to get the "LastUpdatedBy" property
        [JsonIgnore]
        public string LastUpdatedBy { get; set; }
    }
}