using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Rules
{
    public class ZoomApi
    {
        public static string ListUsers()
        {
            var rs = Shearnie.Net.Web.RESTJSON.PostSync("https://api.zoom.us/v1/user/list", new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("api_key", Constants.Zoom.ApiKey),
                new KeyValuePair<string, string>("api_secret", Constants.Zoom.ApiSecret),
            });
            return "";
        }

        public static string ListSessions()
        {
            var rs = Shearnie.Net.Web.RESTJSON.PostSync("https://api.zoom.us/v1/meeting/list", new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("api_key", Constants.Zoom.ApiKey),
                new KeyValuePair<string, string>("api_secret", Constants.Zoom.ApiSecret),
                new KeyValuePair<string, string>("data_type", "JSON"),
                new KeyValuePair<string, string>("host_id", Constants.Zoom.HostId_SteveShearn),
            });
            return "";
        }
    }
}
