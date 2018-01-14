using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using System.Web.Mvc;

namespace bat.logic.ViewModels.Profile
{
    public class New
    {
        public FamilyMember familyMemeber { get; set; }
        public Account Account { get; set; }

        public New()
        {
            this.familyMemeber = new FamilyMember();
            this.Account = new Account();
        }
    }
}
