using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Models
{
    public class Meet : Master
    {
        private string pubToken => "T1==cGFydG5lcl9pZD00NTQ5NjY1MiZzaWc9OGZhNjgzMTIxYjMyMDRiZmJlNjRiYzExNjI1MzFhMzc3MWZlNDE4Nzpyb2xlPXB1Ymxpc2hlciZzZXNzaW9uX2lkPTFfTVg0ME5UUTVOalkxTW41LU1UUTFOakF4TURRNU5qSTRNSDVNVFVOR1QzUjZSVGhFVkd0eGFGSndTVE5vUlhCcFVXZC1VSDQmY3JlYXRlX3RpbWU9MTQ1NjAxMDUyOCZub25jZT0wLjUzODQ4MjY5Njg0NDY2NzcmZXhwaXJlX3RpbWU9MTQ1ODYwMDk4NyZjb25uZWN0aW9uX2RhdGE9";
        private string subToken => "T1==cGFydG5lcl9pZD00NTQ5NjY1MiZzaWc9ZWNiNjUxZjUxYWNlYTg3NmUyYzE4NmU2YmEyMTI3YTg4ZTY0ZmI0Zjpyb2xlPXN1YnNjcmliZXImc2Vzc2lvbl9pZD0xX01YNDBOVFE1TmpZMU1uNS1NVFExTmpBeE1EUTVOakk0TUg1TVRVTkdUM1I2UlRoRVZHdHhhRkp3U1ROb1JYQnBVV2QtVUg0JmNyZWF0ZV90aW1lPTE0NTYwMTA1NDcmbm9uY2U9MC41NzQ4MzExOTUxMTM2MjkxJmV4cGlyZV90aW1lPTE0NTg2MDA5ODcmY29ubmVjdGlvbl9kYXRhPQ==";

        public string session { get; set; }
        public string token { get; set; }

        public Meet()
        {
            this.session = "1_MX40NTQ5NjY1Mn5-MTQ1NjAxMDQ5NjI4MH5MTUNGT3R6RThEVGtxaFJwSTNoRXBpUWd-UH4";
        }

        public void Load(int meId, int otherId)
        {
            using (var conn = new dbEntities())
            {
                var me = conn.Accounts.FirstOrDefault(a => a.ID == meId);
                if (me == null) throw new Exception("Invalid user.");

                var other = conn.Accounts.FirstOrDefault(a => a.ID == otherId);
                if (other == null) throw new Exception("Invalid user.");

                switch (me.AccountType_ID)
                {
                    case (int) bat.logic.Constants.Types.AccountTypes.Teacher:
                        this.token = this.pubToken;
                        break;

                    case (int)bat.logic.Constants.Types.AccountTypes.Student:
                        this.token = this.subToken;
                        break;
                }
            }
        }
    }
}
