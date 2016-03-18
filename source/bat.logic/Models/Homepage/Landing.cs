using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;

namespace bat.logic.Models.Homepage
{
    public class Landing
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        public void Signup(string type, string firstname, string lastname, string email, string password)
        {
            type = type.Trim();
            if (!type.Equals("student", StringComparison.CurrentCultureIgnoreCase) &&
                !type.Equals("teacher", StringComparison.CurrentCultureIgnoreCase))
                throw new Exception("Invalid account type.");

            this.firstName = firstname.Trim();
            this.lastName = lastname.Trim();
            this.email = email.Trim();
            this.password = password.Trim();

            using (var conn = new dbEntities())
            {
                if (conn.Accounts.Any(a => a.Email == this.email))
                    throw new Exception("Email already registered.");

                conn.Accounts.Add(new Account()
                {
                    AccountType_ID =
                        type.Trim() == "student"
                            ? (int) Constants.Types.AccountTypes.Student
                            : (int) Constants.Types.AccountTypes.Teacher,
                    Fname = this.firstName,
                    Lname = this.lastName,
                    Email = this.email,
                    Password = this.password
                });
                conn.SaveChanges();
            }
        }
    }
}
