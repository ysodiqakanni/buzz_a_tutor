﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Constants;
using bat.logic.Exceptions;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using bat.logic.Models;

namespace bat.logic.Models.Profile
{
    public class EditMember : Master
    {
        public FamilyMember familyMember { get; set; }

        public void load (int id)
        {
            using (var conn = new dbEntities())
            {
                this.familyMember = conn.FamilyMembers.FirstOrDefault(a => a.ID == id);
                if (this.account == null) throw new Exception("Family member does not exist.");
            }
        }

        public void Save(FormCollection frm)
        {
            int memberID = Convert.ToInt32((frm["MemberID"]));
            using (var conn = new dbEntities())
            {
                this.familyMember = conn.FamilyMembers.FirstOrDefault(a => a.ID == memberID);
                if (this.account == null) throw new Exception("Account does not exist.");

                familyMember.Name = (frm["Name"]);              
                conn.SaveChanges();
            }
        }

        public void Delete(int id) {
            using (var conn = new dbEntities())
            {
                this.familyMember = conn.FamilyMembers.FirstOrDefault(a => a.ID == id);
                if (this.account == null) throw new Exception("Account does not exist.");

                conn.FamilyMembers.Remove(conn.FamilyMembers.FirstOrDefault(i => i.ID == id));
                conn.SaveChanges();
            }
        }
    }
}
