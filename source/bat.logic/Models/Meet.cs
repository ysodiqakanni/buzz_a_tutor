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
        public string session { get; set; }
        public string token { get; set; }

        public Account me { get; set; }
        public Account other { get; set; }

        public Meet()
        {
            this.session = "1_MX40NTQ5NjY1Mn5-MTQ1NjAxMDQ5NjI4MH5MTUNGT3R6RThEVGtxaFJwSTNoRXBpUWd-UH4";
        }

        public void Load(int meId, int otherId)
        {
            using (var conn = new dbEntities())
            {
                this.me = conn.Accounts.FirstOrDefault(a => a.ID == meId);
                if (this.me == null) throw new Exception("Invalid user.");

                this.other = conn.Accounts.FirstOrDefault(a => a.ID == otherId);
                if (this.other == null) throw new Exception("Invalid user.");

                if (this.me.Email == "alex")
                    this.token = "T1==cGFydG5lcl9pZD00NTQ5NjY1MiZzaWc9NzZkM2Q1YmM5MWQyZmUzZDcwMzZiZmU4MDQxOTA4MTRmNDk3NmY2Yjpyb2xlPXB1Ymxpc2hlciZzZXNzaW9uX2lkPTFfTVg0ME5UUTVOalkxTW41LU1UUTFOakF4TURRNU5qSTRNSDVNVFVOR1QzUjZSVGhFVkd0eGFGSndTVE5vUlhCcFVXZC1VSDQmY3JlYXRlX3RpbWU9MTQ1NjAxMTkyMyZub25jZT0wLjMyMTY5MTkwNTc1NTYxMTQ2JmV4cGlyZV90aW1lPTE0NTg2MDA5ODcmY29ubmVjdGlvbl9kYXRhPQ==";
                else
                    this.token = "T1==cGFydG5lcl9pZD00NTQ5NjY1MiZzaWc9OGE4OTZjZGM1NGQyZTMzMDI1YmI0NzQ4ODZlZDlhM2Y5ZjFjNDEzNDpyb2xlPXB1Ymxpc2hlciZzZXNzaW9uX2lkPTFfTVg0ME5UUTVOalkxTW41LU1UUTFOakF4TURRNU5qSTRNSDVNVFVOR1QzUjZSVGhFVkd0eGFGSndTVE5vUlhCcFVXZC1VSDQmY3JlYXRlX3RpbWU9MTQ1NjAxMTk3OSZub25jZT0wLjg4MzcxMDU4NDg4MDIyMDQmZXhwaXJlX3RpbWU9MTQ1ODYwMDk4NyZjb25uZWN0aW9uX2RhdGE9";
            }
        }
    }
}
