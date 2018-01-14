using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Constants;
using bat.logic.Exceptions;

namespace bat.logic.Models
{
    public class AccountInfo
    {
        public Account account { get; set; }
        public Constants.Types.AccountTypes accountType => (Constants.Types.AccountTypes) this.account.AccountType_ID;
        public List<FamilyMember> familyMembers { get; set; }
        public Account parent { get; set; }

        public AccountInfo()
        {
            this.account = new Account();
        }

        public bool IsEnabled =>
            (this.account.Approved ?? false) == Constants.Status.Approved && (this.account.Disabled ?? false) == Constants.Status.Enabled;

        public bool IsTeacher =>
            this.account.AccountType_ID == (int) Constants.Types.AccountTypes.Teacher;
    }
}
