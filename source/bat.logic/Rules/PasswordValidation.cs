using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace bat.Logic.Rules
{
    public class PasswordValidation
    {
        public static void Match(string password, string password2)
        {
            if (password != password2) throw new Exception("Passwords don't match.");
        }

        public static void Validate(string password)
        {
            //Check regex - The password's first character must be a letter, it must contain at least 4 characters and no more than 15 characters and no characters other than letters, numbers and the underscore may be used
            var passwordNonAlphabet = new Regex(@"[^A-Za-z0-9]");
            var passwordLength = new Regex(@".{5,15}$");
            var passwordNumber = new Regex(@"^(?=.*\d)");
            var passwordUpperCase = new Regex(@"^(?=.*[A-Z])");

            if (passwordNonAlphabet.IsMatch(password)) throw new Exception("Invalid characters in password.");
            if (!passwordLength.IsMatch(password)) throw new Exception("Invalid password length.");
            if (!passwordNumber.IsMatch(password)) throw new Exception("Invalid password requires at least one Numeral.");
            if (!passwordUpperCase.IsMatch(password)) throw new Exception("Invalid password requires at least one Uppercase Letter.");
        }
    }
}
