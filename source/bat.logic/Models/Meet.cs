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
        private string pubToken => "T1==cGFydG5lcl9pZD00NTQ5NjY1MiZzaWc9YzNiMDA0NDdkMGRkOWQ0NzQzMzBkODg5ZTgwZTRmOGZmYmVlYzYyMjpyb2xlPXB1Ymxpc2hlciZzZXNzaW9uX2lkPTJfTVg0ME5UUTVOalkxTW41LU1UUTFOVGMxTmpNd016Y3pNSDVPZWl0alprbG1lVlF6VTFvMVpsVnJkU3QxU3pacWJ6Si1VSDQmY3JlYXRlX3RpbWU9MTQ1NTc1NjM0NiZub25jZT0wLjUxNzUzNjg4MjE4Njc1NTYmZXhwaXJlX3RpbWU9MTQ1NTg0MjY1OCZjb25uZWN0aW9uX2RhdGE9";
        private string subToken => "T1==cGFydG5lcl9pZD00NTQ5NjY1MiZzaWc9OTIzNzg4ZTkwMzlmYWU1MDM2NzIwZDU1MDU5YWYyOTVlYjgyYmNjZTpyb2xlPXN1YnNjcmliZXImc2Vzc2lvbl9pZD0xX01YNDBOVFE1TmpZMU1uNS1NVFExTmpBd09URTJNemsxT0g1cmRXWTBNVmQ2ZWxSdmFFTlhjMDEwVUhoNGNuYzRVMnAtVUg0JmNyZWF0ZV90aW1lPTE0NTYwMDk0NjEmbm9uY2U9MC42NTg5MzU4MTI4NDA1MjI1JmV4cGlyZV90aW1lPTE0NTg2MDA5ODcmY29ubmVjdGlvbl9kYXRhPQ==";

        public string session { get; set; }
        public string token { get; set; }

        public Meet()
        {
            this.session = "1_MX40NTQ5NjY1Mn5-MTQ1NjAwOTE2Mzk1OH5rdWY0MVd6elRvaENXc010UHh4cnc4U2p-UH4";
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
