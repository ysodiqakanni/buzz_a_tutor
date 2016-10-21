﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using Newtonsoft.Json;

namespace bat.logic.Rules
{
    public class Signup
    {
        public static void Register(int type, string firstName, string lastName, string email, string password)
        {
            if (type != (int)logic.Constants.Types.AccountTypes.Student && type != (int)logic.Constants.Types.AccountTypes.Teacher)
                throw new Exception("Invalid account type.");

            firstName = firstName.Trim();
            lastName = lastName.Trim();
            email = email.Trim();
            password = password.Trim();

            if (string.IsNullOrEmpty(firstName))
                throw new Exception("First name required.");

            if (string.IsNullOrEmpty(lastName))
                throw new Exception("Last name required.");

            if (string.IsNullOrEmpty(email))
                throw new Exception("Email required.");

            if (string.IsNullOrEmpty(password))
                throw new Exception("Password required.");

            using (var conn = new dbEntities())
            {
                if (conn.Accounts.Any(a => a.Email == email))
                    throw new Exception("Email already registered.");

                var account = new Account()
                {
                    AccountType_ID = type,
                    Fname = firstName,
                    Lname = lastName,
                    Email = email,
                    Password = Helpers.PasswordStorage.CreateHash(password)
                };
                conn.Accounts.Add(account);
                conn.SaveChanges();

                Rules.ZoomApi.CreateZoomUserAccount(account.ID);

                conn.EventLogs.Add(new EventLog()
                {
                    Account_ID = account.ID,
                    Data = JsonConvert.SerializeObject(
                        new Models.Auditing.AccountRegistration()
                        {
                            ID = account.ID,
                            AccountType_ID = type,
                            AccountType = ((logic.Constants.Types.AccountTypes) type).ToString(),
                            Email = account.Email,
                            Fname = account.Fname,
                            Lname = account.Lname
                        }),
                    EventDate = DateTime.UtcNow
                });
                conn.SaveChanges();
            }
        }
    }
}
