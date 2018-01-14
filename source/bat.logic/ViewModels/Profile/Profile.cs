using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.logic.Models;

namespace bat.logic.ViewModels.Profile
{
    public class Profile
    {
        public Models.AccountInfo AccInfo { get; set; } = new AccountInfo();
    }
}