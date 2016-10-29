using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using Newtonsoft.Json;

namespace bat.logic.Rules
{
    class EventLogging
    {
        public static void Login(Account user)
        {
            using (var conn = new dbEntities())
            {
                conn.Configuration.AutoDetectChangesEnabled = false;
                conn.Configuration.ValidateOnSaveEnabled = false;

                conn.EventLogs.Add(new EventLog()
                {
                    Account_ID = user.ID,
                    Type = Models.Auditing.AccountLogin.TypeToString(),
                    Data = JsonConvert.SerializeObject(
                    new Models.Auditing.AccountLogin()
                    {
                        ID = user.ID,
                        Email = user.Email,
                        Fname = user.Fname,
                        Lname = user.Lname
                    }),
                    EventDate = DateTime.UtcNow,
                    IPAddress = Shearnie.Net.Web.ServerInfo.GetIPAddress
                });
                conn.SaveChanges();
            }
        }

        public static void Register(Account user)
        {
            using (var conn = new dbEntities())
            {
                conn.Configuration.AutoDetectChangesEnabled = false;
                conn.Configuration.ValidateOnSaveEnabled = false;

                conn.EventLogs.Add(new EventLog()
                {
                    Account_ID = user.ID,
                    Type = Models.Auditing.AccountRegistration.TypeToString(),
                    Data = JsonConvert.SerializeObject(
                        new Models.Auditing.AccountRegistration()
                        {
                            ID = user.ID,
                            AccountType_ID = user.AccountType_ID,
                            AccountType = ((logic.Constants.Types.AccountTypes)user.AccountType_ID).ToString(),
                            Email = user.Email,
                            Fname = user.Fname,
                            Lname = user.Lname
                        }),
                    EventDate = DateTime.UtcNow,
                    IPAddress = Shearnie.Net.Web.ServerInfo.GetIPAddress
                });
                conn.SaveChanges();
            }
        }
    }
}
