using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;

namespace bat.logic.ViewModels.Homepage
{
    public class Register : Master
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        public void Signup(int type, string firstname, string lastname, string email, string password)
        {
            this.firstName = firstname.Trim();
            this.lastName = lastname.Trim();
            this.email = email.Trim();
            this.password = password.Trim();

            Rules.Signup.Register(type, firstname, lastname, email, password);
        }
    }
}
