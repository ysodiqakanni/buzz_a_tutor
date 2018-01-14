using bat.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using bat.logic.Services;

namespace bat.logic.Helpers
{
    public class UserAccountInfo
    {
        public static Models.AccountInfo Get(int accountId)
        {
            var svc = DependencyResolver.Current.GetService<AccountInfo>();
            return svc.Get(accountId);
        }

        public static Models.AccountInfo GetAccount(int accountId)
        {
            using (var conn = new dbEntities())
            {
                return new Models.AccountInfo()
                {
                    account = conn.Accounts.AsNoTracking().FirstOrDefault(a => a.ID == accountId)
                };
            }
        }

        public static string GetAccountFirstName(int accountId)
        {
            using (var conn = new dbEntities())
            {
                return conn.Accounts.AsNoTracking().Select(a => new { a.ID, a.Fname }).FirstOrDefault(a => a.ID == accountId)?.Fname;
            }
        }

        public static string GetAccountPicture(int accountId)
        {
            using (var conn = new dbEntities())
            {
                return conn.Accounts.AsNoTracking().Select(a => new { a.ID, a.Picture }).FirstOrDefault(a => a.ID == accountId)?.Picture;
            }
        }

        public static string GetFullAccountName(int accountId)
        {
            using (var conn = new dbEntities())
            {
                var acc = conn.Accounts.AsNoTracking().Select(a => new { a.ID, a.Fname, a.Lname }).FirstOrDefault(a => a.ID == accountId);
                return acc?.Fname + " " + acc?.Lname;
            }
        }
    }
}
