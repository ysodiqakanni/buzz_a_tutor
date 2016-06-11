using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Models.Zoom
{
	public class GetUser
	{
		public string email { get; set; }
		public string id { get; set; }
		public string account_id { get; set; }
		public DateTime created_at { get; set; }
		public string first_name { get; set; }
		public string last_name { get; set; }
		public int type { get; set; }
		public string token { get; set; }
		public string pic_url { get; set; }
		public bool disable_chat { get; set; }
		public bool enable_e2e_encryption { get; set; }
		public bool enable_silent_mode { get; set; }
		public bool disable_group_hd { get; set; }
		public bool disable_recording { get; set; }
		public bool enable_cmr { get; set; }
		public bool enable_auto_recording { get; set; }
		public bool enable_cloud_auto_recording { get; set; }
		public int verified { get; set; }
		public long pmi { get; set; }
		public string vanity_url { get; set; }
		public int meeting_capacity { get; set; }
		public bool enable_webinar { get; set; }
		public int webinar_capacity { get; set; }
		public bool enable_large { get; set; }
		public int large_capacity { get; set; }
		public bool disable_feedback { get; set; }
		public bool disable_jbh_reminder { get; set; }
		public bool enable_breakout_room { get; set; }
		public string dept { get; set; }
		public string timezone { get; set; }
		public string lastClientVersion { get; set; }
		public DateTime lastLoginTime { get; set; }
		public string zpk { get; set; }
	}
}
