using bat.data;
using bat.logic.Constants;
using bat.logic.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace bat.logic.Services
{
    public class AccountInfo : _ServiceClassBaseMarker
    {
        public Models.AccountInfo Get(int accountId)
        {
            if (accountId == 0)
            {
                return new Models.AccountInfo()
                {
                    account = new Account()
                    {
                        ID = 0,
                        Email = AdminLogin.User
                    }
                };
            }

            using (var conn = new dbEntities())
            {
                var ret = new Models.AccountInfo()
                {
                    account = conn.Accounts.AsNoTracking().FirstOrDefault(a => a.ID == accountId)
                };
                if (ret.account == null) throw new InvalidRecordException();

                if (ret.account.AccountType_ID == Constants.Types.Teacher && !(ret.account.Approved ?? false))
                    throw new Exception("Your account is pending approval.");

                if (ret.account.Disabled ?? false)
                    throw new Exception("Your account has been disabled.");

                ret.familyMembers = conn.FamilyMembers.AsNoTracking().Where(i => i.Parent_ID == accountId).ToList();

                // if not a parent, might be a family member
                if (!ret.familyMembers.Any())
                {
                    var fam = conn.FamilyMembers.AsNoTracking()
                        .Select(i => new { i.Account_ID, i.Parent_ID })
                        .FirstOrDefault(i => i.Account_ID == accountId);

                    if (fam != null)
                        ret.parent = conn.Accounts.AsNoTracking().FirstOrDefault(a => a.ID == fam.Parent_ID);
                }

                return ret;
            }
        }
    }
}
