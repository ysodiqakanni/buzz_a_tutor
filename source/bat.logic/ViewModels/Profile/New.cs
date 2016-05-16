﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using System.Web.Mvc;
namespace bat.logic.ViewModels.Profile
{
    public class New : Master
    {
        public FamilyMember familyMemeber { get; set; }
        public Account Account { get; set; }

        public New()
        {
            this.familyMemeber = new FamilyMember();
            this.Account = new Account();
        }

        public void Save(int id, FormCollection frm)
        {
            using (var conn = new dbEntities())
            {
                this.account = conn.Accounts.FirstOrDefault(a => a.ID == id);
                if (this.account == null) throw new Exception("Account does not exist.");

                this.Account = new Account()
                {
                    AccountType_ID = (int) Constants.Types.AccountTypes.Student,
                    Fname = (frm["Fname"]),
                    Lname = (frm["Lname"]),
                    Email = (frm["Email"]),
                    Password = Helpers.PasswordStorage.CreateHash("")
                };
                conn.Accounts.Add(this.Account);
                conn.SaveChanges();

                this.familyMemeber = new FamilyMember()
                {
                    Account_ID = this.Account.ID,
                    Parent_ID = id,
                };
                conn.FamilyMembers.Add(this.familyMemeber);
                conn.SaveChanges();
            }
        }
    }
}


