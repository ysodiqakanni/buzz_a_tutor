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

                if (Picture == null) account.Picture = null;
                else
                {
                    if (Picture.ContentLength > 2500000)
                        throw new Exception("Picture can't exceed 2.5MB in size.");

                    var ext = System.IO.Path.GetExtension(Picture.FileName);
                    if (!logic.Rules.ImageValidation.ValidateExtension(ext))
                        throw new Exception("Invalid file extension.");

                    byte[] thePictureAsBytes = new byte[Picture.ContentLength];
                    using (BinaryReader theReader = new BinaryReader(Picture.InputStream))
                    {
                        thePictureAsBytes = theReader.ReadBytes(Picture.ContentLength);
                    }
                    account.Picture = Convert.ToBase64String(thePictureAsBytes);
                }
                conn.SaveChanges();
            }
        }
    }
}



