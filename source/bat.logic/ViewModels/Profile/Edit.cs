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
    public class Edit : Master
    {
        public new Account account { get; set; }

        public void Load(int id)
        {
            using (var conn = new dbEntities())
            {
                this.account = conn.Accounts.FirstOrDefault(a => a.ID == id);
                if (this.account == null) throw new Exception("Account does not exist.");
            }
        }

        public void Save(int id, string FirstName, string LastName, string Description, string Qualifications, int? Rate, System.Web.HttpPostedFileBase Picture)
        {
            using (var conn = new dbEntities())
            {
                account = conn.Accounts.FirstOrDefault(a => a.ID == id);
                if (this.account == null) throw new Exception("Account does not exist.");
                account.Fname = FirstName;
                account.Lname = LastName;
                account.Description = Description;
                account.Qualifications = Qualifications;
                account.Rate = Rate;
                account.Picture = logic.Helpers.AzureStorage.StoredResources.Upload(Picture);

                conn.SaveChanges();
            }
        }
    }
}



