using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Exceptions
{
    public class WrongAccountException : Exception
    {
        public WrongAccountException() { }
        public WrongAccountException(string message) : base(message) { }
    }
}
