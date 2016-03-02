using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using bat.logic.Constants;
using bat.logic.Exceptions;

namespace bat.logic.Models.Homepage
{
    public class Dashboard : Master
    {
        public List<Lesson> lessons { get; set; }
        public List<Lesson> lessonsToJoin { get; set; } 

        public void Load()
        {
            if (!this.initialised) throw new MasterModelNotInitialised();

            using (var conn = new dbEntities())
            {
                //switch (this.accountType)
                //{
                //    case Types.AccountTypes.Student:
                //        this.teachers =
                //            conn.Accounts.Where(
                //                a => a.ID != this.account.ID && a.AccountType_ID == (int) Types.AccountTypes.Teacher)
                //                .ToList();
                //        break;

                //    case Types.AccountTypes.Teacher:
                //        this.students =
                //            conn.Accounts.Where(
                //                a => a.ID != this.account.ID && a.AccountType_ID == (int)Types.AccountTypes.Student)
                //                .ToList();
                //        break;

                //    default:
                //        throw new Exception("Invalid account type.");
                //}

                this.lessons = conn.Lessons.Where(l => l.Account_ID == this.account.ID).ToList();

                // show lessons student can join
                if (this.accountType == Types.AccountTypes.Student)
                {
                    this.lessonsToJoin =
                        conn.Lessons.Where(l => !this.lessons.Select(ml => ml.ID).Contains(l.ID)).ToList();
                }
            }
        }
    }
}
