﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Exceptions;

namespace bat.logic.ViewModels
{
    public class Master
    {
        internal bool initialised { get; set; }
        public Account account { get; set; }
        public Constants.Types.AccountTypes accountType => (Constants.Types.AccountTypes) this.account.AccountType_ID;
        public List<FamilyMember> familyMembers { get; set; }
        public Account parent { get; set; }

        public Master()
        {
            this.initialised = false;
            this.account = new Account();
        }

        public void Initialise(int accountId)
        {
            using (var conn = new dbEntities())
            {
                this.account = conn.Accounts.FirstOrDefault(a => a.ID == accountId);
                if (this.account == null) throw new InvalidRecordException();

                this.familyMembers = conn.FamilyMembers.Where(i => i.Parent_ID == accountId).ToList();

                // if not a parent, might be a family member
                if (!this.familyMembers.Any())
                {
                    var fam = conn.FamilyMembers.FirstOrDefault(i => i.Account_ID == accountId);
                    if (fam != null)
                        this.parent = conn.Accounts.FirstOrDefault(a => a.ID == fam.Parent_ID);
                }
            }
            this.initialised = true;
        }

        public void Initialise(Account account)
        {
            this.account = account;
            this.initialised = true;
        }

        public string GetAccountName(int accountId)
        {
            using (var conn = new dbEntities())
            {
                return conn.Accounts.Select(a => new { a.ID, a.Fname }).FirstOrDefault(a => a.ID == accountId)?.Fname;
            }
        }
    }
}