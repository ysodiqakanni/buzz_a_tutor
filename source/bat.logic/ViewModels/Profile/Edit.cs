using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using System.Web.Mvc;
using System.IO;

namespace bat.logic.ViewModels.Profile
{
    public class Edit
    {
        public Account account { get; set; } = new Account();
    }
}
