﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.logic.Constants;
using bat.logic.Models.Zoom;
using Newtonsoft.Json;
using bat.data;
using bat.logic.Exceptions;

namespace bat.logic.Rules
{
    public class ZoomApi
    {
        private static void CheckError(string resultJson)
        {
            var rs = JsonConvert.DeserializeObject<Error>(resultJson);
            if (rs.error != null)
                if (rs.error.code > 0)
                    throw new ZoomException(rs.error.code, rs.error.message);
        }

        public static string ListUsers()
        {
            var rs = Shearnie.Net.Web.RESTJSON.PostSync("https://api.zoom.us/v1/user/list", new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("api_key", Constants.Zoom.ApiKey),
                new KeyValuePair<string, string>("api_secret", Constants.Zoom.ApiSecret),
            });
            return "";
        }

        public static CreateUser CreateUser(string fName, string lName, string email, Zoom.UserTypes type)
        {
            var rs = Shearnie.Net.Web.RESTJSON.PostSync("https://api.zoom.us/v1/user/create", new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("api_key", Constants.Zoom.ApiKey),
                new KeyValuePair<string, string>("api_secret", Constants.Zoom.ApiSecret),
                new KeyValuePair<string, string>("data_type", "JSON"),
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("type", ((int) type).ToString()),
                new KeyValuePair<string, string>("first_name", fName),
                new KeyValuePair<string, string>("last_name", lName),
            });
            CheckError(rs);
            return JsonConvert.DeserializeObject<CreateUser>(rs);
        }

        public static GetUser GetUser(string id)
        {
            var rs = Shearnie.Net.Web.RESTJSON.PostSync("https://api.zoom.us/v1/user/get", new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("api_key", Constants.Zoom.ApiKey),
                new KeyValuePair<string, string>("api_secret", Constants.Zoom.ApiSecret),
                new KeyValuePair<string, string>("data_type", "JSON"),
                new KeyValuePair<string, string>("id", id),
            });
            CheckError(rs);
            return JsonConvert.DeserializeObject<GetUser>(rs);
        }

        public static MeetingList ListMeetings(string hostId)
        {
            var rs = Shearnie.Net.Web.RESTJSON.PostSync("https://api.zoom.us/v1/meeting/list", new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("api_key", Constants.Zoom.ApiKey),
                new KeyValuePair<string, string>("api_secret", Constants.Zoom.ApiSecret),
                new KeyValuePair<string, string>("data_type", "JSON"),
                new KeyValuePair<string, string>("host_id", hostId),
            });
            CheckError(rs);
            return JsonConvert.DeserializeObject<MeetingList>(rs);
        }

        public static Meeting CreateMeeting(string hostId, string topic)
        {
            var rs = Shearnie.Net.Web.RESTJSON.PostSync("https://api.zoom.us/v1/meeting/create", new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("api_key", Constants.Zoom.ApiKey),
                new KeyValuePair<string, string>("api_secret", Constants.Zoom.ApiSecret),
                new KeyValuePair<string, string>("data_type", "JSON"),
                new KeyValuePair<string, string>("host_id", hostId),
                new KeyValuePair<string, string>("topic", topic),
                new KeyValuePair<string, string>("type", "2"), // standard scheduled meeting
                new KeyValuePair<string, string>("option_jbh", "true"),
            });
            CheckError(rs);
            return JsonConvert.DeserializeObject<Meeting>(rs);
        }

        public static Account CreateZoomUserAccount(int accountId)
        {
            using (var conn = new dbEntities())
            {
                var account = conn.Accounts.FirstOrDefault(a => a.ID == accountId);
                if (account == null) throw new Exception("Invalid user account.");

                // already registered
                if (!string.IsNullOrEmpty(account.ZoomUserId))
                    return account;

                var user = CreateUser(account.Fname, account.Lname, account.Email, Zoom.UserTypes.Basic);
                account.ZoomUserId = user.id;

                conn.SaveChanges();

                return account;
            }
        }
    }
}
