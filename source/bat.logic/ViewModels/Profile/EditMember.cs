using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using System.Web.Mvc;
using bat.logic.Exceptions;
using EntityFramework.Extensions;

namespace bat.logic.ViewModels.Profile
{
    public class EditMember
    {
        public FamilyMember familyMember { get; set; }
        public data.Account FamilyMemberAccount { get; set; }
    }
}
