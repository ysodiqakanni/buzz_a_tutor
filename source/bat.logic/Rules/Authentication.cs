using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Shearnie.Net.Encryption;
using bat.data;
using bat.logic.Constants;
using bat.logic.Helpers;
using Newtonsoft.Json;

namespace bat.logic.Rules
{
    public class Authentication
    {
        private const string AUTH_NAME = "1a54e569-10cf-4ec5-9b3e-531a66e2d852";
        private const int COOKIE_EXPIRE_DAYS = 2;
        private IOwinContext context { get; set; }

        public Authentication(IOwinContext context)
        {
            this.context = context;
        }

        public static CookieAuthenticationOptions CookieOptions
        {
            get
            {
                return new CookieAuthenticationOptions
                {
                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                    //AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                    LoginPath = new PathString("/loginsignup"),
                    //CookieDomain = ".myapp.com",
                    CookieName = AUTH_NAME,
                    CookieHttpOnly = true,
                    CookieSecure = CookieSecureOption.SameAsRequest,
                    ExpireTimeSpan = TimeSpan.FromDays(COOKIE_EXPIRE_DAYS),
                    //ReturnUrlParameter = "",
                    //SlidingExpiration = true,
                    Provider = new CookieAuthenticationProvider
                    {
                        // Enables the application to validate the security stamp when the user logs in.
                        // This is a security feature which is used when you change a password or add an external login to your account.  
                        //OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        //    validateInterval: TimeSpan.FromMinutes(30),
                        //    regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                    }
                };
            }
        }

        public Account GetUser(string username, string password)
        {
            if (username.Equals(AdminLogin.User))
            {
#if (!DEBUG)
                if (password != AdminLogin.Password)
                    throw new Exception("Invalid username or password.");
#endif
                return new Account()
                {
                    ID = 0,
                    AccountType_ID = 0,
                    Email = "ADMIN"
                };
            }

            using (var conn = new dbEntities())
            {
                var user =
                    conn.Accounts.FirstOrDefault(
                        a => a.Email.Equals(username, StringComparison.CurrentCultureIgnoreCase));
                if (user == null)
                    throw new Exception("Invalid username or password.");

#if (!DEBUG)
                try
                {
                    if (!Helpers.PasswordStorage.VerifyPassword(password, user.Password))
                        throw new Exception("Invalid username or password.");
                }
                catch (InvalidHashException ex)
                {
                    var ignore = ex;
                    // check plain text
                    if (password != user.Password) throw new Exception("Invalid username or password.");
                    user.Password = Helpers.PasswordStorage.CreateHash(password);
                    conn.SaveChanges();
                }
#endif

                return user;
            }
        }

        public Account GetLoggedInUser()
        {
            if (this.context == null) return null;
            var authenticationManager = this.context.Authentication;
            var claim = authenticationManager.User.Identities.FirstOrDefault();
            if (claim == null) return null;
            var id = claim.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var email = claim.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (id == null || email == null) return null;

            // handle null user in home check if admin
            if (Convert.ToInt32(id.Value) == 0)
                return null;

            return new Account()
            {
                ID = Convert.ToInt32(id.Value),
                Email = email.Value
            };
        }

        public Account GetLoggedInAdminUser()
        {
            if (this.context == null) return null;
            var authenticationManager = this.context.Authentication;
            var claim = authenticationManager.User.Identities.FirstOrDefault();
            if (claim == null) return null;
            var id = claim.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var email = claim.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (id == null || email == null) return null;

            if (Convert.ToInt32(id.Value) != 0)
                return null;

            return new Account()
            {
                ID = Convert.ToInt32(id.Value),
                Email = email.Value
            };
        }

        public void Login(Account user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", AUTH_NAME + "-" + user.ID.ToString())
            };
            var id = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            var authenticationManager = this.context.Authentication;
            authenticationManager.SignIn(id);

            using (var conn = new dbEntities())
            {
                conn.Configuration.AutoDetectChangesEnabled = false;
                conn.Configuration.ValidateOnSaveEnabled = false;

                conn.EventLogs.Add(new EventLog()
                {
                    Account_ID = user.ID,
                    Data = JsonConvert.SerializeObject(
                    new Models.Auditing.AccountLogin()
                    {
                        ID = user.ID,
                        Email = user.Email,
                        Fname = user.Fname,
                        Lname = user.Lname
                    }),
                    EventDate = DateTime.UtcNow
                });
                conn.SaveChanges();
            }
        }

        public void Logout()
        {
            var authenticationManager = this.context.Authentication;
            authenticationManager.SignOut();
        }
    }
}